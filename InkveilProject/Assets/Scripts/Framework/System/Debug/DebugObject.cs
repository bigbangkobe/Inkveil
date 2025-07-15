namespace Framework
{
	/// <summary>
	/// 调试对象
	/// </summary>
	public abstract class DebugObject
	{
		/// <summary>
		/// 调试名
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// 调试标题
		/// </summary>
		public virtual string title { get { return name; } }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="name">调试名</param>
		public void Init(string name)
		{
			this.name = name;

			OnInit();
		}

		/// <summary>
		/// 初始化回调
		/// </summary>
		public virtual void OnInit()
		{
		}

		/// <summary>
		/// 渲染回调
		/// </summary>
		public virtual void OnGUI()
		{
		}

		/// <summary>
		/// 更新回调
		/// </summary>
		public virtual void OnUpdate()
		{
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		public virtual void OnDestroy()
		{
		}
	}
}