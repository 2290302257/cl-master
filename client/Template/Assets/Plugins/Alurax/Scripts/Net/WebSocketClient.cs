using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using UnityWebSocket;
using Alurax;


namespace Alurax
{
    public class WebSocketClient
    {
	    public string Host { get; private set; }
		public ushort Port { get; private set; }

		public bool IsConnected
		{
			get
			{
				if (m_Socket != null)
					return m_Socket.ReadyState == WebSocketState.Open;
				return false;
			}
		}
		
		private Action<bool> OnConnectHandler { get; set; } //连接回调
		private Action<bool> OnDisconnectHandler { get; set; } //断开连接回调
		private Action OnKeepAliveHandler { get; set; } //心跳包回调
		private Action<byte[]> OnReceivePacketHandler { get; set; } //接收包回调
		
		private IWebSocket m_Socket;
		private static MemoryStream _stream = new MemoryStream();
		private Coroutine m_CheckSocket;
		private bool m_IsWaittinConnect;

		//定时检测连接
		private int m_KeepAliveTimeMS;
		private DateTime m_LastKeepAliveTime;
        private bool m_KeepAliving;
		public WebSocketClient(Action<bool> connect, Action<bool> disconnect, Action<byte[]> receive)
		{
			m_KeepAliving = false;
            m_KeepAliveTimeMS = 0;
			m_LastKeepAliveTime = DateTime.Now;
			
			OnConnectHandler += connect;
			OnDisconnectHandler += disconnect;
			OnReceivePacketHandler += receive;
		}

		//建立连接
		public void Connect(string host, ushort port)
		{
			try
			{
				Host = host;
				Port = port;
				
				if (m_Socket is { ReadyState: WebSocketState.Connecting or WebSocketState.Open })
				{
					Disconnect(false);
				}
				m_Socket ??= new WebSocket($"ws://{host}:{port}");//wss
				m_Socket.OnOpen += Socket_OnOpen;
				m_Socket.OnMessage += Socket_OnMessage;
				m_Socket.OnClose += Socket_OnClose;
				m_Socket.OnError += Socket_OnError;
				m_Socket.ConnectAsync();

				m_IsWaittinConnect = true;
			}
			catch (Exception e)
			{
				Log.E(e);
			}
		}
		//断开连接
		public void Disconnect(bool reconnect = false)
		{
			if (m_Socket != null)
			{
				m_Socket.OnOpen -= Socket_OnOpen;
				m_Socket.OnMessage -= Socket_OnMessage;
				m_Socket.OnClose -= Socket_OnClose;
				m_Socket.OnError -= Socket_OnError;
				m_Socket.CloseAsync();
				m_Socket = null;
				Log.I("Disconnect");
				OnDisconnectHandler?.Invoke(reconnect);
			}
		}

		public void Send(byte[] data)
		{
			try
			{
				m_Socket.SendAsync(data);
			}
			catch (Exception e)
			{
				Disconnect(true);
				Log.E($"send failed: {e}");
			}
		}

		private void Receive(byte[] data)
		{
			OnReceivePacketHandler?.Invoke(data);
		}

		#region WebSocketCallBack

		private void Socket_OnOpen(object sender, UnityWebSocket.OpenEventArgs e)
		{
			if (IsConnected)
			{
				OnConnectHandler?.Invoke(true);
				Log.I("Connected");
			}
			else
			{
				OnConnectHandler?.Invoke(false);
				Log.I("Connect Filed");
			}
		}

		private void Socket_OnMessage(object sender, UnityWebSocket.MessageEventArgs e)
		{
			if (e.IsBinary)
			{
				Receive(e.RawData);
			}
		}

		private void Socket_OnClose(object sender, UnityWebSocket.CloseEventArgs e)
		{
			Disconnect(true);
			Log.E($"Close : Code : {e.Code}, Reason : {e.Reason}, StatusCode : {e.StatusCode}");
		}

		private void Socket_OnError(object sender, UnityWebSocket.ErrorEventArgs e)
		{
			Disconnect(false);
			Log.E($"Error : {e.Exception} : {e.Message}");
		}
		
		#endregion
    }
}
