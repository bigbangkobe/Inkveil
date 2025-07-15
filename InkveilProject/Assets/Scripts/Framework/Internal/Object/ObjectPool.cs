using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 对象池
	/// </summary>
	public sealed class ObjectPool
	{
		private const string LOG_NULL = "Construct callback is null.";

		/// <summary>
		/// 构造步长
		/// </summary>
		public ushort constructStep = 1;

		/// <summary>
		/// 对象构造回调,参数为obect，有返回值为object
		/// </summary>
		/// <returns>返回构造对象</returns>
		private Func<object,object> m_OnConstruct;
		/// <summary>
		/// 销毁回调
		/// </summary>
		private Action<object, object> m_OnDestroy;
		/// <summary>
		/// 启用回调
		/// </summary>
		private Action<object, object> m_OnEnabled;
		/// <summary>
		/// 关闭回调
		/// </summary>
		private Action<object, object> m_OnDisabled;
		/// <summary>
		/// 启用链表
		/// </summary>
		private List<object> m_EnabledList = new List<object>();
		/// <summary>
		/// 关闭池
		/// </summary>
		private Queue<object> m_DisabledPool = new Queue<object>();

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="onContruct">对象构造回调</param>
		/// <param name="onDestroy">对象销毁回调</param>
		/// <param name="onEnabled">对象启用回调</param>
		/// <param name="onDisabled">对象关闭回调</param>
		public ObjectPool(Func<object,object> onContruct, Action<object, object> onDestroy = null, Action<object, object> onEnabled = null, Action<object, object> onDisabled = null)
		{
			if (onContruct == null)
			{
				Debug.LogError(LOG_NULL);
			}

			m_OnConstruct = onContruct;
			m_OnDestroy = onDestroy;
			m_OnEnabled = onEnabled;
			m_OnDisabled = onDisabled;
		}

		/// <summary>
		/// 添加对象
		/// </summary>
		private object Add(object o)
		{
			for (int i = 0; i < constructStep - 1; ++i)
			{
				object obj = m_OnConstruct.Invoke(o);
				m_DisabledPool.Enqueue(obj);
			}

			return m_OnConstruct.Invoke(o);
		}

		/// <summary>
		/// 获取对象
		/// </summary>
		/// <returns>返回对象</returns>
		public object Get(object o = null)
		{
			object obj = m_DisabledPool.Count > 0 ? m_DisabledPool.Dequeue() : Add(o);
			m_EnabledList.Add(obj);
            //Debug.Log("获取对象" + obj.ToString());

            if (m_OnEnabled != null)
			{
                m_OnEnabled.Invoke(obj, o);
			}

			return obj;
		}

		/// <summary>
		/// 获取所有对象
		/// </summary>
		/// <returns>返回所有对象</returns>
		public object[] GetAll()
		{
			return m_EnabledList.ToArray();
		}

		/// <summary>
		/// 回收对象
		/// </summary>
		/// <param name="obj">对象</param>
		public void Remove(object obj, object o = null)
		{
			if (!m_EnabledList.Contains(obj))
			{
				return;
			}
            //Debug.Log("回收对象" + obj.ToString());

            m_EnabledList.Remove(obj);
			m_DisabledPool.Enqueue(obj);

			if (m_OnDisabled != null)
			{
                m_OnDisabled.Invoke(obj, o);
			}
		}

		/// <summary>
		/// 清理对象
		/// </summary>
		/// <param name="destroy">是否销毁</param>
		public void Clear(bool destroy = true, object o = null)
		{
			if (destroy)
			{
				if (m_OnDestroy != null)
				{
					for (int i = 0; i < m_EnabledList.Count; ++i)
					{
						object obj = m_EnabledList[i];
                        m_OnDestroy.Invoke(obj, o);
					}

					while (m_DisabledPool.Count > 0)
					{
                        m_OnDestroy.Invoke(m_DisabledPool.Dequeue(), o);
					}
				}

				m_EnabledList.Clear();
				m_DisabledPool.Clear();
			}
			else
			{
				for (int i = 0; i < m_EnabledList.Count; ++i)
				{
					object obj = m_EnabledList[i];
					m_DisabledPool.Enqueue(obj);

					if (m_OnDisabled != null)
					{
						m_OnDisabled.Invoke(obj, o);
					}
				}
				m_EnabledList.Clear();
			}
		}
	}
}