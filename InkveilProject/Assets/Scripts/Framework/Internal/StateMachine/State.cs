namespace Framework
{
	/// <summary>
	/// 状态基类
	/// </summary>
	public abstract class State
	{
		/// <summary>
		/// 状态机
		/// </summary>
		public StateMachine stateMachine { get; private set; }
		/// <summary>
		/// 状态名
		/// </summary>
		public string name { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public void Init(StateMachine stateMachine, string name)
		{
			this.stateMachine = stateMachine;
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
		/// 进入回调
		/// </summary>
		public virtual void OnEnter()
		{
		}

		/// <summary>
		/// 离开回调
		/// </summary>
		public virtual void OnExit()
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

		/// <summary>
		/// 切换接口
		/// </summary>
		/// <param name="nextState">下一个状态</param>
		/// <returns>返回切换结果</returns>
		public virtual bool OnCondition(string nextState)
		{
			return nextState != name;
		}
	}
}