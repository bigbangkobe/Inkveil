using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 调试系统
	/// </summary>
	public sealed class DebugSystem : MonoSingleton<DebugSystem>
	{
		private const string LOG_GET = "Can't find out debug:{0}.";
		private const string GUI_BOX = "Box";
		private const string GUI_LEFT = "<";
		private const string GUI_RIGHT = ">";

		/// <summary>
		/// UI尺寸
		/// </summary>
		public static int size { get { return (int)(Screen.height * instance.scale); } }

		/// <summary>
		/// UI缩放
		/// </summary>
		[Range(0.01f, 0.1f)]
		public float scale = 0.02f;

		/// <summary>
		/// 调试开关
		/// </summary>
		private bool m_Enabled = false;
		/// <summary>
		/// 调试对象表 [键:调试名 值:调试对象]
		/// </summary>
		private Dictionary<string, DebugObject> m_DebugObjectMap = new Dictionary<string, DebugObject>();
		/// <summary>
		/// 调试对象数组
		/// </summary>
		private DebugObject[] m_DebugObjects;
		/// <summary>
		/// 当前渲染对象
		/// </summary>
		private DebugObject m_DebugObject;

		/// <summary>
		/// 绘制回调
		/// </summary>
		private void OnGUI()
		{
			GUI.skin.label.fontSize = size;
			GUI.skin.label.normal.textColor = Color.white;
			GUI.skin.button.fontSize = size;
			GUI.skin.textField.fontSize = size;
			GUI.skin.textArea.fontSize = size;

			float dSize = size * 2;
			GUI.skin.horizontalScrollbar.fixedHeight = size;
			GUI.skin.horizontalScrollbarThumb.fixedWidth = dSize;
			GUI.skin.horizontalScrollbarThumb.fixedHeight = size;
			GUI.skin.verticalScrollbar.fixedWidth = size;
			GUI.skin.verticalScrollbarThumb.fixedWidth = size;
			GUI.skin.verticalScrollbarThumb.fixedHeight = dSize;

			GUILayout.BeginHorizontal(GUI_BOX, GUILayout.Width(m_Enabled ? Screen.width : 0));
			if (GUILayout.Button(m_Enabled ? GUI_LEFT : GUI_RIGHT, GUILayout.Width(dSize)))
			{
				m_Enabled = !m_Enabled;
			}

			if (!m_Enabled)
			{
				GUILayout.EndHorizontal();

				return;
			}

			if (m_DebugObjects != null)
			{
				for (int i = 0; i < m_DebugObjects.Length; ++i)
				{
					DebugObject debugObject = m_DebugObjects[i];
					if (GUILayout.Button(debugObject.title))
					{
						m_DebugObject = m_DebugObject == debugObject ? null : debugObject;
					}
				}
			}
			GUILayout.EndHorizontal();

			if (m_DebugObject != null)
			{
				m_DebugObject.OnGUI();
			}
		}

		/// <summary>
		/// 更新回调
		/// </summary>
		private void Update()
		{
			if (m_DebugObject != null)
			{
				m_DebugObject.OnUpdate();
			}
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		private void OnDestroy()
		{
			if (m_DebugObjects != null)
			{
				for (int i = 0; i < m_DebugObjects.Length; ++i)
				{
					DebugObject debugObject = m_DebugObjects[i];
					debugObject.OnDestroy();
				}
			}

			m_DebugObjectMap.Clear();
		}

		/// <summary>
		/// 添加调试对象
		/// </summary>
		/// <param name="name">调试名</param>
		/// <param name="type">调试类型</param>
		/// <returns>返回调试对象</returns>
		public static DebugObject Add(string name, Type type)
		{
			DebugObject debugObject = null;
			if (instance.m_DebugObjectMap.TryGetValue(name, out debugObject))
			{
				return debugObject;
			}

			debugObject = Activator.CreateInstance(type) as DebugObject;
			debugObject.Init(name);
			instance.m_DebugObjectMap[name] = debugObject;
			instance.m_DebugObjects = new DebugObject[instance.m_DebugObjectMap.Count];
			instance.m_DebugObjectMap.Values.CopyTo(instance.m_DebugObjects, 0);

			return debugObject;
		}

		/// <summary>
		/// 获取调试对象
		/// </summary>
		/// <param name="name">调试名</param>
		/// <returns>返回调试对象</returns>
		public static DebugObject Get(string name)
		{
			DebugObject debugObject = null;
			instance.m_DebugObjectMap.TryGetValue(name, out debugObject);

			return debugObject;
		}

		/// <summary>
		/// 移除调试对象
		/// </summary>
		/// <param name="name">调试名</param>
		public static void Remove(string name)
		{
			DebugObject debugObject = null;
			if (!instance.m_DebugObjectMap.TryGetValue(name, out debugObject))
			{
				Debug.LogError(string.Format(LOG_GET, name));

				return;
			}

			debugObject.OnDestroy();
			instance.m_DebugObjectMap.Remove(name);
			instance.m_DebugObjects = new DebugObject[instance.m_DebugObjectMap.Count];
			instance.m_DebugObjectMap.Values.CopyTo(instance.m_DebugObjects, 0);
		}
	}
}