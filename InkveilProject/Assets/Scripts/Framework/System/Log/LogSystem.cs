using UnityEngine;
using System;
using System.IO;

namespace Framework
{
	/// <summary>
	/// 日志系统
	/// </summary>
	public sealed class LogSystem : Singleton<LogSystem>
	{
		private const string LOG_NULL = "Log path is null or empty, please run the init first.";
		/// <summary>
		/// 控制台格式
		/// </summary>
		private const string FORMAT_CONSOLE = "[{0}] {1}";

		/// <summary>
		/// 日志路径
		/// </summary>
		public static string path { get; private set; }
		/// <summary>
		/// 是否接收日志
		/// </summary>
		public static bool receivedLog
		{
			get { return s_EnabledConsoleLog; }
			set
			{
				if (value == s_EnabledConsoleLog)
				{
					return;
				}

				s_EnabledConsoleLog = value;
				if (value)
				{
					Application.logMessageReceivedThreaded += OnReceivedLog;
				}
				else
				{
					Application.logMessageReceivedThreaded -= OnReceivedLog;
				}
			}
		}
		private static bool s_EnabledConsoleLog = false;

		/// <summary>
		/// 日志回调
		/// </summary>
		private Func<object, string> m_OnLogCallback;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">日志路径</param>
		/// <param name="onLogCallback">日志回调</param>
		public static void Init(string path, Func<object, string> onLogCallback = null)
		{
			string directory = path.Substring(0, path.LastIndexOf('/'));
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			LogSystem.path = path;
			instance.m_OnLogCallback = onLogCallback;
		}

		/// <summary>
		/// 打印日志
		/// </summary>
		/// <param name="message">打印消息</param>
		public static void Log(object message)
		{
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError(LOG_NULL);

				return;
			}

			if (instance.m_OnLogCallback != null)
			{
				message = instance.m_OnLogCallback.Invoke(message);
			}

			StreamWriter streamWriter = new StreamWriter(path, true);
			streamWriter.WriteLine(message);
			streamWriter.Close();
		}

		/// <summary>
		/// 清理日志
		/// </summary>
		public static void Clear()
		{
			File.WriteAllText(path, string.Empty);
		}

		/// <summary>
		/// 日志回调
		/// </summary>
		/// <param name="condition">日志消息</param>
		/// <param name="stackTrace">调用堆栈</param>
		/// <param name="type">日志类型</param>
		private static void OnReceivedLog(string condition, string stackTrace, LogType type)
		{
			Log(string.Format(FORMAT_CONSOLE, type, condition));
		}
	}
}