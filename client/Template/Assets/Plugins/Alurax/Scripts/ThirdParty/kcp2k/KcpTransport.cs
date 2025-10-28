using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
// using Unity.Netcode;
// using Unity.Netcode.Transports.UTP;
// using Unity.Networking.Transport;
// using UnityEngine;
// using NetworkEvent = Unity.Netcode.NetworkEvent;

namespace Alurax
{
    // [AddComponentMenu("Netcode/Kcp Transport")]
    // public class KcpTransport : NetworkTransport
    // {
    //     public struct ConnectionAddressData
    //     {
    //         public string Address;
    //         public ushort Port;
    //     }
    //     
    //     KcpConfig config = new KcpConfig(
    //         // force NoDelay and minimum interval.
    //         // this way UpdateSeveralTimes() doesn't need to wait very long and
    //         // tests run a lot faster.
    //         NoDelay: true,
    //         // not all platforms support DualMode.
    //         // run tests without it so they work on all platforms.
    //         DualMode: false,
    //         Interval: 1, // 1ms so at interval code at least runs.
    //         Timeout: 1000*10,
    //
    //         // large window sizes so large messages are flushed with very few
    //         // update calls. otherwise tests take too long.
    //         SendWindowSize: Kcp.WND_SND * 1000,
    //         ReceiveWindowSize: Kcp.WND_RCV * 1000,
    //
    //         // congestion window _heavily_ restricts send/recv window sizes
    //         // sending a max sized message would require thousands of updates.
    //         CongestionWindow: false,
    //
    //         // maximum retransmit attempts until dead_link detected
    //         // default * 2 to check if configuration works
    //         MaxRetransmits: Kcp.DEADLINK * 2,
    //         
    //         //快速模式
    //         FastResend:2
    //         
    //     );
    //
    //     private KcpServer _server;
    //     private KcpClient _client;
    //     private ulong m_ServerClientId=0;
    //     private static ConnectionAddressData s_DefaultConnectionAddressData = new ConnectionAddressData { Address = "127.0.0.1", Port = 7777 };
    //     
    //     public ConnectionAddressData ConnectionData = s_DefaultConnectionAddressData;
    //     public override ulong ServerClientId => m_ServerClientId;
    //
    //     public override void SetAddressPort(string addr, ushort port)
    //     {
    //         ConnectionData.Address = addr;
    //         ConnectionData.Port = port;
    //         Log.I("[#####DS] SetAddressPort Address:" + addr + " Port:" + port);
    //     }
    //
    //     public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
    //     {
    //         if (_client != null)
    //         {
    //             _checkRtt = true;
    //             _lastSendTime = Time.realtimeSinceStartup;
    //             _client.Send(payload, KcpChannel.Reliable);
    //         }else if (_server != null)
    //         {
    //             _server.Send((int)clientId, payload,KcpChannel.Reliable);
    //         }
    //     }
    //
    //     public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
    //     {
    //         clientId = default;
    //         payload = default;
    //         receiveTime = default;
    //         return NetworkEvent.Nothing;
    //     }
    //
    //     public override bool StartClient()
    //     {
    //         if (_client != null)
    //         {
    //             Log.E("Client already started!");
    //             return false;
    //         }
    //         
    //         _client = new KcpClient(
    //             () =>
    //             {
    //                 InvokeOnTransportEvent(NetworkEvent.Connect,
    //                     ServerClientId,
    //                     default,
    //                     Time.realtimeSinceStartup);
    //             },
    //             (message, channel) =>
    //             {
    //                 if (_checkRtt)
    //                 {
    //                     _checkRtt = false;
    //                     this._lastRttTime = Mathf.Max(0, Time.realtimeSinceStartup - _lastSendTime);
    //                 }
    //
    //                 InvokeOnTransportEvent(NetworkEvent.Data,
    //                     ServerClientId,
    //                     message,
    //                     Time.realtimeSinceStartup); 
    //             },
    //             () =>
    //             {
    //                 InvokeOnTransportEvent(NetworkEvent.Disconnect,
    //                     ServerClientId,
    //                     default,
    //                     Time.realtimeSinceStartup);
    //             },
    //             (error, reason) =>
    //             {
    //                 InvokeOnTransportEvent(NetworkEvent.Disconnect,
    //                     ServerClientId,
    //                     default,
    //                     Time.realtimeSinceStartup);
    //             },
    //             config
    //         );
    //         Log.I($"Connect ip:{ConnectionData.Address}:{ConnectionData.Port}");
    //         _client.Connect(ConnectionData.Address, ConnectionData.Port);
    //         return true;
    //     }
    //
    //     public override bool StartServer()
    //     {
    //         if (_server != null)
    //         {
    //             Log.E("Server already started!");
    //             return false;
    //         }
    //
    //         _server = new KcpServer(
    //              (connectionId) =>
    //              {
    //                  InvokeOnTransportEvent(NetworkEvent.Connect,
    //                      (ulong)connectionId,
    //                      default,
    //                      Time.realtimeSinceStartup);
    //                  
    //              },
    //             (connectionId, message, channel) =>
    //              {
    //                  InvokeOnTransportEvent(NetworkEvent.Data,
    //                      (ulong)connectionId,
    //                      message,
    //                      Time.realtimeSinceStartup); 
    //              },
    //              (connectionId) =>
    //              {
    //                  InvokeOnTransportEvent(NetworkEvent.Disconnect,
    //                      (ulong)connectionId,
    //                      default,
    //                      Time.realtimeSinceStartup);
    //              },
    //             (connectionId, error, reason) =>
    //              {
    //                  InvokeOnTransportEvent(NetworkEvent.Disconnect,
    //                      (ulong)connectionId,
    //                      default,
    //                      Time.realtimeSinceStartup);
    //              },
    //             config
    //         );
    //         // start server
    //         Log.I($"Start Server Port:{ConnectionData.Port}");
    //         _server.Start(ConnectionData.Port);
    //         return true;
    //     }
    //
    //     public override void DisconnectRemoteClient(ulong clientId)
    //     {
    //        if(_server!=null)
    //            _server.Disconnect((int)clientId);
    //     }
    //
    //     public override void DisconnectLocalClient()
    //     {
    //         if (_client != null)
    //         {
    //             _client.Disconnect();
    //             _client = null;
    //         } 
    //         if (_server != null)
    //         {
    //             _server.Stop();
    //             _server = null;
    //         }
    //     }
    //
    //     private float _lastSendTime;
    //     private float _lastRttTime;
    //     private bool _checkRtt = false;
    //     public override ulong GetCurrentRtt(ulong clientId)
    //     {
    //         return (ulong)(_lastRttTime * 1000 );
    //     }
    //
    //     public override void Shutdown()
    //     {
    //         DisposeInternals();
    //         m_ServerClientId = 0;
    //     }
    //
    //     public override void Initialize(NetworkManager networkManager = null)
    //     {
    //       
    //     }
    //
    //
    //     private void DisposeInternals()
    //     {
    //         if (_client!=null)
    //         {
    //             _client.Disconnect();
    //             _client = null;
    //         }
    //         if(_server!=null)
    //         {
    //             _server.Stop();
    //             _server = null;
    //         }
    //     }
    //     private void Update()
    //     {
    //         _client?.Tick();
    //         _server?.Tick();
    //     }
    //     
    //     private void OnDestroy()
    //     {
    //         DisposeInternals();
    //     }
    // }
}