using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 定时器系统
    /// </summary>
    public sealed class TimerSystem : MonoSingleton<TimerSystem>
    {
        /// <summary>
        /// 输出定时器回调为空错误文本
        /// </summary>
        private const string LOG_CALLBACK_NULL = "Timer callback is null.";

        /// <summary>
        /// 输出定时器对象为空错误文本
        /// </summary>
        private const string LOG_TIMER_NULL = "Timer object is null.";

        private ObjectPool m_TimerPool = new ObjectPool(OnTimerConstruct, OnTimerDestroy, OnTimerEnabled, OnTimerDisabled);

        /// <summary>
        /// 摧毁回调
        /// </summary>
        private void OnDestroy()
        {
            Clear();
        }

        public static async Task<TimerObject> Start(Action<object> timerCallback,bool pause, float time, float interval = 0, int count = 0, object arg = null)
        {
            if (timerCallback == null)
            {
                //定时器回调为空
                Debug.LogError(LOG_CALLBACK_NULL);
                return null;
            }

            //启动定时器
            TimerObject timerObject = await instance.m_TimerPool.GetAsync() as TimerObject;
            timerObject.StartTimer(timerCallback, pause, time, interval, count, arg);

            return timerObject;
        }

        /// <summary>
        /// 清理定时器
        /// </summary>
        public static void Clear()
        {
            //将定时器从对象池中删除
            instance.m_TimerPool.Clear();
        }

        /// <summary>
        /// 定时器构造回调函数
        /// </summary>
        /// <returns>定时器对象</returns>
        private static Task<object> OnTimerConstruct(object obj)
        {
            TimerObject timerObject = instance.gameObject.AddComponent<TimerObject>();
            return Task.FromResult<object>(timerObject);
        }


        /// <summary>
        /// 定时器摧毁回调
        /// </summary>
        /// <param name="obj">定时器对象</param>
        private static void OnTimerDestroy(object obj, object o = null)
        {
            TimerObject timerObject = obj as TimerObject;
            timerObject.Stop(false);
        }

        /// <summary>
        /// 定时器开启回调
        /// </summary>
        /// <param name="obj">对象</param>
        private static void OnTimerEnabled(object obj, object o = null)
        {
            TimerObject timerObject = obj as TimerObject;
            //绑定定时器停止事件
            timerObject.onStopEvent += OnTimerStop;
            timerObject.enabled = true;
        }

        /// <summary>
        /// 定时器关闭回调函数
        /// </summary>
        /// <param name="obj">定时器对象</param>
        private static void OnTimerDisabled(object obj, object o = null)
        {
            TimerObject timerObject = obj as TimerObject;
            timerObject.enabled = false;
        }

        /// <summary>
        /// 定时器停止回调
        /// </summary>
        /// <param name="timerObject">定时器对象</param>
        private static void OnTimerStop(TimerObject timerObject)
        {
            instance.m_TimerPool.Remove(timerObject);
        }
    }
}
