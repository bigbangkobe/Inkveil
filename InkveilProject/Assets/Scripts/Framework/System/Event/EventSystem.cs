using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 事件系统
	/// </summary>
	public sealed class EventSystem : Singleton<EventSystem>
	{
		private const string LOG_CALLBACK_NULL = "Event callback {0} is null.";
		private const string LOG_GET = "Can't find out event callback:{0}.";

		/// <summary>
		/// 派发事件
		/// </summary>
		public static event Action<int, object> onBroadcastEvent;

		/// <summary>
		/// 事件回调表 [键:事件ID 值:事件对象]
		/// </summary>
		private Dictionary<int, EventObject> m_CallbackMap = new Dictionary<int, EventObject>();

		/// <summary>
		/// 派发事件
		/// </summary>
		/// <param name="id">事件ID</param>
		/// <param name="msg">事件消息</param>
		public static void Broadcast(int id, object msg = null)
		{
			if (onBroadcastEvent != null)
			{
				onBroadcastEvent.Invoke(id, msg);
			}

			EventObject eventObject = null;
			if (!instance.m_CallbackMap.TryGetValue(id, out eventObject))
			{
				Debug.Log(string.Format(LOG_GET, id));

				return;
			}

			if (eventObject.isCache)
			{
				eventObject.cacheQueue.Enqueue(msg);
			}
			else if (eventObject.callback != null)
			{
				eventObject.callback.Invoke(msg);
			}
		}

		/// <summary>
		/// 是否含有事件
		/// </summary>
		/// <param name="id">事件ID</param>
		/// <returns>返回结果</returns>
		public static bool HasEvent(int id)
		{
			return instance.m_CallbackMap.ContainsKey(id);
		}

		/// <summary>
		/// 注册事件
		/// </summary>
		/// <param name="id">事件ID</param>
		/// <param name="onCallback">事件回调</param>
		public static void Register(int id, Action<object> onCallback)
		{
			if (onCallback == null)
			{
				Debug.LogError(string.Format(LOG_CALLBACK_NULL, id));

				return;
			}

			EventObject eventObject = null;
			if (!instance.m_CallbackMap.TryGetValue(id, out eventObject))
			{
				eventObject = new EventObject();
				eventObject.callback = onCallback;
				instance.m_CallbackMap[id] = eventObject;
			}
			else
			{
				eventObject.callback -= onCallback;
				eventObject.callback += onCallback;
			}
		}

		/// <summary>
		/// 注销事件
		/// </summary>
		/// <param name="id">事件ID</param>
		/// <param name="onCallback">事件回调</param>
		public static void Cancel(int id, Action<object> onCallback)
		{
			if (onCallback == null)
			{
				Debug.LogError(string.Format(LOG_CALLBACK_NULL, id));

				return;
			}

			EventObject eventObject = null;
			if (instance.m_CallbackMap.TryGetValue(id, out eventObject))
			{
				eventObject.callback -= onCallback;
			}
		}

		/// <summary>
		/// 清理事件
		/// </summary>
		/// <param name="id">事件ID</param>
		public static void Clear(int id)
		{
			EventObject eventObject = null;
			if (instance.m_CallbackMap.TryGetValue(id, out eventObject))
			{
				eventObject.callback = null;
			}
		}

		/// <summary>
		/// 设置缓存消息
		/// </summary>
		/// <param name="id">事件ID</param>
		/// <param name="isCache">是否缓存</param>
		public static void SetCache(int id, bool isCache)
		{
			EventObject eventObject = null;
			if (!instance.m_CallbackMap.TryGetValue(id, out eventObject))
			{
				eventObject = new EventObject();
				instance.m_CallbackMap[id] = eventObject;
			}

			eventObject.isCache = isCache;
			if (!isCache)
			{
				if (eventObject.callback == null)
				{
					eventObject.cacheQueue.Clear();
				}
				else
				{
					while (eventObject.cacheQueue.Count > 0)
					{
						object msg = eventObject.cacheQueue.Dequeue();
						eventObject.callback.Invoke(msg);
					}
				}
			}
		}

		/// <summary>
		/// 事件对象
		/// </summary>
		private class EventObject
		{
			/// <summary>
			/// 事件回调
			/// </summary>
			public Action<object> callback;
			/// <summary>
			/// 是否缓存
			/// </summary>
			public bool isCache = false;
			/// <summary>
			/// 缓存队列
			/// </summary>
			public Queue<object> cacheQueue = new Queue<object>();
		}
	}
}