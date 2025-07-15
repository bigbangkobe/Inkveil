using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace Framework
{
	/// <summary>
	/// 网络服务器
	/// </summary>
	public sealed class NetworkServer
	{
		private const string LOG_ERROR = "Unknown error occurred in network.";
		private const string LOG_HEAD = "Network message head data is error:{0} {1}";
		private const string LOG_CONNECT = "{0} socket is connected.";
		private const string LOG_DISCONNECT = "{0} socket is disconnected.";
		private const string LOG_GET = "Can't find out network message:{0}({1}).";
		private const string LOG_RECEIVE = "Can't find out network message callback:{0}.";

		/// <summary>
		/// 消息头数据
		/// </summary>
		private const short HEAD = 0x0C03;
		/// <summary>
		/// 消息头大小(消息头数据(short) + 消息ID(short) + 数据长度(int))
		/// </summary>
		private const int HEAD_SIZE = 8;
		/// <summary>
		/// 加密钥匙
		/// </summary>
		private static readonly byte[] s_Key = { 77, 22, 34, 38, 68, 35, 25, 66 };

		/// <summary>
		/// 连接回调
		/// </summary>
		public event Action<bool, string> onConnect;
		/// <summary>
		/// 断开回调
		/// </summary>
		public event Action<string> onDisconnect;
		/// <summary>
		/// 接收回调
		/// </summary>
		public event Action<NetworkMessage> onReceive;
		/// <summary>
		/// 发送回调
		/// </summary>
		public event Action<NetworkMessage> onSend;
		/// <summary>
		/// Ping回调
		/// </summary>
		public event Action<int> onPing;

		/// <summary>
		/// 服务器名
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// 是否已连接
		/// </summary>
		public bool isConnected { get; private set; }
		/// <summary>
		/// 本地IP
		/// </summary>
		public string localHost { get { return isConnected ? ((IPEndPoint)m_Socket.LocalEndPoint).Address.ToString() : null; } }
		/// <summary>
		/// 本地端口
		/// </summary>
		public int localPort { get { return isConnected ? ((IPEndPoint)m_Socket.LocalEndPoint).Port : 0; } }
		/// <summary>
		/// 远程IP
		/// </summary>
		public string remoteHost { get { return isConnected ? ((IPEndPoint)m_Socket.RemoteEndPoint).Address.ToString() : null; } }
		/// <summary>
		/// 远程端口
		/// </summary>
		public int remotePort { get { return isConnected ? ((IPEndPoint)m_Socket.RemoteEndPoint).Port : 0; } }
		/// <summary>
		/// 网络延迟
		/// </summary>
		public int ping { get; private set; }
		/// <summary>
		/// 接收字节
		/// </summary>
		public long receiveBytes { get; private set; }
		/// <summary>
		/// 发送字节
		/// </summary>
		public long sendBytes { get; private set; }

		/// <summary>
		/// Socket对象
		/// </summary>
		private Socket m_Socket;
		/// <summary>
		/// Buffer
		/// </summary>
		private byte[] m_Buffer;
		/// <summary>
		/// Buffer索引值
		/// </summary>
		private int m_BufferIndex;
		/// <summary>
		/// Buffer链表
		/// </summary>
		private List<byte> m_BufferList = new List<byte>();
		/// <summary>
		/// 消息表 [键:消息ID 值:协议信息]
		/// </summary>
		private Dictionary<short, MessageInfo> m_MessageMap = new Dictionary<short, MessageInfo>();
		/// <summary>
		/// 解密钥匙
		/// </summary>
		private byte[] m_DecryptKey;
		/// <summary>
		/// 备份钥匙
		/// </summary>
		private byte[] m_BackupKey;
		/// <summary>
		/// 加密钥匙
		/// </summary>
		private byte[] m_EncryptKey;
		/// <summary>
		/// Short字节数据
		/// </summary>
		private byte[] m_ShortBytes = new byte[2];
		/// <summary>
		/// Int字节数据
		/// </summary>
		private byte[] m_IntBytes = new byte[4];
		/// <summary>
		/// 异常消息
		/// </summary>
		private string m_ExceptionMessage;
		/// <summary>
		/// Ping定时器
		/// </summary>
		private Timer m_PingTimer;
		/// <summary>
		/// Ping时间间隔
		/// </summary>
		private float m_PingTime;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">服务器名</param>
		/// <param name="bufferSize">缓存长度</param>
		/// <param name="pingTime">ping时间间隔</param>
		public NetworkServer(string name, int bufferSize, float pingTime)
		{
			this.name = name;
			m_Buffer = new byte[bufferSize];
			m_BufferIndex = 0;
			m_PingTime = pingTime;
			isConnected = false;
		}

		/// <summary>
		/// 更新回调
		/// </summary>
		public void OnUpdate()
		{
			if (isConnected && !m_Socket.Connected)
			{
				Disconnect();
			}
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		public void OnDestroy()
		{
			Disconnect(true);
		}

		/// <summary>
		/// 注册网络消息
		/// </summary>
		/// <param name="id">消息ID</param>
		/// <param name="name">消息名</param>
		/// <param name="type">消息类型</param>
		public void Register(short id, string name, Type type)
		{
			m_MessageMap[id] = new MessageInfo()
			{
				id = id,
				name = name,
				type = type,
			};
		}

		/// <summary>
		/// 获取网络消息
		/// </summary>
		/// <param name="id">消息ID</param>
		/// <returns>返回网络消息</returns>
		public NetworkMessage GetMessage(short id)
		{
			MessageInfo messageInfo = null;
			if (!m_MessageMap.TryGetValue(id, out messageInfo))
			{
				Debug.Log(string.Format(LOG_GET, name, id));

				return null;
			}

			NetworkMessage networkMessage = Activator.CreateInstance(messageInfo.type) as NetworkMessage;
			networkMessage.Init(this, messageInfo.id, messageInfo.name);

			return networkMessage;
		}

		/// <summary>
		/// 获取所有网络消息
		/// </summary>
		/// <returns>返回消息表</returns>
		public NetworkMessage[] GetAllMessage()
		{
			NetworkMessage[] networkMessages = new NetworkMessage[m_MessageMap.Count];
			int i = 0;
			foreach (short id in m_MessageMap.Keys)
			{
				networkMessages[i++] = GetMessage(id);
			}

			return networkMessages;
		}

		/// <summary>
		/// 连接服务器
		/// </summary>
		/// <param name="ip">服务器IP</param>
		/// <param name="port">服务器端口</param>
		/// <param name="isV6">是否V6网络</param>
		public void Connect(string ip, int port, bool isV6 = false)
		{
			if (isConnected)
			{
				Disconnect(true);
			}

			m_Socket = new Socket(isV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			m_Socket.ReceiveBufferSize = m_Buffer.Length;
			m_Socket.BeginConnect(ip, port, OnAsyncConnect, null);
		}

		/// <summary>
		/// 异步连接回调
		/// </summary>
		/// <param name="asyncResult">异步结果</param>
		private void OnAsyncConnect(IAsyncResult asyncResult)
		{
			try
			{
				m_Socket.EndConnect(asyncResult);

				m_DecryptKey = new byte[s_Key.Length];
				m_BackupKey = new byte[s_Key.Length];
				m_EncryptKey = new byte[s_Key.Length];
				Buffer.BlockCopy(s_Key, 0, m_DecryptKey, 0, s_Key.Length);
				Buffer.BlockCopy(s_Key, 0, m_EncryptKey, 0, s_Key.Length);
				m_ExceptionMessage = string.Empty;

				isConnected = true;
				BeginReceive();

				Core.Invoke((arg) =>
				{
					if (onConnect != null)
					{
						onConnect.Invoke(isConnected, arg.ToString());
					}

					if (m_PingTimer == null)
					{
						m_PingTimer = new Timer(StartPing, m_PingTime, 0, null, true);
					}
					else
					{
						m_PingTimer.Restart();
					}
				}, string.Empty);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				isConnected = false;
				m_ExceptionMessage = e.Message;
				Core.Invoke((arg) =>
				{
					if (onConnect != null)
					{
						onConnect.Invoke(isConnected, arg.ToString());
					}
				}, m_ExceptionMessage);
			}
		}

		/// <summary>
		/// 断开连接
		/// </summary>
		/// <param name="destroy">是否销毁</param>
		public void Disconnect(bool destroy = false)
		{
			try
			{
				isConnected = false;

				if (m_Socket != null)
				{
					if (m_Socket.Connected)
					{
						m_Socket.Shutdown(SocketShutdown.Both);
					}
					m_Socket.Close();
					m_Socket = null;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				m_ExceptionMessage = e.Message;
			}
			finally
			{
				Core.Invoke((arg) =>
				{
					if (m_PingTimer != null)
					{
						m_PingTimer.Stop();
					}

					if (!destroy && onDisconnect != null)
					{
						onDisconnect.Invoke(arg.ToString());
					}
				}, m_ExceptionMessage);
			}
		}

		/// <summary>
		/// 接收消息
		/// </summary>
		/// <param name="networkMessage">网络消息</param>
		public void Receive(NetworkMessage networkMessage)
		{
			if (networkMessage == null)
			{
				return;
			}

			receiveBytes += networkMessage.buffer.length;

			if (onReceive != null)
			{
				onReceive.Invoke(networkMessage);
			}

			if (!EventSystem.HasEvent(networkMessage.id))
			{
				Debug.LogError(string.Format(LOG_RECEIVE, networkMessage));
			}
			else
			{
				EventSystem.Broadcast(networkMessage.id, networkMessage);
			}
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="networkMessage">网络消息</param>
		public void Send(NetworkMessage networkMessage)
		{
			if (networkMessage == null)
			{
				return;
			}

			try
			{
				if (m_Socket == null
					|| !m_Socket.Connected)
				{
					m_ExceptionMessage = string.Format(LOG_DISCONNECT, name);
					Debug.LogError(m_ExceptionMessage);

					return;
				}

				byte[] bytes = Encode(networkMessage);
				m_Socket.Send(bytes);
				sendBytes += bytes.Length;

				if (onSend != null)
				{
					onSend.Invoke(networkMessage);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				m_ExceptionMessage = e.Message;
				Disconnect();
			}
		}

		/// <summary>
		/// 开始接收消息
		/// </summary>
		private void BeginReceive()
		{
			m_Socket.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, OnAsyncReceive, null);
		}

		/// <summary>
		/// 异步接收回调
		/// </summary>
		/// <param name="asyncResult"></param>
		private void OnAsyncReceive(IAsyncResult asyncResult)
		{
			try
			{
				if (m_Socket == null
					|| !m_Socket.Connected)
				{
					m_ExceptionMessage = string.Format(LOG_DISCONNECT, name);

					return;
				}

				//获取接收长度
				SocketError socketError = SocketError.SocketError;
				int size = m_Socket.EndReceive(asyncResult, out socketError);
				if (socketError != SocketError.Success || size <= 0)
				{
					m_ExceptionMessage = string.Format(LOG_DISCONNECT, name);
					Disconnect();

					return;
				}

				//把接收数据加入到缓存
				byte[] buffer = new byte[size];
				Buffer.BlockCopy(m_Buffer, 0, buffer, 0, size);
				Core.Invoke(OnDecodeMessage, buffer);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				m_ExceptionMessage = e.Message;
				Disconnect();
			}
			finally
			{
				BeginReceive();
			}
		}

		private void OnDecodeMessage(object arg)
		{
			byte[] buffer = (byte[])arg;
			m_BufferList.AddRange(buffer);
			buffer = m_BufferList.ToArray();

			//解析数据
			NetworkMessage networkMessage = null;
			DecodeResult result = DecodeResult.Succeed;
			while (result == DecodeResult.Succeed)
			{
				result = Decode(buffer, ref m_BufferIndex, out networkMessage);
				if (networkMessage != null)
				{
					Receive(networkMessage);
				}
			}

			if (result != DecodeResult.LessLength)
			{
				m_BufferIndex = 0;
				m_BufferList.Clear();
			}

			if (result == DecodeResult.Error)
			{
				m_ExceptionMessage = LOG_ERROR;
				Disconnect();
			}
		}

		/// <summary>
		/// 开始测Ping
		/// </summary>
		/// <param name="arg">参数</param>
		private void StartPing(object arg)
		{
			Core.instance.StartCoroutine(PingCoroutine());
		}

		/// <summary>
		/// 测Ping协程
		/// </summary>
		/// <returns></returns>
		private IEnumerator PingCoroutine()
		{
			//Ping ping = new Ping(remoteHost);

			//while (!ping.isDone)
			//{
			//	yield return new WaitForEndOfFrame();
			//}

			//this.ping = ping.time;

			//if (onPing != null)
			//{
			//	onPing.Invoke(ping.time);
			//}
			yield return new WaitForFixedUpdate();
		}

		/// <summary>
		/// 解析数据
		/// </summary>
		/// <param name="bytes">数据</param>
		/// <param name="index">索引值</param>
		/// <param name="networkMessage">网络消息</param>
		/// <returns>返回解析结果</returns>
		private DecodeResult Decode(byte[] bytes, ref int index, out NetworkMessage networkMessage)
		{
			networkMessage = null;

			if (index >= bytes.Length)
			{
				return DecodeResult.End;
			}

			int dataIndex = 0;
			//解密消息头数据
			Buffer.BlockCopy(bytes, index + dataIndex, m_ShortBytes, 0, m_ShortBytes.Length);

			BackupKey();
			byte lastCipher = m_DecryptKey[0];

			lastCipher = Decrypt(dataIndex, lastCipher, m_ShortBytes);
			short head = (short)(((m_ShortBytes[0] & 0xff) << 8) | (m_ShortBytes[1] & 0xff));
			if (head != HEAD)
			{
				//Debug.LogError(string.Format(LOG_HEAD, head, Extension.ToString(m_ShortBytes)));

				return DecodeResult.Error;
			}

			//解密消息ID
			dataIndex += 2;
			Buffer.BlockCopy(bytes, index + dataIndex, m_ShortBytes, 0, m_ShortBytes.Length);
			lastCipher = Decrypt(dataIndex, lastCipher, m_ShortBytes);
			short codeId = (short)(((m_ShortBytes[0] & 0xff) << 8) | (m_ShortBytes[1] & 0xff));

			//解密长度
			dataIndex += 2;
			Buffer.BlockCopy(bytes, index + dataIndex, m_IntBytes, 0, m_IntBytes.Length);
			lastCipher = Decrypt(dataIndex, lastCipher, m_IntBytes);
			int length = (((m_IntBytes[0] & 0x000000ff) << 24) | ((m_IntBytes[1] & 0x000000ff) << 16) | ((m_IntBytes[2] & 0x000000ff) << 8) | (m_IntBytes[3] & 0x000000ff));
			if (codeId <= 0 || length < 0)
			{
				//尽量不丢包
				++index;
				RevertKey();

				return DecodeResult.LessLength;
			}
			else if (length + index > bytes.Length)
			{
				RevertKey();

				return DecodeResult.LessLength;
			}

			//解密数据
			byte[] data = null;
			if (length > 0)
			{
				data = new byte[length];
				dataIndex += 4;
				Buffer.BlockCopy(bytes, index + dataIndex, data, 0, data.Length);
				Decrypt(dataIndex, lastCipher, data);
			}

			index += dataIndex + length;

			networkMessage = GetMessage(codeId);
			if (networkMessage == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, codeId));

				return DecodeResult.Succeed;
			}

			try
			{
				networkMessage.Decode(data);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			return DecodeResult.Succeed;
		}

		/// <summary>
		/// 编码数据
		/// </summary>
		/// <param name="networkMessage">网络消息</param>
		/// <returns>返回数据</returns>
		private byte[] Encode(NetworkMessage networkMessage)
		{
			byte[] data = networkMessage.Encode();
			byte[] bytes = new byte[HEAD_SIZE + data.Length];

			ReverseCopy(BitConverter.GetBytes(HEAD), 0, bytes, 0, 2);
			ReverseCopy(BitConverter.GetBytes(networkMessage.id), 0, bytes, 2, 2);
			ReverseCopy(BitConverter.GetBytes(data.Length), 0, bytes, 4, 4);
			Buffer.BlockCopy(data, 0, bytes, HEAD_SIZE, data.Length);

			Encrypt(0, m_EncryptKey[0], bytes);

			return bytes;
		}

		/// <summary>
		/// 解密数据
		/// </summary>
		/// <param name="beginIndex">开始索引值</param>
		/// <param name="lastCipher">上一个密码</param>
		/// <param name="bytes">数据</param>
		/// <returns>返回数据</returns>
		private byte Decrypt(int beginIndex, byte lastCipher, byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				int keyIndex = beginIndex & 7;
				byte tempCipher = bytes[i];
				m_DecryptKey[keyIndex] = (byte)(((~(m_DecryptKey[keyIndex] - lastCipher)) ^ beginIndex) & 0xff);
				bytes[i] = (byte)(~((bytes[i] - lastCipher) ^ m_DecryptKey[keyIndex]) & 0xff);
				lastCipher = tempCipher;
				beginIndex++;
			}

			return lastCipher;
		}

		/// <summary>
		/// 加密数据
		/// </summary>
		/// <param name="beginIndex">开始索引值</param>
		/// <param name="lastCipher">上一个密码</param>
		/// <param name="bytes">数据</param>
		/// <returns>返回缓存</returns>
		private int Encrypt(int beginIndex, byte lastCipher, byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				int keyIndex = beginIndex & 7;
				m_EncryptKey[keyIndex] = (byte)(((~(m_EncryptKey[keyIndex] - lastCipher)) ^ beginIndex) & 0xff);
				byte b = (byte)(((~bytes[i]) ^ m_EncryptKey[keyIndex]) + lastCipher);
				bytes[i] = b;
				lastCipher = b;
				beginIndex++;
			}

			return beginIndex;
		}

		/// <summary>
		/// 备份钥匙
		/// </summary>
		private void BackupKey()
		{
			Buffer.BlockCopy(m_DecryptKey, 0, m_BackupKey, 0, m_BackupKey.Length);
		}

		/// <summary>
		/// 还原钥匙
		/// </summary>
		private void RevertKey()
		{
			Buffer.BlockCopy(m_BackupKey, 0, m_DecryptKey, 0, m_DecryptKey.Length);
		}

		/// <summary>
		/// 反向拷贝
		/// </summary>
		/// <param name="src">源数据</param>
		/// <param name="srcOffset">源偏移值</param>
		/// <param name="dst">目标数据</param>
		/// <param name="dstOffset">目标偏移值</param>
		/// <param name="count">拷贝数量</param>
		private void ReverseCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
		{
			Array.Reverse(src);
			Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
		}

		/// <summary>
		/// 解码结果
		/// </summary>
		private enum DecodeResult
		{
			End,			//结束
			Error,			//错误
			LessLength,		//少于长度
			Succeed,		//成功
		}

		/// <summary>
		/// 协议信息
		/// </summary>
		private class MessageInfo
		{
			public short id;	//Id
			public string name;	//名字
			public Type type;	//类型
		}
	}
}