using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Framework
{
	/// <summary>
	/// 动态面板
	/// </summary>
	public sealed class UIDynamicPanel : UIPanel
	{
		/// <summary>
		/// 面板类型
		/// </summary>
		private Type m_PanelType;
		/// <summary>
		/// 面板预设
		/// </summary>
		private GameObject m_PanelPrefab;
		/// <summary>
		/// 面板对象池
		/// </summary>
		private ObjectPool m_PanelPool;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="type">面板类型</param>
		/// <param name="prefab">面板父节点</param>
		public UIDynamicPanel(Type type, GameObject prefab)
		{
			m_PanelType = type;
			m_PanelPrefab = prefab;
			m_PanelPool = new ObjectPool(OnPanelConstruct, OnPanelDestroy, OnPanelEnabled, OnPanelDisabled);
		}

		/// <summary>
		/// 启用回调
		/// </summary>
		public override void OnEnabled()
		{
		}

		/// <summary>
		/// 关闭回调
		/// </summary>
		public override void OnDisabled()
		{
		}

		/// <summary>
		/// 销毁函数
		/// </summary>
		public override void OnDestroy()
		{
			base.OnDestroy();

			Clear();
		}

		/// <summary>
		/// 添加面板
		/// </summary>
		/// <returns>返回面板对象</returns>
		public async Task<UIPanel> Add()
		{
			UIPanel panel = await m_PanelPool.GetAsync() as UIPanel;
			m_PanelList.Add(panel);

			return panel;
		}

		/// <summary>
		/// 移除面板
		/// </summary>
		/// <param name="panel">面板对象</param>
		public void Remove(UIPanel panel)
		{
			m_PanelPool.Remove(panel);
			m_PanelList.Remove(panel);
		}

		/// <summary>
		/// 清理面板
		/// </summary>
		public void Clear()
		{
			m_PanelPool.Clear();
			m_PanelList.Clear();
		}

		/// <summary>
		/// 面板构造回调
		/// </summary>
		/// <returns>返回面板</returns>
		private Task<object> OnPanelConstruct(object obj)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_PanelPrefab);
			gameObject.name = m_PanelPrefab.name;
			gameObject.SetActive(false);
			gameObject.transform.SetParent(transform, false);

			UIPanel panel = Activator.CreateInstance(m_PanelType) as UIPanel;
			panel.Init(bundleName, gameObject);

            return Task.FromResult<object>(panel); // ✅ 正确包装返回值
        }

        /// <summary>
        /// 面板销毁回调
        /// </summary>
        /// <param name="obj">对象</param>
        private void OnPanelDestroy(object obj, object o = null)
		{
			UIPanel panel = obj as UIPanel;
			panel.OnDestroy();
			UnityEngine.Object.Destroy(panel.gameObject);
		}

		/// <summary>
		/// 面板启用回调
		/// </summary>
		/// <param name="obj">对象</param>
		private void OnPanelEnabled(object obj, object o = null)
		{
			UIPanel panel = obj as UIPanel;
			panel.enabled = true;
		}

		/// <summary>
		/// 面板关闭回调
		/// </summary>
		/// <param name="obj">对象</param>
		private void OnPanelDisabled(object obj, object o = null)
		{
			UIPanel panel = obj as UIPanel;
			panel.enabled = false;
		}
	}
}