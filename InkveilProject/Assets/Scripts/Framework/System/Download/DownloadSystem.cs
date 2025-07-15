﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 下载系统
	/// </summary>
	public sealed class DownloadSystem : Singleton<DownloadSystem>
	{
		private const string LOG_NULL = "Url is null.";
		private const string FORMAT_PREFS = "Cache_{0}";
		private const string FORMAT_URL = "file://{0}";
		private const string FORMAT_PATH = "{0}/{1}";

		/// <summary>
		/// 缓存路径
		/// </summary>
		public static string cachePath;

		/// <summary>
		/// 下载表 [键:URL 值:下载信息]
		/// </summary>
		private static Dictionary<string, DownloadInfo> s_DownloadMap = new Dictionary<string, DownloadInfo>();
		/// <summary>
		/// 下载队列
		/// </summary>
		private static Queue<DownloadInfo> s_DownloadQueue = new Queue<DownloadInfo>();

		/// <summary>
		/// 初始化下载系统
		/// </summary>
		/// <param name="cachePath">缓存路径</param>
		public static void Init(string cachePath)
		{
			DownloadSystem.cachePath = cachePath;
		}

		/// <summary>
		/// 下载
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="downloadCallback">下载回调</param>
		/// <param name="version">版本号</param>
		/// <param name="type">下载类型</param>
		/// <returns>返回下载信息</returns>
		public static DownloadInfo Download(string url, Action<UnityWebRequest,object> downloadCallback, long version = 0, Type type = null,object arg = null)
		{
			//如果url时空的报错
			if (string.IsNullOrEmpty(url))
			{
				Debug.LogError(LOG_NULL);

				return null;
			}

			//尝试获取已下载好的信息,有则直接返回和调用回调
			DownloadInfo downloadInfo = null;
			if (s_DownloadMap.TryGetValue(url, out downloadInfo))
			{
				if (downloadInfo.request.isDone && downloadCallback != null)
				{
                    downloadCallback.Invoke(downloadInfo.request, arg);
				}
				else
				{
					downloadInfo.downloadCallback += downloadCallback;
				}
			}
			else
			{
				//构建下载信息
				downloadInfo = new DownloadInfo();
				downloadInfo.url = url;
				downloadInfo.downloadCallback = downloadCallback;
				downloadInfo.version = version;
                downloadInfo.type = type;
                downloadInfo.arg = arg;
				s_DownloadMap[url] = downloadInfo;

				if (s_DownloadQueue.Count > 0)
				{
					s_DownloadQueue.Enqueue(downloadInfo);
				}
				else
				{
					StartDownload(downloadInfo);
				}
			}

			return downloadInfo;
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		public static void Clear()
		{
			foreach (DownloadInfo downloadInfo in s_DownloadMap.Values)
			{
				if (downloadInfo.request != null)
				{
					downloadInfo.request.Dispose();
				}
			}

			s_DownloadMap.Clear();
		}

		/// <summary>
		/// 是否缓存
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="version">版本</param>
		/// <returns>返回结果</returns>
		public static bool HasCached(string url, long version = 0)
		{
			string key;
			FileInfo fileInfo;

			return HasCached(url, out key, out fileInfo, version);
		}

		/// <summary>
		/// 是否缓存
		/// </summary>
		/// <param name="url">地址</param>
		/// <param name="key">储存键</param>
		/// <param name="fileInfo">文件信息</param>
		/// <param name="version">版本</param>
		/// <returns>返回结果</returns>
		public static bool HasCached(string url, out string key, out FileInfo fileInfo, long version = 0)
		{
			string md5 = Extension.ToMD5(url.ToLower());
			key = string.Format(FORMAT_PREFS, md5);
			fileInfo = new FileInfo(string.Format(FORMAT_PATH, cachePath, md5));
			//获取版本信息
			if (version >= 0 && PlayerPrefs.HasKey(key))
			{
				//对比版本
				long localVersion = long.Parse(PlayerPrefs.GetString(key, "0"));
				if (localVersion == version && fileInfo.Exists)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 清理缓存
		/// </summary>
		/// <param name="url"></param>
		public static void ClearCache(string url)
		{
			string key;
			FileInfo fileInfo;
			HasCached(url, out key, out fileInfo);

			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}

			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
		}

		/// <summary>
		/// 清理所有缓存
		/// </summary>
		public static void ClearAllCache()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(cachePath);
			FileInfo[] fileInfos = directoryInfo.GetFiles();
			for (int i = 0; i < fileInfos.Length; ++i)
			{
				FileInfo fileInfo = fileInfos[i];
				string key = string.Format(FORMAT_PREFS, fileInfo.Name);
				if (PlayerPrefs.HasKey(key))
				{
					PlayerPrefs.DeleteKey(key);
				}
				fileInfo.Delete();
			}
		}

		/// <summary>
		/// 开始下载
		/// </summary>
		/// <param name="downloadInfo">下载信息</param>
		private static void StartDownload(DownloadInfo downloadInfo)
		{
			Core.instance.StartCoroutine(DownloadCoroutine(downloadInfo));
		}

		/// <summary>
		/// 下载协程
		/// </summary>
		/// <param name="downloadInfo">下载信息</param>
		/// <returns>返回迭代器</returns>
		private static IEnumerator DownloadCoroutine(DownloadInfo downloadInfo)
		{
			//查看缓存信息
			string key;
			FileInfo fileInfo;
			bool cached = HasCached(downloadInfo.url, out key, out fileInfo, downloadInfo.version);
			string url = cached ? string.Format(FORMAT_URL, fileInfo.FullName) : downloadInfo.url;

			if (downloadInfo.type == typeof(Texture))
			{
				downloadInfo.request = UnityWebRequestTexture.GetTexture(url);
			}
			else
			{
				downloadInfo.request = UnityWebRequest.Get(url);
			}

		    downloadInfo.request.timeout = 3600;

			yield return downloadInfo.request.SendWebRequest();

			//如果发生网络错误
			if (!string.IsNullOrEmpty(downloadInfo.request.error))
			{
				Debug.LogError(downloadInfo.request.error);
			}
			//如果非读取缓存则写入缓存
			else if (!cached)
			{
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				else if (!fileInfo.Directory.Exists)
				{
					fileInfo.Directory.Create();
				}

				File.WriteAllBytes(fileInfo.FullName, downloadInfo.request.downloadHandler.data);
				PlayerPrefs.SetString(key, downloadInfo.version.ToString());
			}

			//调用回调
			if (downloadInfo.downloadCallback != null)
			{
				downloadInfo.downloadCallback.Invoke(downloadInfo.request, downloadInfo.arg);
				downloadInfo.downloadCallback = null;
			}

			//下载下一个资源
			if (s_DownloadQueue.Count > 0)
			{
				downloadInfo = s_DownloadQueue.Dequeue();
				StartDownload(downloadInfo);
			}
		}

		/// <summary>
		/// 下载信息
		/// </summary>
		public sealed class DownloadInfo
		{
            /// <summary>
            /// 下载的额外参数
            /// </summary>
            public object arg { get; internal set; }
			/// <summary>
			/// 下载地址
			/// </summary>
			public string url { get; internal set; }
			/// <summary>
			/// 版本号
			/// </summary>
			public long version { get; internal set; }
			/// <summary>
			/// 下载类型
			/// </summary>
			public Type type { get; internal set; }
			/// <summary>
			/// 下载回调
			/// </summary>
			public Action<UnityWebRequest,object> downloadCallback { get; internal set; }
			/// <summary>
			/// 下载请求
			/// </summary>
			public UnityWebRequest request { get; internal set; }
		}
	}
}