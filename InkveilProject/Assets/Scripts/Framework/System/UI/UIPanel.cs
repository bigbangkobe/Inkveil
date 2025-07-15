using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Framework
{
	/// <summary>
	/// 面板类
	/// </summary>
	public abstract class UIPanel
	{
		private const string LOG_GET = "{0} can't get {1} component:{2}.";

		/// <summary>
		/// 游戏物体
		/// </summary>
		public GameObject gameObject { get; private set; }
		/// <summary>
		/// 世界变换
		/// </summary>
		public RectTransform transform { get { return gameObject.transform as RectTransform; } }
		/// <summary>
		/// 面板包名
		/// </summary>
		public string bundleName { get; private set; }
		/// <summary>
		/// UI名
		/// </summary>
		public string name
		{
			get { return gameObject.name; }
			set { gameObject.name = value; }
		}
		/// <summary>
		/// 启用开关
		/// </summary>
		public virtual bool enabled
		{
			get { return gameObject.activeSelf; }
			set
			{
				gameObject.SetActive(value);
				if (value)
				{
					OnEnabled();
				}
				else
				{
					OnDisabled();
				}
			}
		}

		/// <summary>
		/// 获取面板数量
		/// </summary>
		/// <returns></returns>
		public int panelCount { get { return m_PanelList.Count; } }
		/// <summary>
		/// 面板链表
		/// </summary>
		protected List<UIPanel> m_PanelList = new List<UIPanel>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="bundleName">面板包名</param>
		/// <param name="gameObject">游戏物体</param>
		public void Init(string bundleName, GameObject gameObject)
		{
			this.bundleName = bundleName;
			this.gameObject = gameObject;

			OnInit();
		}

		/// <summary>
		/// 初始化回调
		/// </summary>
		public virtual void OnInit()
		{
		}

		/// <summary>
		/// 启用回调
		/// </summary>
		public virtual void OnEnabled()
		{
			for (int i = 0; i < m_PanelList.Count; ++i)
			{
				if (m_PanelList[i].enabled)
				{
					m_PanelList[i].OnEnabled();
				}
			}
		}

		/// <summary>
		/// 关闭回调
		/// </summary>
		public virtual void OnDisabled()
		{
			for (int i = 0; i < m_PanelList.Count; ++i)
			{
				m_PanelList[i].OnDisabled();
			}
		}

		/// <summary>
		/// 更新回调
		/// </summary>
		public virtual void OnUpdate()
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

		/// <summary>
		/// 销毁回调
		/// </summary>
		public virtual void OnDestroy()
		{
			for (int i = 0; i < m_PanelList.Count; ++i)
			{
				m_PanelList[i].OnDestroy();
			}
			m_PanelList.Clear();
		}

		/// <summary>
		/// 获取所有面板
		/// </summary>
		/// <returns>返回所有面板</returns>
		public UIPanel[] GetAllPanel()
		{
			return m_PanelList.ToArray();
		}

		/// <summary>
		/// 获取节点
		/// </summary>
		/// <param name="childName">节点名</param>
		/// <returns></returns>
		public Transform GetChild(string childName = null)
		{
			Transform child = string.IsNullOrEmpty(childName) ? transform : transform.Find(childName);
			if (child == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, "child", childName));
			}

			return child;
		}

		/// <summary>
		/// 获取面板
		/// </summary>
		/// <param name="type">面板类型</param>
		/// <param name="childName">面板节点名</param>
		/// <param name="bundleName">面板包名</param>
		/// <returns>返回面板</returns>
		public UIPanel GetPanel(Type type, string childName = null, string bundleName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			UIPanel panel = Activator.CreateInstance(type) as UIPanel;
			panel.Init(bundleName ?? childName, child.gameObject);
			m_PanelList.Add(panel);

			return panel;
		}

		/// <summary>
		/// 获取动态面板
		/// </summary>
		/// <param name="type">动态面板类型</param>
		/// <param name="bundleName">面板资源名</param>
		/// <param name="childName">面板节点名</param>
		/// <param name="isAssetBundle">是否资源包</param>
		/// <returns>返回动态面板</returns>
		public UIDynamicPanel GetDynamicPanel(Type type, string bundleName, string childName = null, bool isAssetBundle = true)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			GameObject asset = null;
			if (isAssetBundle)
			{
				string name = bundleName.Substring(bundleName.LastIndexOf('/') + 1);
				name = name.Remove(name.LastIndexOf('.'));
				asset = AssetSystem.Load(bundleName, name, typeof(GameObject)) as GameObject;
			}
			else
			{
				asset = ResourceService.Load<GameObject>(bundleName);
			}

			UIDynamicPanel panel = new UIDynamicPanel(type, asset);
			panel.Init(bundleName, child.gameObject);
			m_PanelList.Add(panel);

			return panel;
		}

		/// <summary>
		/// 获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component GetComponent(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component component = child.GetComponent(type);
			if (component == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return component;
		}

		/// <summary>
		/// 从子节点获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component GetComponentInChildren(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component component = child.GetComponentInChildren(type, true);
			if (component == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return component;
		}

		/// <summary>
		/// 从父节点获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component GetComponentInParent(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component component = child.GetComponentInParent(type);
			if (component == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return component;
		}

		/// <summary>
		/// 获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component[] GetComponents(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component[] components = child.GetComponents(type);
			if (components == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return components;
		}

		/// <summary>
		/// 从子节点获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component[] GetComponentsInChildren(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component[] components = child.GetComponentsInChildren(type, true);
			if (components == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return components;
		}

		/// <summary>
		/// 从父节点获取组件
		/// </summary>
		/// <param name="type">组件类型</param>
		/// <param name="childName">节点名</param>
		/// <returns>返回组件</returns>
		public Component[] GetComponentsInParent(Type type, string childName = null)
		{
			Transform child = GetChild(childName);
			if (child == null)
			{
				return null;
			}

			Component[] components = child.GetComponentsInParent(type, true);
			if (components == null)
			{
				Debug.LogError(string.Format(LOG_GET, name, type.Name, childName));
			}

			return components;
		}

        /// <summary>
        /// 显示UI
        /// </summary>
        protected virtual void OnShowEnable()
        {

        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        protected virtual void OnHideDisable()
        {

        }

        /// <summary>
        /// 显示当前UI
        /// </summary>
        /// <param name="param">附加参数</param>
        public void Show(object param = null)
        {
            OnShow(param);
        }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        public void Hide()
        {
            OnHide();
        }


        /// <summary>
        /// 初始化UI主要用于寻找组件等
        /// </summary>
        public void UIInit()
        {
            OnInit();
        }

        /// <summary>
        /// 显示当前界面
        /// </summary>
        /// <param name="param">附加参数</param>
        protected virtual void OnShow(object param)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack);
        }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        protected virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 每日重置
        /// </summary>
        /// <param name="day">周几重置，-1表示每天，0-6表示周日到周六</param>
        protected virtual void OnDayReset(object obj)
        {
            Debug.Log("每日重置");
        }

        /// <summary>
        /// 每周重置
        /// </summary>
        protected virtual void OnWeekReset(object obj)
        {
            Debug.Log("每周重置");
        }

        /// <summary>
        /// 每月重置
        /// </summary>
        protected virtual void OnMonthReset(object obj)
        {
            Debug.Log("每月重置");
        }
    }
}