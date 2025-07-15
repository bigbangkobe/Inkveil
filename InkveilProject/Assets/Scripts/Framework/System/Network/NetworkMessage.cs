namespace Framework
{
	/// <summary>
	/// 网络消息
	/// </summary>
	public abstract class NetworkMessage
	{
		protected const string FORMAT_NAME = "{0} ({1})\t{2}";

		/// <summary>
		/// 消息ID
		/// </summary>
		public short id { get; private set; }
		/// <summary>
		/// 消息名
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// 网络缓存
		/// </summary>
		public NetworkBuffer buffer { get; private set; }
		/// <summary>
		/// 服务器
		/// </summary>
		public NetworkServer server { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="server">服务器</param>
		/// <param name="id">消息ID</param>
		/// <param name="name">消息名</param>
		public void Init(NetworkServer server, short id, string name)
		{
			this.server = server;
			this.id = id;
			this.name = name;
			buffer = new NetworkBuffer();

			OnInit();
		}

		/// <summary>
		/// 初始化回调
		/// </summary>
		public virtual void OnInit()
		{
		}

		/// <summary>
		/// 解析数据
		/// </summary>
		/// <param name="buff">消息数据</param>
		public void Decode(byte[] buff)
		{
			buffer.Init(buff);
			OnDecode();
		}

		/// <summary>
		/// 解析数据回调
		/// </summary>
		public virtual void OnDecode()
		{
		}

		/// <summary>
		/// 封装数据
		/// </summary>
		/// <returns>返回消息数据</returns>
		public byte[] Encode()
		{
			OnEncode();

			return buffer.bytes;
		}

		/// <summary>
		/// 封装数据回调
		/// </summary>
		public virtual void OnEncode()
		{
		}

		/// <summary>
		/// 转换字符串
		/// </summary>
		/// <returns>返回字符串</returns>
		public override string ToString()
		{
			return string.Format(FORMAT_NAME, id, name ?? "Null", buffer);
		}
	}
}