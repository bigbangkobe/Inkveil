using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 有限状态机
	/// </summary>
	public sealed class FSM : StateMachine
	{
		private const string LOG_GET = "Can't find out state:{0}.";

		/// <summary>
		/// 当前状态
		/// </summary>
		public State state { get; private set; }
		/// <summary>
		/// 上一个状态
		/// </summary>
		public State lastState { get; private set; }

		/// <summary>
		/// 状态表 [键:状态名 值:状态接口]
		/// </summary>
		private Dictionary<string, State> m_StateMap = new Dictionary<string, State>();

		/// <summary>
		/// 构造函数
		/// </summary>
		public FSM()
		{
			enabled = true;
		}

		/// <summary>
		/// 更新接口
		/// </summary>
		public override void OnUpdate()
		{
			base.OnUpdate();

			if (!enabled)
			{
				return;
			}

			if (state != null)
			{
				state.OnUpdate();
			}
		}

		/// <summary>
		/// 销毁接口
		/// </summary>
		public override void OnDestroy()
		{
			base.OnDestroy();

			Clear();
		}

		/// <summary>
		/// 初始化状态机
		/// </summary>
		/// <param name="name">状态名</param>
		public void Init(string name)
		{
			State state = null;
			if (!m_StateMap.TryGetValue(name, out state))
			{
				Debug.LogError(string.Format(LOG_GET, name));

				return;
			}

			lastState = null;
			this.state = state;
			this.state.OnEnter();
		}

		/// <summary>
		/// 进入状态
		/// </summary>
		/// <param name="name">状态名</param>
		/// <returns>返回进入结果</returns>
		public bool Enter(string name)
		{
			State nextState = null;
			if (!m_StateMap.TryGetValue(name, out nextState))
			{
				return false;
			}

			if (state != null)
			{
				if (!state.OnCondition(name))
				{
					return false;
				}

				state.OnExit();
			}

			lastState = state;
			state = nextState;
			nextState.OnEnter();

			return true;
		}

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="name">状态名</param>
		/// <param name="type">状态类型</param>
		/// <returns>返回状态对象</returns>
		public State Add(string name, Type type)
		{
			State state = null;
			if (!m_StateMap.TryGetValue(name, out state))
			{
				state = Activator.CreateInstance(type) as State;
				m_StateMap[name] = state;

				state.Init(this, name);
			}

			return state;
		}

		/// <summary>
		/// 获取状态
		/// </summary>
		/// <param name="name">状态名</param>
		/// <returns>返回状态对象</returns>
		public State Get(string name)
		{
			State state = null;
			m_StateMap.TryGetValue(name, out state);

			return state;
		}

		/// <summary>
		/// 移除状态
		/// </summary>
		/// <param name="name">状态名</param>
		public void Remove(string name)
		{
			State state = null;
			if (!m_StateMap.TryGetValue(name, out state))
			{
				Debug.LogError(string.Format(LOG_GET, name));

				return;
			}

			state.OnDestroy();
			m_StateMap.Remove(name);
		}

		/// <summary>
		/// 清理状态机
		/// </summary>
		public void Clear()
		{
			foreach (State state in m_StateMap.Values)
			{
				state.OnDestroy();
			}
			m_StateMap.Clear();
		}
	}
}