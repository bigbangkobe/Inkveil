using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 状态机
	/// </summary>
	public abstract class StateMachine
	{
		/// <summary>
		/// 状态机开关
		/// </summary>
		public bool enabled { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public StateMachine()
		{
			enabled = true;
		}

		/// <summary>
		/// 更新接口
		/// </summary>
		public virtual void OnUpdate()
		{
		}

		/// <summary>
		/// 销毁接口
		/// </summary>
		public virtual void OnDestroy()
		{
		}
	}
}