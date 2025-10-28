using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using kcp2k;
using UnityEngine;

namespace Alurax
{
    public class NetworkManager
    {
        KcpConfig config = new KcpConfig(
            // force NoDelay and minimum interval.
            // this way UpdateSeveralTimes() doesn't need to wait very long and
            // tests run a lot faster.
            NoDelay: true,
            // not all platforms support DualMode.
            // run tests without it so they work on all platforms.
            DualMode: false,
            Interval: 1, // 1ms so at interval code at least runs.
            Timeout: 1000*60*10,

            // large window sizes so large messages are flushed with very few
            // update calls. otherwise tests take too long.
            SendWindowSize: Kcp.WND_SND * 1000,
            ReceiveWindowSize: Kcp.WND_RCV * 1000,

            // congestion window _heavily_ restricts send/recv window sizes
            // sending a max sized message would require thousands of updates.
            CongestionWindow: false,

            // maximum retransmit attempts until dead_link detected
            // default * 2 to check if configuration works
            MaxRetransmits: Kcp.DEADLINK * 2,
            
            //快速模式
            FastResend:2
            
        );
        
        //server side
        public Action OnServerStarted = null;
        public Action OnServerStop = null;
        public Action<int> OnServerClientConnected = null;
        public Action<int> OnServerClientDisconnect = null;
        public Action<int> OnServerClientError= null;
        public Action<int,ArraySegment<byte>> OnServerReceiveData = null;
        

        //client side
        public Action OnClientStarted = null;
        public Action OnClientConnected = null;
        public Action OnClientDisconnect = null;
        public Action<ArraySegment<byte>> OnClientReceiveData = null;

        //common side
        public Action OnTicked = null;
        
        static NetworkManager _singleton;
        public static NetworkManager Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new NetworkManager();
                return _singleton;
            }
        }
        
        
        public bool IsClientConnected { get; private set; }
        public bool IsServer{ get; private set; }
        public bool IsClient{ get; private set; }
        public bool IsHost{ get; private set; }
        public bool IsLocal{ get; private set; }

        private NetLocalServer _local;
        private KcpServer _server;
        private KcpClient _client;
        private string _address;
        private ushort _port;
        private int _serverClientId;
        private int _localTimer;
        
        public NetworkManager()
        {
           var network = Alurax.Inst.gameObject.AddComponent<InternalNetwork>();
           network.OnUpdate = Update;
           network.OnDestroyed= OnDestroy;
        }

        public void SetConfig(ushort port)
        {
            SetConfig(string.Empty, port, 0);
        }
        
        public void SetConfig(string addr, ushort port)
        {
            SetConfig(addr, port, 0);
        }
        
        public void SetConfig(string addr, ushort port, int serverClientId)
        {
            _address = addr;
            _port = port;
            _serverClientId= serverClientId;
            Log.I($"[#####DS] SetAddressPort Address:{addr} Port:{port} serverClientId:{serverClientId}");
        }
        
        public void SendToServer(ArraySegment<byte> payload)
        {
            if (_client != null)
            {
                _client.Send(payload, KcpChannel.Reliable);
            }
            else if (_local != null)
            {
                _local.SendToServer(payload);
            }
            
            if (IsHost)
            {
                OnServerReceiveData?.Invoke(_serverClientId, payload);
            }
        }
        
        public void SendToClient(int clientId, ArraySegment<byte> payload)
        {
            if (_server != null)
            {
                _server.Send(clientId, payload,KcpChannel.Reliable);
            }
            else if (_local != null)
            {
                _local.SendToClient(clientId, payload);
            }
            
            if (IsHost && clientId == _serverClientId)
            {
                OnClientReceiveData?.Invoke(payload);
            }
        }

        public bool StartHost()
        {
            if (_server != null)
            {
                Log.E("host already started!");
                return false;
            }
            IsHost = IsServer = IsClient = IsClientConnected = true;
            IsLocal = false;
            
            InternalStartServer();
            OnServerStarted?.Invoke();
            OnClientStarted?.Invoke();
            OnServerClientConnected?.Invoke(_serverClientId);
            OnClientConnected?.Invoke();
            return true;
        }
        
        public bool StartLocal()
        {
            if (_local != null)
            {
                Log.E("Local already started!");
                return false;
            }
            IsLocal = IsClient = IsServer = IsClientConnected = true;
            IsHost = false;
            
            _local = new NetLocalServer(_serverClientId, (data) =>
                {
                    this.OnClientReceiveData?.Invoke(data);
                },(clientId, data) =>
                {
                    this.OnServerReceiveData?.Invoke(clientId, data);
                });
            
            OnServerStarted?.Invoke();
            OnClientStarted?.Invoke();
            OnServerClientConnected?.Invoke(_serverClientId);

            _localTimer=TaskManager.NextFrame(() =>
            {
                OnClientConnected?.Invoke();
            });
            return true;
        }
        
        public bool StartClient()
        {
            if (_client != null)
            {
                Log.E("Client already started!");
                return false;
            }
            IsLocal = IsServer = IsHost = IsClientConnected = false;
            IsClient = true;
            
            _client = new KcpClient(
                () =>
                {
                    IsClientConnected = true;
                    this.OnClientConnected?.Invoke();
                },
                (message, channel) =>
                {
                    this.OnClientReceiveData?.Invoke(message.ToArray());
                },
                () =>
                {
                    IsClientConnected = false;
                    this.OnClientDisconnect?.Invoke();
                },
                (error, reason) =>
                {
                    Log.E($"[#####DS] OnError {error} {reason}");
                    IsClientConnected = false;
                    this.OnClientDisconnect?.Invoke();
                },
                config
            );
            Log.I($"[#####DS] Connect ip:{_address}:{_port}");
            _client.Connect(_address, _port);
            OnClientStarted?.Invoke();
            return true;
            return true;
        }

        public bool StartServer()
        {
            if (_server != null)
            {
                Log.E("Server already started!");
                return false;
            }

            IsLocal = IsClient = IsHost = IsClientConnected = false;
            IsServer = true;

            InternalStartServer();
            OnServerStarted?.Invoke();
            return true;
        }

        public void DisconnectRemoteClient(uint clientId)
        {
           if(_server!=null)
               _server.Disconnect((int)clientId);
        }

        public void DisconnectLocalClient()
        {
            if(!IsHost && !IsLocal)
                DisposeInternals();
        }

        public void Shutdown()
        {
            DisposeInternals();
            _serverClientId = 0;
        }
        private void InternalStartServer()
        {
            _server = new KcpServer(
                (connectionId) =>
                {
                    this.OnServerClientConnected?.Invoke(connectionId);
                },
                (connectionId, message, channel) =>
                {
                    this.OnServerReceiveData?.Invoke(connectionId,message.ToArray());
                },
                (connectionId) =>
                {
                    this.OnServerClientDisconnect?.Invoke(connectionId);
                },
                (connectionId, error, reason) =>
                {
                    Log.E($"[#####DS] ip : {connectionId} OnError {error} {reason}");
                    this.OnServerClientError?.Invoke(connectionId);
                    this.OnServerClientDisconnect?.Invoke(connectionId);
                },
                config
            );
            // start server
            Log.I($"[#####DS] Start Server Port:{_port}");
            _server.Start(_port);
        }
        private void DisposeInternals()
        {
            IsClientConnected = false;
            if (_client != null)
            {
                _client.Disconnect();
                _client = null;
            }
            
            if(_server != null)
            {
                _server.Stop();
                _server = null;
                OnServerStop?.Invoke();
            }

            if (_local != null)
            {
                TaskManager.Remove(_localTimer);
                _local = null;
                OnServerStop?.Invoke();
                OnClientDisconnect?.Invoke();
            }
        }
        private void Update()
        {
            _client?.Tick();
            _server?.Tick();
            OnTicked?.Invoke();
        }
        private void OnDestroy()
        {
            DisposeInternals();
        }
        
        class InternalNetwork : MonoBehaviour
        {
            public Action OnUpdate;
            public Action OnDestroyed;
            void Update()
            {
                OnUpdate?.Invoke();
            }
            private void OnDestroy()
            {
                OnDestroyed?.Invoke();
            }
        }
    }
}
