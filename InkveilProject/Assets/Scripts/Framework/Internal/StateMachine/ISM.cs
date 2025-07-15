using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 无限状态机
	/// </summary>
	public sealed class ISM : StateMachine
	{
		private const string LOG_STATE = "Can't find out state:{0}.";

		/// <summary>
		/// 状态表 [键:状态名 值:状态接口]
		/// </summary>
		private Dictionary<string, State> m_StateMap = new Dictionary<string, State>();
		/// <summary>
		/// 状态列表
		/// </summary>
		private List<State> m_StateList = new List<State>();

		/// <summary>
		/// 构造函数
		/// </summary>
		public ISM()
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

			for (int i = 0; i < m_StateList.Count; ++i)
			{
				m_StateList[i].OnUpdate();
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
				m_StateList.Add(state);

				state.Init(this, name);
				state.OnEnter();
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
				Debug.LogError(string.Format(LOG_STATE, name));

				return;
			}

			state.OnDestroy();
			m_StateMap.Remove(name);
			m_StateList.Remove(state);
		}

		/// <summary>
		/// 清理状态机
		/// </summary>
		public void Clear()
		{
			m_StateMap.Clear();

			for (int i = 0; i < m_StateList.Count; ++i)
			{
				m_StateList[i].OnDestroy();
			}
			m_StateList.Clear();
		}
	}
}