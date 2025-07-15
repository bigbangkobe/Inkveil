using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 时间定时器对象
    /// </summary>
    public sealed class TimerObject : MonoBehaviour
    {
        /// <summary>
        /// 定义定时器回调函数
        /// </summary>
        private const string FUNTION = "OnTimerCallback";

        /// <summary>
        /// 停止事件
        /// </summary>
        public Action<TimerObject> onStopEvent;

        /// <summary>
        /// 定时器回调事件
        /// </summary>
        public Action<object> timerCallback { get; private set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        public object arg { get; private set; }

        /// <summary>
        /// 定时时间
        /// </summary>
        public float time { get; private set; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public float interval { get; private set; }

        /// <summary>
        /// 重复次数
        /// </summary>
        public int count { get; private set; }

        /// <summary>
        /// 开始定时器
        /// </summary>
        /// <param name="timerCallback">定时器回调函数</param>
        /// <param name="time">定时时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="count">重复次数</param>
        /// <param name="arg">回调参数</param>
        public void StartTimer(Action<object> timerCallback, bool pause, float time, float interval = 0, int count = 0, object arg = null)
        {
            this.timerCallback = timerCallback;
            this.arg = arg;
            this.time = time;
            this.interval = interval;
            this.count = count;

            //初始是否暂停
            if (pause)
                return;

            if (interval > 0)
            {
                //使用反射回调来进行定时器调用
                InvokeRepeating(FUNTION, time, interval);
            }
            else
            {
                Invoke(FUNTION, time);
            }
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        /// <param name="time">定时时间</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="count">重复次数</param>
        public void Restart(float time = -1, float interval = -1, int count = -1)
        {
            this.time = time < 0 ? this.time : time;
            this.interval = interval < 0 ? this.interval : interval;
            this.count = count < 0 ? this.count : count;

            CancelInvoke(FUNTION);

            if (this.interval > 0)
            {
                InvokeRepeating(FUNTION, this.time, this.interval);
            }
            else
            {
                Invoke(FUNTION, this.time);
            }
        }

        /// <summary>
        /// 暂停定时器
        /// </summary>
        public void Pause()
        {
            CancelInvoke(FUNTION);
        }

        /// <summary>
        /// 完成定时器
        /// </summary>
        public void Complete()
        {
            CancelInvoke(FUNTION);
            timerCallback.Invoke(arg);
        }

        /// <summary>
        /// 停止定时器
        /// </summary>
        /// <param name="destroy">是否销毁</param>
        public void Stop(bool destroy = false)
        {
            timerCallback = null;
            arg = null;

            CancelInvoke(FUNTION);

            if (destroy)
            {
                Destroy(this);
            }
            else if (onStopEvent != null)
            {
                onStopEvent.Invoke(this);
                onStopEvent = null;
            }
        }

        /// <summary>
        /// 定时器回调
        /// </summary>
        private void OnTimerCallback()
        {
            timerCallback.Invoke(arg);
            if (/*(interval <= 0) && */count > 0 && --count <= 0)
            {
                Stop();
            }
        }
    }
}