using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Text;

namespace Alurax
{
	public class TCPClient
	{
		private int mRecvBufferSize = 1 * 1024 * 1024; // 最大的接收缓存大小 1MB
		
		public string Host { get; private set; }
		public ushort Port { get; private set; }

		public bool IsConnected
		{
			get
			{
				if (m_Socket != null)
					return m_Socket.Connected;
				return false;
			}
		}
		
		private Action<bool> onDisconnectHandler { get; set; } //断开连接回调
		private Action onKeepAliveHandler { get; set; } //心跳包回调
		private Action<NetPacket> onRecvPacketHandler;  //接收包回调

		private Socket m_Socket;
		private Coroutine m_CheckSocket;
		private CancellationTokenSource m_Cts = new CancellationTokenSource();
		private bool m_IsWaittinConnect;
		
		// 缓存收到的二进制数据
		private byte[] m_Recvbuf;
		private byte[] m_Leavebuf;
		private int m_LeaveBufLen;
		
		// 心跳包自动发送配置
		private int m_KeepAliveTimeMS;
		private DateTime m_LastKeepAliveTime;
        private bool m_KeepAliving;
		public TCPClient()
		{
			m_KeepAliving = false;
            m_KeepAliveTimeMS = 0;
			m_LastKeepAliveTime = DateTime.Now;
			SetRecvBufferSize(mRecvBufferSize);
		}
		
		/*
		 *  建立连接
		 */
		public void Connect(string host, ushort port, Action<bool> connectCallback , int timeoutMS = 2000)
		{
			try
			{
				Host = host;
				Port = port;
				bool ipv4 = false;
				bool ipv6 = false;
				Log.I($"connect {host}:{port}");
				if (!System.Net.IPAddress.TryParse(host, out var __))
				{
					IPAddress[] addresses = Dns.GetHostAddresses(host);
					foreach (IPAddress address in addresses)
					{
						Log.I($"ip address {address}");
						if (address.AddressFamily == AddressFamily.InterNetwork)
							ipv4 = true;
						else if (address.AddressFamily == AddressFamily.InterNetworkV6)
							ipv6 = true;
					}
				}
				else
				{
					ipv4 = true;
					Log.I($"ip host {host}");
				}
				m_Socket?.Close(); 
				m_Socket = null;
#if (UNITY_IOS && !UNITY_EDITOR)
				Socket socket = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#else
				Socket socket = new Socket(ipv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
#endif
				socket.SendTimeout = timeoutMS;
				m_IsWaittinConnect = true;
				m_LeaveBufLen = 0;
				socket.BeginConnect(host, port, new AsyncCallback(ConnectCallback), socket);
			}
			catch (Exception e)
			{
				Log.E(e);
			}finally
			{
				if (m_CheckSocket != null)
				{
					Alurax.Inst.StopCoroutine(m_CheckSocket);
					m_CheckSocket = null;
				}
				m_CheckSocket = Alurax.Inst.StartCoroutine(WaitConnect(connectCallback));
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			Socket socket = ar.AsyncState as Socket;
			if (socket != null && socket.Connected)
			{
				socket.EndConnect(ar);
				if (m_IsWaittinConnect)
					m_Socket = socket;
				else
					socket.Close();
			}
		}
		
		public void Disconnect(bool reconnect)
		{
			if (m_Socket != null)
			{
				m_Socket.Close();
				m_Socket = null;
				onDisconnectHandler?.Invoke(reconnect);
			}
		}

		/*
		 *  设置连接意外断开时(网络故障、服务器主动关闭等)的回调函数
		 */
		public void SetDisconnectCallback(Action<bool> callback)
		{
			onDisconnectHandler += callback;
		}
		
		public void SetKeepAliveCallback(Action callback)
		{
			onKeepAliveHandler += callback;
		}

		public void SetRecvPacketCallback(Action<NetPacket> callback)
		{
			onRecvPacketHandler += callback;
		}

		public void Send(MemoryStream stream)
		{
			try
			{
				m_Socket.Send(stream.GetBuffer(),0,(int)stream.Length, SocketFlags.None);
			}
			catch (Exception e)
			{
				Disconnect(true);
				Log.E($"send failed: {e}");
			}
		}

		void DoReceiveLoop(object state)
		{
			CancellationToken token = (CancellationToken) state;
			while (true)
			{
				try
				{
					if (token.IsCancellationRequested)
					{
						return;
					}
					
					if(m_Socket !=null && m_Socket.Connected)
					{
						int received = m_Socket.Receive(m_Recvbuf, m_LeaveBufLen, mRecvBufferSize-m_LeaveBufLen, SocketFlags.None);
						if (m_LeaveBufLen > 0)
						{
							Array.Copy(m_Leavebuf,0,m_Recvbuf,0,m_LeaveBufLen);
							received += m_LeaveBufLen;
							m_LeaveBufLen = 0;
						}
						if (received > 0)
							ReceivePayload(received);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
					Disconnect(true);
					break;
				}
			}
		}

		private void ReceivePayload(int length)
		{
			// 被析构 则直接返回
			if (m_Socket == null || !m_Socket.Connected)
			{
				Log.W("socket has been disposed.");
				return;
			}
			
			// 如果接受数据长度为0，表示对方关闭了连接
			if (length == 0)
			{
				Disconnect(true);
				return;
			}
			int leaveIndex = 0;
			while (leaveIndex < length)
			{
				NetPacket netPacket = new NetPacket();
				if (netPacket.Deserialize(m_Recvbuf, length, leaveIndex, out var processLen))
				{
					leaveIndex += processLen;
					onRecvPacketHandler(netPacket);
				}
				else
				{
					m_LeaveBufLen = length - leaveIndex;
					Array.Copy(m_Recvbuf, leaveIndex, m_Leavebuf, 0, m_LeaveBufLen);
					break;
				}
			}
		}

		public void SetRecvBufferSize(int size)
		{
			m_LeaveBufLen = 0;
			mRecvBufferSize = size;
			m_Recvbuf = new byte[mRecvBufferSize];
			m_Leavebuf = new byte[mRecvBufferSize];
		}
		
		public bool SetKeepAliveMS(int timeMS)
		{
			if (timeMS <= 0)
			{
				Log.W("time_ms <= 0 or packet == null");
				return false;
			}

			m_KeepAliveTimeMS = timeMS;
			return true;
		}

		public void Tick()
		{
			if (IsConnected && m_KeepAliving)
			{
				TimeSpan timeSpan = DateTime.Now - m_LastKeepAliveTime;
				if (timeSpan.TotalMilliseconds >= m_KeepAliveTimeMS)
				{
					onKeepAliveHandler?.Invoke();
					m_LastKeepAliveTime = DateTime.Now;
				}
			}
		}

		IEnumerator WaitConnect(Action<bool> callback)
		{
			//2秒超时
			m_IsWaittinConnect = true;
			float start = Time.time;
			while (!IsConnected)
			{
				float connectTime = Time.time - start;
				if(connectTime < 2f)
					yield return null;
				else
					break;
			}
			m_IsWaittinConnect = false;
			if (IsConnected)
			{
				Log.I($"connect {Host}:{Port} success");
				callback?.Invoke(true);
				m_KeepAliving = true;
				m_Cts.Cancel();
				m_Cts = new CancellationTokenSource();
				System.Threading.ThreadPool.QueueUserWorkItem(DoReceiveLoop,m_Cts.Token);
			}
			else
			{
				Log.I($"connect {Host}:{Port} timeout");
				callback?.Invoke(false);
			}
		}
	}
}
