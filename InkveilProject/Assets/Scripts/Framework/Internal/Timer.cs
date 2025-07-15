using System;
using System.Timers;

namespace Framework
{
	/// <summary>
	/// 定时器
	/// </summary>
	public sealed class Timer
	{
		private const float MIN_TIME = 0.0001f;

		/// <summary>
		/// 停止事件
		/// </summary>
		public event Action onStopEvent;

		/// <summary>
		/// 重复次数
		/// </summary>
		public int count { get; private set; }

		/// <summary>
		/// 系统定时器
		/// </summary>
		private System.Timers.Timer m_Timer;
		/// <summary>
		/// 定时回调
		/// </summary>
		private Action<object> m_Callback;
		/// <summary>
		/// 回调参数
		/// </summary>
		private object m_Arg;
		/// <summary>
		/// 重复次数
		/// </summary>
		private int m_Count;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="timerCallback">定时器回调</param>
		/// <param name="interval">间隔时间</param>
		/// <param name="count">重复次数</param>
		/// <param name="arg">回调参数</param>
		public Timer(Action<object> timerCallback, float interval = 0, int count = 1, object arg = null, bool invoke = false)
		{
			m_Callback = timerCallback;
			m_Arg = arg;
			m_Count = count;
			this.count = count;

			if (invoke)
			{
				m_Callback.Invoke(m_Arg);
			}

			m_Timer = new System.Timers.Timer();
			m_Timer.Elapsed += OnElapsed;
			m_Timer.Interval = Math.Max(interval, MIN_TIME) * 1000;
			m_Timer.AutoReset = true;
			m_Timer.Enabled = true;
		}

		/// <summary>
		/// 重新开始
		/// </summary>
		/// <param name="interval">间隔时间</param>
		/// <param name="count">重复次数</param>
		public void Restart(float interval = -1, int count = -1, bool invoke = false)
		{
			if (interval >= 0)
			{
				m_Timer.Interval = Math.Max(interval, MIN_TIME) * 1000;
			}

			if (count < 0)
			{
				this.count = m_Count;
			}
			else
			{
				m_Count = count;
				this.count = count;
			}

			if (invoke)
			{
				m_Callback.Invoke(m_Arg);
			}

			m_Timer.Enabled = false;
			m_Timer.Enabled = true;
		}

		/// <summary>
		/// 停止定时器
		/// </summary>
		/// <param name="invoke">是否调用</param>
		public void Stop(bool invoke = false)
		{
			if (invoke && m_Callback != null)
			{
				m_Callback.Invoke(m_Arg);
			}

			if (onStopEvent != null)
			{
				onStopEvent.Invoke();
			}

			m_Timer.Enabled = false;
		}

		/// <summary>
		/// 定时器回调
		/// </summary>
		/// <param name="state">参数</param>
		private void OnElapsed(object sender, ElapsedEventArgs eventArgs)
		{
			if (!Core.Invoke(OnTimerExcute)
				|| (count > 0 && --count <= 0))
			{
				Stop();
			}
		}

		/// <summary>
		/// 定时器调用
		/// </summary>
		/// <param name="arg">参数</param>
		private void OnTimerExcute(object arg)
		{
			m_Callback.Invoke(m_Arg);
		}
	}
}