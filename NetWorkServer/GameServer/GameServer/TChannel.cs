using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ETModel
{
	/// <summary>
	/// 封装Socket,将回调push到主线程处理
	/// </summary>
	public sealed class TChannel
	{
		private Socket socket;
		private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
		private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

		private readonly byte[] recvBuffer = new byte[1024];
		private readonly byte[] sendBuffer = new byte[1024];

		private readonly MemoryStream memoryStream;

		private bool isSending;

		private bool isRecving;

		public IPEndPoint RemoteAddress;

        private bool isConnected;


		private readonly byte[] packetSizeCache;
		
		public TChannel(IPEndPoint ipEndPoint)
		{
			this.memoryStream = new MemoryStream();
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.socket.NoDelay = true;
			this.innArgs.Completed += this.OnComplete;
			this.outArgs.Completed += this.OnComplete;

			this.RemoteAddress = ipEndPoint;

			this.isConnected = false;
			this.isSending = false;
		}
		

		public void Dispose()
		{
			this.socket.Close();
			this.innArgs.Dispose();
			this.outArgs.Dispose();
			this.innArgs = null;
			this.outArgs = null;
			this.socket = null;
			this.memoryStream.Dispose();
		}
		
		public  MemoryStream Stream
		{
			get
			{
				return this.memoryStream;
			}
		}

		public  void Start()
		{
			if (!this.isConnected)
			{
				this.ConnectAsync(this.RemoteAddress);
				return;
			}

			if (!this.isRecving)
			{
				this.isRecving = true;
				this.StartRecv();
			}

			//this.GetService().MarkNeedStartSend(this.Id);
		}
		


		private void OnComplete(object sender, SocketAsyncEventArgs e)
		{

		}

		public void ConnectAsync(IPEndPoint ipEndPoint)
		{
			this.outArgs.RemoteEndPoint = ipEndPoint;
			if (this.socket.ConnectAsync(this.outArgs))
			{
				return;
			}
			OnConnectComplete(this.outArgs);
		}

		private void OnConnectComplete(object o)
		{
			if (this.socket == null)
			{
				return;
			}
			SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;
			
			if (e.SocketError != SocketError.Success)
			{
				this.OnError((int)e.SocketError);	
				return;
			}

			e.RemoteEndPoint = null;
			this.isConnected = true;
			
			this.Start();
		}
		public int Error { get; set; }
		private Action<TChannel, int> errorCallback;

		public event Action<TChannel, int> ErrorCallback
		{
			add
			{
				this.errorCallback += value;
			}
			remove
			{
				this.errorCallback -= value;
			}
		}

		private Action<MemoryStream> readCallback;

		public event Action<MemoryStream> ReadCallback
		{
			add
			{
				this.readCallback += value;
			}
			remove
			{
				this.readCallback -= value;
			}
		}

		protected void OnRead(MemoryStream memoryStream)
		{
			this.readCallback.Invoke(memoryStream);
		}

		protected void OnError(int e)
		{
			this.Error = e;
			this.errorCallback?.Invoke(this, e);
		}

		private void OnDisconnectComplete(object o)
		{
			SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
			this.OnError((int)e.SocketError);
		}

		private void StartRecv()
		{
			this.RecvAsync(this.recvBuffer	);
		}

		public void RecvAsync(byte[] buffer, int offset = 0, int count =0)
		{
			try
			{
				this.innArgs.SetBuffer(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
			}
			
			if (this.socket.ReceiveAsync(this.innArgs))
			{
				return;
			}
			OnRecvComplete(this.innArgs);
		}

		private void OnRecvComplete(object o)
		{
			if (this.socket == null)
			{
				return;
			}
			SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;

			if (e.SocketError != SocketError.Success)
			{
				this.OnError((int)e.SocketError);
				return;
			}

			if (e.BytesTransferred == 0)
			{
				//this.OnError(ErrorCode.ERR_PeerDisconnect);
				return;
			}

			//this.recvBuffer.LastIndex += e.BytesTransferred;
			//if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
			//{
			//	//this.recvBuffer.AddLast();
			//	//this.recvBuffer.LastIndex = 0;
			//}

			// 收到消息回调
			while (true)
			{
				try
				{
					//if (!this.parser.Parse())
					//{
					//	break;
					//}
				}
				catch (Exception ee)
				{
					//Log.Error($"ip: {this.RemoteAddress} {ee}");
					//this.OnError(ErrorCode.ERR_SocketError);
					return;
				}

				try
				{
					//this.OnRead(this.parser.GetPacket());
				}
				catch (Exception ee)
				{
					//Log.Error(ee);
				}
			}

			if (this.socket == null)
			{
				return;
			}
			
			this.StartRecv();
		}

		public bool IsSending => this.isSending;

		public void StartSend()
		{
			if(!this.isConnected)
			{
				return;
			}
			
			// 没有数据需要发送
			if (this.sendBuffer.Length == 0)
			{
				this.isSending = false;
				return;
			}

			this.isSending = true;

			/*
			int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
			if (sendSize > this.sendBuffer.Length)
			{
				sendSize = (int)this.sendBuffer.Length;
			}

			this.SendAsync(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
			*/
		}

		public void SendAsync(byte[] buffer, int offset, int count)
		{
			try
			{
				this.outArgs.SetBuffer(buffer, offset, count);
			}
			catch (Exception e)
			{
				throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
			}
			if (this.socket.SendAsync(this.outArgs))
			{
				return;
			}
			OnSendComplete(this.outArgs);
		}

		private void OnSendComplete(object o)
		{
			if (this.socket == null)
			{
				return;
			}
			SocketAsyncEventArgs e = (SocketAsyncEventArgs) o;

			if (e.SocketError != SocketError.Success)
			{
				this.OnError((int)e.SocketError);
				return;
			}
			
			if (e.BytesTransferred == 0)
			{
				//this.OnError(ErrorCode.ERR_PeerDisconnect);
				return;
			}
			
			//this.sendBuffer.FirstIndex += e.BytesTransferred;
			//if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize)
			//{
			//	this.sendBuffer.FirstIndex = 0;
			//	this.sendBuffer.RemoveFirst();
			//}
			
			this.StartSend();
		}
	}
}