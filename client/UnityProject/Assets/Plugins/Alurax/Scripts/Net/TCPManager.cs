using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;

namespace Alurax
{
	public static class TCPManager
	{
		#region Public Callbacks
		public static bool IsConnectingServer { get; internal set; }

		public static CustomAction OnConnected = new CustomAction(); // 主动连接成功
		public static CustomAction OnConnectFailed = new CustomAction(); // 主动连接失败
		public static CustomAction OnReConnected = new CustomAction();// 断线重连成功
		public static CustomAction OnReConnectFailed = new CustomAction();// 断线重连失败
		public static CustomAction<int> OnReConnectStart = new CustomAction<int>();// 开始断线重连
		public static CustomAction OnDisConnected = new CustomAction(); // 被动断开连接
		public static CustomAction OnKeepAlive = new CustomAction(); // 处理心跳包发送
		public static CustomAction<NetPacket> OnDispatch = new CustomAction<NetPacket>(); //分发协议
		#endregion


		private static NetConfig _config = new NetConfig();
		// 客户端
		private static string _connectIp;
		private static ushort _connectPort;
		private static TCPClient _tcpClient;
		private static bool _loadingScene;
		private static MemoryStream _stream = new MemoryStream();
		
		// 断线重连
        private static bool _reconnectAuto;
        private static bool _reconnectFailed;  
        private static int  _reconnectTimes;        
        
        // 消息队列
        private static ConcurrentQueue<NetPacket> _messageQueue = new ConcurrentQueue<NetPacket>();

        static TCPManager()
        {
	        TaskManager.Update(Tick);
        }

        public static void Config(NetConfig config)
        {
	        _config = config;
	        NetPacket.CustomMetaSerializer = config.MetaSerializer;
        }
        
		public static void Connect(string ip, ushort port)
		{
			_connectIp = ip;
			_connectPort = port;
			if (IsConnected())
			{
				_tcpClient.Disconnect(false);
			}
			InternalConnect(false);
		}
		
		public static void DisConnect(bool needReconnect = false)
		{
			_reconnectAuto = needReconnect;
			if (_tcpClient != null)
			{
				_tcpClient.Disconnect(needReconnect);
				_tcpClient = null;
			}
		}		
		
		public static bool IsConnected()
		{
			if (_tcpClient != null)
			{
				return _tcpClient.IsConnected;
			}
			return false;
		}	
		
        public static void StopConnect()
        {
            DisConnect();
        }
        
        public static void SendPacket(ushort msgId, byte[] body)
        {
	        if (_tcpClient != null)
	        {
		        NetPacket packet = new NetPacket();
		        packet.Serialize(_stream, msgId, body);
		        _tcpClient.Send(packet.Send);
	        }
	        else
                Log.W("Game Server not connected");
        }

        public static void SetLoadingScene(bool isLoading)
        {
	        _loadingScene = isLoading;
        }
        

        //网络回调，每1/10秒回调。如果卡机或者在后台，会跳回调
        static void Tick()
        {
	        if (_loadingScene) return;
	        
	        if (Time.frameCount % 5 == 0)
	        {
		        if (IsConnected())
		        {
			        _tcpClient.Tick();
		        }
		        else
		        {
			        CheckConnectState();
		        }
	        }
	        DoCmd();
        }
        
		static void CheckConnectState()
		{
			if (!IsConnected() && _reconnectAuto)
			{
				if (!IsConnectingServer)
				{
					if (_reconnectTimes < _config.ReConnectMaxCount)
					{
						_reconnectFailed = false;
						_reconnectTimes++;
						Log.I($"第{_reconnectTimes}次重连");
						OnReConnectStart?.Invoke(_reconnectTimes);
						InternalConnect(true);
					}
					else
					{
						if (!_reconnectFailed)
						{
							_reconnectFailed = true;
							Log.I($"重连彻底失败");
							OnReConnectFailed?.Invoke();
						}
					}
				}
			}
		}

		private static void DoCmd()
		{
			try
			{
				while (_messageQueue.TryDequeue(out NetPacket packet))
				{
					OnDispatch?.Invoke(packet);
				}
			}
			catch (System.Exception err)
			{
				Log.E("DoCmd Error: " + err.Message + err.StackTrace);
			}
		}

		static void InternalConnect(bool isReconnect)
		{
			if (_tcpClient == null)
			{
				_reconnectAuto = false;
				_tcpClient = new TCPClient();
				_tcpClient.SetRecvPacketCallback(OnRecvPacketCallback);
				_tcpClient.SetDisconnectCallback(OnDisconnectCallBack);
				_tcpClient.SetKeepAliveCallback(OnKeepAliveCallBack);
				_tcpClient.SetKeepAliveMS(_config.KeepAliveIntervalMS);
				_tcpClient.SetRecvBufferSize(_config.RecvBufferSize);
			}

			IsConnectingServer = true;
			_tcpClient.Connect(_connectIp, _connectPort, (connected) =>
			{
				if(connected)
				{
					_reconnectTimes = 0;
					IsConnectingServer = false;
					if(isReconnect)
						OnReConnected?.Invoke();
					else
						OnConnected?.Invoke();
				}
				else
				{
					IsConnectingServer = false;
					if(!isReconnect)
						OnConnectFailed?.Invoke();
				}
			});
		}
		
		private static void ConnectionFailed(string reason)
		{
			Log.E($"ConnectionFailed {reason}");
			DisConnect(true);
		}
		
		private static void OnDisconnectCallBack(bool reconnect)
		{
			_reconnectTimes = 0;
			_reconnectAuto = reconnect;
			OnDisConnected?.Invoke();
		}

		private static void OnKeepAliveCallBack()
		{
			OnKeepAlive?.Invoke();
		}

		private static void OnRecvPacketCallback(NetPacket packet)
		{
			_messageQueue.Enqueue(packet);
		}
		

	}
}
