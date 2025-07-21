using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
    public sealed class ObjectPool
    {
        private const string LOG_NULL = "Construct callback is null.";

        public ushort constructStep = 1;

        private Func<object, object> m_OnConstructSync;
        private Func<object, Task<object>> m_OnConstructAsync;

        private Action<object, object> m_OnDestroy;
        private Action<object, object> m_OnEnabled;
        private Action<object, object> m_OnDisabled;

        private List<object> m_EnabledList = new List<object>();
        private Queue<object> m_DisabledPool = new Queue<object>();

        ///// <summary>
        ///// 同步构造池
        ///// </summary>
        //public ObjectPool(Func<object, object> onConstruct, Action<object, object> onDestroy = null, Action<object, object> onEnabled = null, Action<object, object> onDisabled = null)
        //{
        //    if (onConstruct == null)
        //        Debug.LogError(LOG_NULL);

        //    m_OnConstructSync = onConstruct;
        //    m_OnDestroy = onDestroy;
        //    m_OnEnabled = onEnabled;
        //    m_OnDisabled = onDisabled;
        //}

        /// <summary>
        /// 异步构造池
        /// </summary>
        public ObjectPool(Func<object, Task<object>> onConstructAsync, Action<object, object> onDestroy = null, Action<object, object> onEnabled = null, Action<object, object> onDisabled = null)
        {
            if (onConstructAsync == null)
                Debug.LogError(LOG_NULL);

            m_OnConstructAsync = onConstructAsync;
            m_OnDestroy = onDestroy;
            m_OnEnabled = onEnabled;
            m_OnDisabled = onDisabled;
        }

        private object AddSync(object o)
        {
            for (int i = 0; i < constructStep - 1; ++i)
            {
                object obj = m_OnConstructSync.Invoke(o);
                m_DisabledPool.Enqueue(obj);
            }
            return m_OnConstructSync.Invoke(o);
        }

        private async Task<object> AddAsync(object o)
        {
            for (int i = 0; i < constructStep - 1; ++i)
            {
                object obj = await m_OnConstructAsync.Invoke(o);
                m_DisabledPool.Enqueue(obj);
            }
            return await m_OnConstructAsync.Invoke(o);
        }

        //public object Get(object o = null)
        //{
        //    if (m_OnConstructSync == null)
        //    {
        //        Debug.LogError("同步构造未初始化，不能使用 Get()");
        //        return null;
        //    }

        //    object obj = m_DisabledPool.Count > 0 ? m_DisabledPool.Dequeue() : AddSync(o);
        //    m_EnabledList.Add(obj);
        //    m_OnEnabled?.Invoke(obj, o);
        //    return obj;
        //}

        public async Task<object> GetAsync(object o = null)
        {
            if (m_OnConstructAsync == null)
            {
                Debug.LogError("异步构造未初始化，不能使用 GetAsync()");
                return null;
            }

            object obj = m_DisabledPool.Count > 0 ? m_DisabledPool.Dequeue() : await AddAsync(o);
            m_EnabledList.Add(obj);
            m_OnEnabled?.Invoke(obj, o);
            return obj;
        }

        public object[] GetAll()
        {
            return m_EnabledList.ToArray();
        }

        public void Remove(object obj, object o = null)
        {
            if (!m_EnabledList.Contains(obj)) return;

            m_EnabledList.Remove(obj);
            m_DisabledPool.Enqueue(obj);
            m_OnDisabled?.Invoke(obj, o);
        }

        public void Clear(bool destroy = true, object o = null)
        {
            if (destroy)
            {
                if (m_OnDestroy != null)
                {
                    foreach (var obj in m_EnabledList)
                        m_OnDestroy.Invoke(obj, o);
                    while (m_DisabledPool.Count > 0)
                        m_OnDestroy.Invoke(m_DisabledPool.Dequeue(), o);
                }
                m_EnabledList.Clear();
                m_DisabledPool.Clear();
            }
            else
            {
                foreach (var obj in m_EnabledList)
                {
                    m_DisabledPool.Enqueue(obj);
                    m_OnDisabled?.Invoke(obj, o);
                }
                m_EnabledList.Clear();
            }
        }
    }
}
