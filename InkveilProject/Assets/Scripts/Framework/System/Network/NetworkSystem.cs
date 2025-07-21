using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 网络系统
	/// </summary>
	public sealed class NetworkSystem : Singleton<NetworkSystem>
	{
		private const string LOG_GET = "Can't find out network server:{0}.";
		private const string FORMAT_COMMAND = "{0}/{1}";
		private const string FORMAT_ARG = "{0}?{1}";
		private const string FORMAT_ARGS = "{0}={1}&";

		/// <summary>
		/// Http请求回调
		/// </summary>
		public static event Action<UnityWebRequest> onHttpRequest;
		/// <summary>
		/// Http返回回调
		/// </summary>
		public static event Action<UnityWebRequest> onHttpReturn;

		/// <summary>
		/// 服务器表 [键:服务器地址 值:服务器对象]
		/// </summary>
		private Dictionary<string, NetworkServer> m_NetworkServerMap = new Dictionary<string, NetworkServer>();
		/// <summary>
		/// 服务器对象数组
		/// </summary>
		private NetworkServer[] m_NetworkServers;

		/// <summary>
		/// 更新回调
		/// </summary>
		private void Update()
		{
			if (m_NetworkServers != null)
			{
				for (int i = 0; i < m_NetworkServers.Length; ++i)
				{
					m_NetworkServers[i].OnUpdate();
				}
			}
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		private void OnDestroy()
		{
			Clear();
		}

		/// <summary>
		/// 添加服务器
		/// </summary>
		/// <param name="name">服务器名</param>
		/// <param name="buffSize">缓存长度</param>
		/// <param name="pingTime">Ping时间间隔</param>
		/// <returns>返回服务器对象</returns>
		public static NetworkServer Add(string name, int buffSize, float pingTime)
		{
			NetworkServer networkServer = null;
			if (!instance.m_NetworkServerMap.TryGetValue(name, out networkServer))
			{
				networkServer = new NetworkServer(name, buffSize, pingTime);
				instance.m_NetworkServerMap[name] = networkServer;
				instance.m_NetworkServers = new NetworkServer[instance.m_NetworkServerMap.Count];
				instance.m_NetworkServerMap.Values.CopyTo(instance.m_NetworkServers, 0);
			}

			return networkServer;
		}

		/// <summary>
		/// 获取服务器
		/// </summary>
		/// <param name="name">服务器名</param>
		/// <returns>返回服务器</returns>
		public static NetworkServer Get(string name)
		{
			NetworkServer networkServer = null;
			if (!instance.m_NetworkServerMap.TryGetValue(name, out networkServer))
			{
				Debug.LogError(string.Format(LOG_GET, name));
			}

			return networkServer;
		}

		/// <summary>
		/// 获取所有服务器
		/// </summary>
		/// <returns>返回所有服务器</returns>
		public static NetworkServer[] GetAll()
		{
			return instance.m_NetworkServers;
		}

		/// <summary>
		/// 移除服务器
		/// </summary>
		/// <param name="name">服务器名</param>
		public static void Remove(string name)
		{
			NetworkServer networkServer = null;
			if (!instance.m_NetworkServerMap.TryGetValue(name, out networkServer))
			{
				return;
			}

			networkServer.Disconnect(true);

			instance.m_NetworkServerMap.Remove(name);
			instance.m_NetworkServers = new NetworkServer[instance.m_NetworkServerMap.Count];
			instance.m_NetworkServerMap.Values.CopyTo(instance.m_NetworkServers, 0);
		}

		/// <summary>
		/// 清理服务器
		/// </summary>
		public static void Clear()
		{
			for (int i = 0; i < instance.m_NetworkServers.Length; ++i)
			{
				instance.m_NetworkServers[i].OnDestroy();
			}
			instance.m_NetworkServers = null;

			instance.m_NetworkServerMap.Clear();
		}

		/// <summary>
		/// 请求HttpGet
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="callback">返回回调</param>
		/// <param name="command">命令</param>
		/// <param name="args">参数</param>
		public static void RequestHttpGet(string url, Action<UnityWebRequest> callback, string command, params string[] args)
		{
			RequestHttp(url, UnityWebRequest.kHttpVerbGET, callback, null, command, args);
		}

		/// <summary>
		/// 请求HttpPut
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="callback">返回回调</param>
		/// <param name="data">数据</param>
		/// <param name="command">命令</param>
		/// <param name="args">参数</param>
		public static void RequestHttpPut(string url, Action<UnityWebRequest> callback, string data, string command, params string[] args)
		{
			RequestHttp(url, UnityWebRequest.kHttpVerbPUT, callback, data, command, args);
		}

		/// <summary>
		/// 请求HttpPost
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="callback">返回回调</param>
		/// <param name="data">数据</param>
		/// <param name="command">命令</param>
		/// <param name="args">参数</param>
		public static void RequestHttpPost(string url, Action<UnityWebRequest> callback, string data, string command, params string[] args)
		{
			RequestHttp(url, UnityWebRequest.kHttpVerbPOST, callback, data, command, args);
		}

		/// <summary>
		/// HttpGet
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="method">方法</param>
		/// <param name="callback">返回回调</param>
		/// <param name="data">数据</param>
		/// <param name="command">命令</param>
		/// <param name="args">参数</param>
		private static void RequestHttp(string url, string method, Action<UnityWebRequest> callback, string data, string command, params string[] args)
		{
			if (!string.IsNullOrEmpty(command))
			{
				url = string.Format(FORMAT_COMMAND, url, command);
			}

			if (args != null && args.Length > 0)
			{
				string arg = string.Empty;
				for (int i = 0; i < args.Length; i += 2)
				{
					if (i + 1 >= args.Length)
					{
						break;
					}
					string key = UnityWebRequest.EscapeURL(args[i]);
					string value = UnityWebRequest.EscapeURL(args[i + 1]);
					arg += string.Format(FORMAT_ARGS, key, value);
				}
				arg = arg.Remove(arg.Length - 1);
				url = string.Format(FORMAT_ARG, url, arg);
			}

			Core.instance.StartCoroutine(HttpCoroutine(url, method, data, callback));
		}

		/// <summary>
		/// Http协程
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="method">方法</param>
		/// <param name="callback">返回回调</param>
		/// <returns>返回迭代器</returns>
		private static IEnumerator HttpCoroutine(string url, string method, string data, Action<UnityWebRequest> callback)
		{
			UnityWebRequest request = null;
			if (method == UnityWebRequest.kHttpVerbPUT)
			{
				request = UnityWebRequest.Put(url, data);
			}
			else if (method == UnityWebRequest.kHttpVerbPOST)
			{
				request = UnityWebRequest.Post(url, data);
			}
			else
			{
				request = UnityWebRequest.Get(url);
			}

			if (onHttpRequest != null)
			{
				onHttpRequest.Invoke(request);
			}

			yield return request.SendWebRequest();

			if (onHttpReturn != null)
			{
				onHttpReturn.Invoke(request);
			}

			if (callback != null)
			{
				callback.Invoke(request);
			}

			request.Dispose();
		}
	}
}