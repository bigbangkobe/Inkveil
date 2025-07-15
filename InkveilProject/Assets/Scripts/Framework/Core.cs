using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 框架核心
	/// </summary>
	public sealed class Core : MonoSingleton<Core>
	{
		/// <summary>
		/// 调用队列
		/// </summary>
		private static Queue<InvokeObject> m_InvokeQueue;

		protected override void Awake()
		{
			base.Awake();

			gameObject.hideFlags = HideFlags.NotEditable;
			DontDestroyOnLoad(gameObject);

			m_InvokeQueue = new Queue<InvokeObject>();
		}

		private void OnDestroy()
		{
			m_InvokeQueue = null;
		}

		private void Update()
		{
			while (m_InvokeQueue.Count > 0)
			{
				lock (m_InvokeQueue)
				{
					InvokeObject excuteObject = m_InvokeQueue.Dequeue();
					excuteObject.callback.Invoke(excuteObject.arg);
				}
			}
		}

		/// <summary>
		/// 子线程调用
		/// </summary>
		/// <param name="callback">回调函数</param>
		/// <param name="arg">回调参数</param>
		/// <returns>返回调用结果</returns>
		public static bool Invoke(Action<object> callback, object arg = null)
		{
			if (m_InvokeQueue == null)
			{
				return false;
			}

			lock (m_InvokeQueue)
			{
				InvokeObject excuteObject = new InvokeObject();
				excuteObject.callback = callback;
				excuteObject.arg = arg;
				m_InvokeQueue.Enqueue(excuteObject);
			}

			return true;
		}

		/// <summary>
		/// 调用对象
		/// </summary>
		private class InvokeObject
		{
			/// <summary>
			/// 调用回调
			/// </summary>
			public Action<object> callback;
			/// <summary>
			/// 调用参数
			/// </summary>
			public object arg;
		}
	}
}