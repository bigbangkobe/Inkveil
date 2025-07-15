using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 资源系统
	/// </summary>
	public sealed class AssetSystem : Singleton<AssetSystem>
	{
		private const string FORMAT_PATH = "{0}/{1}";
		private const string LOG_NULL = "Load complete callback is null.";
		private const string LOG_GET = "Can't find out asset bundle:{0}.";
		private const string LOG_LOAD = "Can't load asset:{0}.";

#if UNITY_EDITOR && !LOCAL_AB
		private const string EXT_ASSET_BUNDLE = ".bundle";
		private static Dictionary<Type, string> s_TypeMap = new Dictionary<Type, string>()
		{
			{ typeof(GameObject),	".prefab"},
			{ typeof(Material),		".mat"},
			{ typeof(TextAsset),	".json"},
			{ typeof(Texture),		".png"},
			{ typeof(Sprite),		".png"},
			{ typeof(AudioClip),	".mp3"},
		};
#endif

		/// <summary>
		/// 资源总表
		/// </summary>
		private AssetBundleManifest m_Manifest;
		/// <summary>
		/// 资源地址
		/// </summary>
		private string m_AssetURL;
		/// <summary>
		/// 资源包表 [键:资源包名 值:资源包]
		/// </summary>
		private Dictionary<string, AssetBundle> m_AssetBundleMap = new Dictionary<string, AssetBundle>();
		/// <summary>
		/// 资源请求表 [键:资源包名 值:加载请求]
		/// </summary>
		private Dictionary<string, AssetBundleCreateRequest> m_RequestMap = new Dictionary<string, AssetBundleCreateRequest>();
		/// <summary>
		/// 加载队列
		/// </summary>
		private Queue<LoadInfo> m_LoadQueue = new Queue<LoadInfo>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="assetURL">资源地址</param>
		/// <param name="manifest">资源包</param>
		public static void Init(string assetURL, string manifest)
		{
			Clear();

			instance.m_AssetURL = assetURL;
			string path = GetPath(manifest);
			if (File.Exists(path))
			{
				AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
				instance.m_Manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
				Add(manifest, assetBundle);
			}
		}

		/// <summary>
		/// 添加资源包
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="assetBundle">资源包</param>
		public static void Add(string bundleName, AssetBundle assetBundle)
		{
			instance.m_AssetBundleMap[bundleName] = assetBundle;
		}

		/// <summary>
		/// 移除资源包
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="unload">是否释放资源</param>
		public static void Remove(string bundleName, bool unload = true)
		{
			AssetBundle assetBundle = null;
			if (!instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle))
			{
				return;
			}

			if (unload)
			{
				assetBundle.Unload(true);
			}
			instance.m_AssetBundleMap.Remove(bundleName);
		}

		/// <summary>
		/// 获取资源路径
		/// </summary>
		/// <param name="bundleName"></param>
		/// <returns></returns>
		private static string GetPath(string bundleName)
		{
#if UNITY_EDITOR && !LOCAL_AB
			return string.Format(FORMAT_PATH, instance.m_AssetURL, bundleName);
#else
			string url = string.Format(FORMAT_PATH, instance.m_AssetURL, bundleName);
			string key;
			FileInfo fileInfo;
			DownloadSystem.HasCached(url, out key, out fileInfo);

			return fileInfo.FullName;
#endif
		}

		/// <summary>
		/// 获取资源包
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <returns>返回资源包</returns>
		public static AssetBundle Get(string bundleName)
		{
			AssetBundle assetBundle = null;
			if (instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle))
			{
				return assetBundle;
			}
			else if (instance.m_Manifest != null)
			{
				string[] dependencies = instance.m_Manifest.GetAllDependencies(bundleName);
				for (int i = 0; i < dependencies.Length; ++i)
				{
					string dependence = dependencies[i];
					Get(dependence);
				}

                string path = GetPath(bundleName);
				if (!File.Exists(path))
				{
					Debug.LogError(string.Format(LOG_GET, path));

					return null;
				}

				assetBundle = AssetBundle.LoadFromFile(path);
				Add(bundleName, assetBundle);
			}

			return assetBundle;
		}

		/// <summary>
		/// 异步获取资源包
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="onLoadComplete">加载回调</param>
		public static void GetAsync(string bundleName, Action<AssetBundle> onLoadComplete)
		{
			AssetBundle assetBundle = null;
			if (instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle))
			{
				if (onLoadComplete != null)
				{
					onLoadComplete.Invoke(assetBundle);
				}
			}
			else
			{
				List<string> dependenList = new List<string>();
				GetAllDependencies(bundleName, dependenList);
				Core.instance.StartCoroutine(LoadAssetBundleCoroutine(dependenList, () =>
				{
					instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle);
					onLoadComplete.Invoke(assetBundle);
				}));
			}
		}

		/// <summary>
		/// 获取所有依赖资源
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="dependenList">依赖列表</param>
		private static void GetAllDependencies(string bundleName, List<string> dependenList)
		{
			if (instance.m_Manifest == null)
			{
				return;
			}

			string[] dependencies = instance.m_Manifest.GetAllDependencies(bundleName);
			for (int i = 0; i < dependencies.Length; ++i)
			{
				string dependence = dependencies[i];
				if (dependenList.Contains(dependence))
				{
					continue;
				}

				GetAllDependencies(dependence, dependenList);
			}

			dependenList.Add(bundleName);
		}

		/// <summary>
		/// 加载协程
		/// </summary>
		/// <param name="bundleList">资源包链表</param>
		/// <param name="onLoadComplete">加载回调</param>
		/// <returns></returns>
		private static IEnumerator LoadAssetBundleCoroutine(List<string> bundleList, Action onLoadComplete)
		{
			for (int i = 0; i < bundleList.Count; ++i)
			{
				string bundleName = bundleList[i];
				AssetBundle assetBundle = null;
				if (instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle))
				{
					continue;
				}

				AssetBundleCreateRequest request = null;
				if (instance.m_RequestMap.TryGetValue(bundleName, out request))
				{
					yield return new WaitWhile(() => { return !request.isDone; });
				}
				else
				{
					string path = GetPath(bundleName);
					if (File.Exists(path))
					{
						request = AssetBundle.LoadFromFileAsync(path);
						instance.m_RequestMap[bundleName] = request;

						yield return request;

						assetBundle = request.assetBundle;
						Add(bundleName, assetBundle);
						instance.m_RequestMap.Remove(bundleName);
					}
					else
					{
						Debug.LogError(string.Format(LOG_GET, path));
					}
				}
			}

			onLoadComplete.Invoke();
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="name">资源名</param>
		/// <param name="type">资源类型</param>
		/// <returns>返回资源</returns>
		public static UnityEngine.Object Load(string bundleName, string name = null, Type type = null)
		{
			if (type == null)
			{
				type = typeof(GameObject);
			}

#if UNITY_EDITOR && !LOCAL_AB
			string path = null;
			string ext = s_TypeMap[type];
			if (name == null || bundleName.Contains(name))
			{
				path = "Assets/" + bundleName.Replace(EXT_ASSET_BUNDLE, ext);
			}
			else
			{
				path = "Assets/" + string.Format(FORMAT_PATH, bundleName.Replace(EXT_ASSET_BUNDLE, string.Empty), name + ext);
			}

			UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
			if (asset == null)
			{
				Debug.LogError(string.Format(LOG_LOAD, path));
			}

			return asset;
#else
			AssetBundle assetBundle = Get(bundleName);
			if (name == null)
			{
				name = bundleName.Substring(bundleName.LastIndexOf('/') + 1);
				name = name.Remove(name.LastIndexOf('.'));
            }

			return assetBundle.LoadAsset(name, type);
#endif
		}

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <param name="loadComplete">加载完成回调</param>
		/// <param name="bundleName">资源包名</param>
		/// <param name="name">资源名</param>
		/// <param name="type">资源类型</param>
		/// <param name="arg">加载参数</param>
		/// <returns>返回加载请求</returns>
		public static LoadInfo LoadAsync(Action<object, object> loadComplete, string bundleName, string name = null, Type type = null, object arg = null)
		{
			if (loadComplete == null)
			{
				Debug.LogError(LOG_NULL);

				return null;
			}

			if (name == null)
			{
				name = bundleName.Substring(bundleName.LastIndexOf('/') + 1);
				name = name.Remove(name.LastIndexOf('.'));
			}

			if (type == null)
			{
				type = typeof(GameObject);
			}

			LoadInfo loadInfo = new LoadInfo();
			loadInfo.loadCallback = loadComplete;
			loadInfo.bundleName = bundleName;
			loadInfo.name = name;
			loadInfo.type = type;
			loadInfo.arg = arg;

#if UNITY_EDITOR && !LOCAL_AB
			string path = null;
			string ext = s_TypeMap[type];
			if (name == null || bundleName.Contains(name))
			{
				path = "Assets/" + bundleName.Replace(EXT_ASSET_BUNDLE, ext);
			}
			else
			{
				path = "Assets/" + string.Format(FORMAT_PATH, bundleName.Replace(EXT_ASSET_BUNDLE, string.Empty), name + ext);
			}

			UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);
			if (asset == null)
			{
				Debug.LogError(string.Format(LOG_LOAD, path));
			}
			else
			{
				loadComplete.Invoke(asset, loadInfo.arg);
			}
#else
			if (instance.m_LoadQueue.Count > 0)
			{
				instance.m_LoadQueue.Enqueue(loadInfo);
			}
			else
			{
				StartLoad(loadInfo);
			}
#endif

			return loadInfo;
		}

		/// <summary>
		/// 加载所有资源
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="type">资源类型</param>
		/// <returns>返回资源</returns>
		public static UnityEngine.Object[] LoadAll(string bundleName, Type type = null)
		{
#if UNITY_EDITOR && !LOCAL_AB
			string path = "Assets/" + bundleName.Replace(EXT_ASSET_BUNDLE, string.Empty);

			UnityEngine.Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
			if (assets == null)
			{
				Debug.LogError(string.Format(LOG_LOAD, path));
			}

			return assets;
#else
			AssetBundle assetBundle = Get(bundleName);

			return type == null ? assetBundle.LoadAllAssets() : assetBundle.LoadAllAssets(type);
#endif
		}

		/// <summary>
		/// 异步加载所有资源
		/// </summary>
		/// <param name="loadCallback">加载完成回调</param>
		/// <param name="bundleName">资源包名</param>
		/// <param name="type">资源类型</param>
		/// <param name="arg">加载参数</param>
		/// <returns>返回加载请求</returns>
		public static LoadInfo LoadAllAsync(Action<object, object> loadCallback, string bundleName, Type type = null, object arg = null)
		{
			if (loadCallback == null)
			{
				Debug.LogError(LOG_NULL);

				return null;
			}

			LoadInfo loadInfo = new LoadInfo();
			loadInfo.loadCallback = loadCallback;
			loadInfo.bundleName = bundleName;
			loadInfo.type = type;
			loadInfo.arg = arg;

#if UNITY_EDITOR && !LOCAL_AB
			string path = "Assets/" + bundleName.Replace(EXT_ASSET_BUNDLE, string.Empty);

			UnityEngine.Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
			if (assets == null)
			{
				Debug.LogError(string.Format(LOG_LOAD, path));
			}
			else
			{
				loadCallback.Invoke(assets, loadInfo.arg);
			}
#else
			if (instance.m_LoadQueue.Count > 0)
			{
				instance.m_LoadQueue.Enqueue(loadInfo);
			}
			else
			{
				StartLoad(loadInfo);
			}
#endif

			return loadInfo;
		}

		/// <summary>
		/// 卸载资源
		/// </summary>
		/// <param name="bundleName">资源包名</param>
		/// <param name="unloadAll">是否卸载全部资源</param>
		public static void Unload(string bundleName, bool unloadAll)
		{
			AssetBundle assetBundle = null;
			if (!instance.m_AssetBundleMap.TryGetValue(bundleName, out assetBundle))
			{
				return;
			}

			assetBundle.Unload(unloadAll);
			instance.m_AssetBundleMap.Remove(bundleName);
		}

		/// <summary>
		/// 清除所有资源
		/// </summary>
		public static void Clear()
		{
			foreach (AssetBundle assetBundle in instance.m_AssetBundleMap.Values)
			{
				assetBundle.Unload(true);
			}
			instance.m_AssetBundleMap.Clear();
		}

		/// <summary>
		/// 开始加载
		/// </summary>
		/// <param name="loadInfo">加载信息</param>
		private static void StartLoad(LoadInfo loadInfo)
		{
			GetAsync(loadInfo.bundleName, (assetBundle) =>
			{
				Core.instance.StartCoroutine(LoadCoroutine(loadInfo, assetBundle));
			});
		}

		/// <summary>
		/// 加载协程
		/// </summary>
		/// <param name="loadInfo">加载信息</param>
		/// <returns>返回迭代器</returns>
		private static IEnumerator LoadCoroutine(LoadInfo loadInfo, AssetBundle assetBundle)
		{
			AssetBundleRequest request = null;

			bool isLoadAll = string.IsNullOrEmpty(loadInfo.name);
			if (isLoadAll)
			{
				request = loadInfo.type == null ? assetBundle.LoadAllAssetsAsync() : assetBundle.LoadAllAssetsAsync(loadInfo.type);
			}
			else
			{
				request = loadInfo.type == null ? assetBundle.LoadAssetAsync(loadInfo.name) : assetBundle.LoadAssetAsync(loadInfo.name, loadInfo.type);
			}
			loadInfo.request = request;

			yield return request;

			loadInfo.loadCallback.Invoke(isLoadAll ? (object)request.allAssets : request.asset, loadInfo.arg);

			if (instance.m_LoadQueue.Count > 0)
			{
				loadInfo = instance.m_LoadQueue.Dequeue();
				StartLoad(loadInfo);
			}
		}

		/// <summary>
		/// 加载信息
		/// </summary>
		public sealed class LoadInfo
		{
			/// <summary>
			/// 加载回调
			/// </summary>
			public Action<object, object> loadCallback { get; internal set; }
			/// <summary>
			/// 资源包名
			/// </summary>
			public string bundleName { get; internal set; }
			/// <summary>
			/// 资源名
			/// </summary>
			public string name { get; internal set; }
			/// <summary>
			/// 资源类型
			/// </summary>
			public Type type { get; internal set; }
			/// <summary>
			/// 加载参数
			/// </summary>
			public object arg { get; internal set; }
			/// <summary>
			/// 资源包请求
			/// </summary>
			public AssetBundleRequest request { get; internal set; }
		}
	}
}