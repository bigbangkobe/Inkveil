using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
	/// <summary>
	/// UI系统
	/// </summary>
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	[RequireComponent(typeof(UnityEngine.EventSystems.EventSystem), typeof(StandaloneInputModule))]
	public sealed class UISystem : MonoSingleton<UISystem>
	{
		/// <summary>
		/// 名字分隔符
		/// </summary>
		private const char SPLIT_NAME = '.';

		/// <summary>
		/// 画布对象
		/// </summary>
		public static Canvas canvas { get; private set; }
		/// <summary>
		/// 画布缩放对象
		/// </summary>
		public static CanvasScaler canvasScaler { get; private set; }
		/// <summary>
		/// 图像射线接收器
		/// </summary>
		public static GraphicRaycaster graphicRaycaster { get; private set; }
		/// <summary>
		/// 画布缩放对象
		/// </summary>
		public static UnityEngine.EventSystems.EventSystem eventSystem { get; private set; }
		/// <summary>
		/// UI摄像机
		/// </summary>
		public new static Camera camera { get { return canvas == null ? null : canvas.worldCamera; } }

		/// <summary>
		/// 画布大小
		/// </summary>
		public static Vector2 canvasSize { get { return (instance.transform as RectTransform).sizeDelta; } }
		/// <summary>
		/// 屏幕到UI缩放比例
		/// </summary>
		public static Vector2 screenToUIScale { get { return new Vector2(canvasSize.x / Screen.width, canvasSize.y / Screen.height); } }
		/// <summary>
		/// UI到屏幕缩放比例
		/// </summary>
		public static Vector2 uiToScreenScale { get { return new Vector2(Screen.width / canvasSize.x, Screen.height / canvasSize.y); } }
		/// <summary>
		/// 世界到UI缩放比例
		/// </summary>
		public static Vector2 worldToUIScale { get { return new Vector2(1 / canvas.transform.localScale.x, 1 / canvas.transform.localScale.y); } }
		/// <summary>
		/// UI到世界缩放比例
		/// </summary>
		public static Vector2 uiToWorldScale { get { return new Vector2(canvas.transform.localScale.x, canvas.transform.localScale.y); } }
		/// <summary>
		/// 世界到屏幕缩放比例
		/// </summary>
		public static Vector2 worldToScreenScale { get { return new Vector2(worldToUIScale.x * uiToScreenScale.x, worldToUIScale.y * uiToScreenScale.y); } }
		/// <summary>
		/// 屏幕到世界缩放比例
		/// </summary>
		public static Vector2 screenToWorldScale { get { return new Vector2(screenToUIScale.x * uiToWorldScale.x, screenToUIScale.y * uiToWorldScale.y); } }

		/// <summary>
		/// 面板表 [键:面板名 值:面板对象]
		/// </summary>
		private Dictionary<string, UIPanel> m_PanelMap = new Dictionary<string, UIPanel>();
		/// <summary>
		/// 面板链表
		/// </summary>
		private List<UIPanel> m_PanelList = new List<UIPanel>();
		/// <summary>
		/// 异步回调表 [键:面板名 值:回调]
		/// </summary>
		private Dictionary<string, Action<UIPanel>> m_AsyncCallbackMap = new Dictionary<string, Action<UIPanel>>();

		/// <summary>
		/// 更新回调
		/// </summary>
		private void Update()
		{
			if (m_PanelList != null)
			{
				for (int i = 0; i < m_PanelList.Count; ++i)
				{
					UIPanel panel = m_PanelList[i];
					if (!panel.enabled)
					{
						continue;
					}

					panel.OnUpdate();
				}
			}
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		private void OnDestroy()
		{
			if (m_PanelList != null)
			{
				for (int i = 0; i < m_PanelList.Count; ++i)
				{
					UIPanel panel = m_PanelList[i];
					panel.OnDestroy();
				}
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="renderMode">渲染模式</param>
		/// <param name="camera">摄像机</param>
		/// <param name="scaleMode">缩放模式</param>
		/// <param name="resolution">自适应分辨率</param>
		public static void Init(RenderMode renderMode,
			Camera camera,
			CanvasScaler.ScaleMode scaleMode,
			Vector2 resolution)
		{
			canvas = instance.gameObject.GetComponent<Canvas>();
			canvasScaler = instance.gameObject.GetComponent<CanvasScaler>();
			graphicRaycaster = instance.gameObject.GetComponent<GraphicRaycaster>();
			eventSystem = instance.gameObject.GetComponent<UnityEngine.EventSystems.EventSystem>();

			canvas.planeDistance = 0;
			canvas.renderMode = renderMode;
			canvas.worldCamera = camera;
			canvasScaler.uiScaleMode = scaleMode;
			canvasScaler.referenceResolution = resolution;
		}

		/// <summary>
		/// 打开面板
		/// </summary>
		/// <param name="bundleName">面板包名</param>
		/// <param name="type">面板类型</param>
		/// <param name="isAssetBundle">是否资源包加载</param>
		/// <returns>返回面板对象</returns>
		public async static Task<UIPanel> Open(string bundleName, Type type, bool isAssetBundle = true)
		{
			UIPanel panel = Get(bundleName);
			if (panel == null)
			{
				GameObject asset = null;
				if (isAssetBundle)
				{
					string name = bundleName.Substring(bundleName.LastIndexOf('/') + 1);
					name = name.Remove(name.LastIndexOf('.'));
					asset = AssetSystem.Load(bundleName, name, typeof(GameObject)) as GameObject;
				}
				else
				{
					asset = await ResourceService.LoadAsync<GameObject>(bundleName);
				}

				GameObject gameObject = Instantiate(asset);
				gameObject.name = asset.name;
				gameObject.transform.SetParent(instance.transform, false);

				panel = Activator.CreateInstance(type) as UIPanel;
				panel.Init(bundleName, gameObject);
				instance.m_PanelMap[bundleName] = panel;
				instance.m_PanelList.Add(panel);
			}

			panel.enabled = true;

			return panel;
		}

		/// <summary>
		/// 异步打开面板
		/// </summary>
		/// <param name="bundleName">面板包名</param>
		/// <param name="type">面板类型</param>
		/// <param name="openCallback">打开回调</param>
		/// <param name="isAssetBundle">是否资源包加载</param>
		public static void OpenAsync(string bundleName, Type type, Action<UIPanel> openCallback = null, bool isAssetBundle = true)
		{
			UIPanel panel = Get(bundleName);
			if (panel == null)
			{
				Action<UIPanel> callback = null;
				if (instance.m_AsyncCallbackMap.TryGetValue(bundleName, out callback))
				{
					callback -= openCallback;
					callback += openCallback;
					instance.m_AsyncCallbackMap[bundleName] = callback;
				}
				else
				{
					instance.m_AsyncCallbackMap[bundleName] = openCallback;
					if (isAssetBundle)
					{
						string name = bundleName.Substring(bundleName.LastIndexOf('/') + 1);
						name = name.Remove(name.LastIndexOf('.'));
						AssetSystem.LoadAsync(OnLoadComplete, bundleName, name, typeof(GameObject), new object[] { bundleName, type });
					}
					else
					{
						instance.StartCoroutine(OnLoadComplete(bundleName, type));
					}
				}
			}
			else
			{
				if (openCallback != null)
				{
					openCallback.Invoke(panel);
				}
				panel.enabled = true;
			}
		}

		/// <summary>
		/// 获取面板
		/// </summary>
		/// <param name="bundleName">面板包名</param>
		/// <returns>返回面板</returns>
		public static UIPanel Get(string bundleName)
		{
			UIPanel panel = null;
			instance.m_PanelMap.TryGetValue(bundleName, out panel);

			return panel;
		}

		/// <summary>
		/// 获取所有面板
		/// </summary>
		/// <returns></returns>
		public static UIPanel[] GetAll()
		{
			return instance.m_PanelList.ToArray();
		}

		/// <summary>
		/// 关闭面板
		/// </summary>
		/// <param name="bundleName">面板名</param>
		/// <param name="destroy">是否销毁</param>
		public static void Close(string bundleName, bool destroy = false)
		{
			UIPanel panel = Get(bundleName);
			if (panel == null)
			{
				return;
			}

			if (destroy)
			{
				instance.m_PanelMap.Remove(bundleName);
				instance.m_PanelList.Remove(panel);
				panel.OnDestroy();
				Destroy(panel.gameObject);
			}
			else
			{
				panel.enabled = false;
			}
		}

		/// <summary>
		/// 屏幕转UI坐标
		/// </summary>
		/// <param name="position">屏幕坐标</param>
		/// <returns>返回UI坐标</returns>
		public static Vector2 ScreenToUIPoint(Vector2 position)
		{
			RectTransform transform = instance.transform as RectTransform;
			Vector2 point = new Vector2((position.x / Screen.width - 0.5f) * transform.sizeDelta.x,
				(position.y / Screen.height - 0.5f) * transform.sizeDelta.y);

			return point;
		}

		/// <summary>
		/// UI转屏幕坐标
		/// </summary>
		/// <param name="position">UI坐标</param>
		/// <returns>返回屏幕坐标</returns>
		public static Vector2 UIToScreenPoint(Vector2 position)
		{
			RectTransform transform = instance.transform as RectTransform;
			Vector2 point = new Vector2((position.x / transform.sizeDelta.x + 0.5f) * Screen.width,
				(position.y / transform.sizeDelta.y + 0.5f) * Screen.height);

			return point;
		}

		/// <summary>
		/// 世界转UI坐标
		/// </summary>
		/// <param name="position">世界坐标</param>
		/// <returns>返回UI坐标</returns>
		public static Vector2 WorldToUIPoint(Vector3 position)
		{
			Vector2 screenPosition = WorldToScreenPoint(position);
			return ScreenToUIPoint(screenPosition);
		}

		/// <summary>
		/// UI转世界坐标
		/// </summary>
		/// <param name="position">UI坐标</param>
		/// <returns>返回世界坐标</returns>
		public static Vector3 UIToWorldPoint(Vector2 position)
		{
			Vector2 screenPosition = UIToScreenPoint(position);
			return ScreenToWorldPoint(screenPosition);
		}

		/// <summary>
		/// 世界转屏幕坐标
		/// </summary>
		/// <param name="position">世界坐标</param>
		/// <returns>返回屏幕坐标</returns>
		public static Vector3 WorldToScreenPoint(Vector3 position)
		{
			return camera.WorldToScreenPoint(position);
		}

		/// <summary>
		/// 屏幕转世界坐标
		/// </summary>
		/// <param name="position">屏幕坐标</param>
		/// <returns>返回世界坐标</returns>
		public static Vector3 ScreenToWorldPoint(Vector3 position)
		{
			return camera.ScreenToWorldPoint(position);
		}

		private static void OnLoadComplete(object asset, object arg)
		{
			GameObject uiAsset = asset as GameObject;
			GameObject gameObject = Instantiate(uiAsset);
			gameObject.name = uiAsset.name;
			gameObject.transform.SetParent(instance.transform, false);

			object[] args = arg as object[];
			string bundleName = args[0] as string;
			Type type = args[1] as Type;

			UIPanel panel = Activator.CreateInstance(type) as UIPanel;
			panel.Init(bundleName, gameObject);
			instance.m_PanelMap[bundleName] = panel;
			instance.m_PanelList.Add(panel);

			panel.enabled = true;
			Action<UIPanel> openCallback = instance.m_AsyncCallbackMap[bundleName];
			if (openCallback != null)
			{
				openCallback.Invoke(panel);
				instance.m_AsyncCallbackMap.Remove(bundleName);
			}
		}

        private static IEnumerator OnLoadComplete(string bundleName, Type type)
        {
            var handle = ResourceService.LoadAsync<GameObject>(bundleName); // 返回 Task<GameObject>
            while (!handle.IsCompleted)
                yield return null;

            var loadedAsset = handle.Result;

            var request = new FakeResourceRequest();
            request.SetAsset(loadedAsset);

            yield return request;

            GameObject gameObject = UnityEngine.Object.Instantiate(request.asset as GameObject);
            gameObject.name = request.asset.name;
            gameObject.transform.SetParent(instance.transform, false);

            UIPanel panel = Activator.CreateInstance(type) as UIPanel;
            panel.Init(bundleName, gameObject);
            instance.m_PanelMap[bundleName] = panel;
            instance.m_PanelList.Add(panel);

            panel.enabled = true;
            if (instance.m_AsyncCallbackMap.TryGetValue(bundleName, out var openCallback))
            {
                openCallback.Invoke(panel);
                instance.m_AsyncCallbackMap.Remove(bundleName);
            }
        }

    }
}