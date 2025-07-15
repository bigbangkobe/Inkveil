using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
	/// <summary>
	/// 滚动页面
	/// </summary>
	public sealed class UIScrollPage : ScrollRect
	{
		/// <summary>
		/// 是否在拖拽
		/// </summary>
		public bool isDraging { get; private set; }
		/// <summary>
		/// 页面索引值
		/// </summary>
		public int page { get; private set; }

		/// <summary>
		/// 选项组
		/// </summary>
		private ToggleGroup m_ToggleGroup;
		/// <summary>
		/// 选项资源
		/// </summary>
		private Toggle m_ToggleAsset;
		/// <summary>
		/// 选项对象池
		/// </summary>
		private ObjectPool m_TogglePool;
		/// <summary>
		/// 选项链表
		/// </summary>
		private List<Toggle> m_ToggleList = new List<Toggle>();
		/// <summary>
		/// 目标位置
		/// </summary>
		private float m_TargetPosition;
		/// <summary>
		/// 是否立刻
		/// </summary>
		private bool m_Immediate;

		private List<Transform> m_ChildList = new List<Transform>();
		private Vector2 m_Offset;
		private int m_Add;

		protected override void Awake()
		{
			base.Awake();

			m_ToggleGroup = GetComponentInChildren<ToggleGroup>();
			m_ToggleAsset = GetComponentInChildren<Toggle>(true);
			m_TogglePool = new ObjectPool(OnToggleConstruct, OnToggleDestroy, OnToggleEnabled, OnToggleDisabled);
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif

			UpdateToggle();
			UpdatePosition();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (m_ChildList.Count == content.childCount)
			{
				for (int i = 0; i < m_ChildList.Count; ++i)
				{
					Transform child = m_ChildList[i];
					child.SetSiblingIndex(i);
				}
				horizontalNormalizedPosition = 0;
				m_TargetPosition = 0;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (m_TogglePool != null)
			{
				m_TogglePool.Clear();
			}
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);

			isDraging = true;
			m_Offset = Vector2.zero;
			m_Add = 0;
		}

		public override void OnDrag(PointerEventData eventData)
		{
			float position = horizontalNormalizedPosition;
			if (position < 0)
			{
				RectTransform child = content.GetChild(content.childCount - 1) as RectTransform;
				child.SetAsFirstSibling();
				--m_Add;

				float width = viewport.sizeDelta.x;
				float scale = Screen.width / (UISystem.canvas.transform as RectTransform).sizeDelta.x;
				m_Offset = new Vector2(m_Add * width * scale, 0);
			}
			else if (position > 1)
			{
				RectTransform child = content.GetChild(0) as RectTransform;
				child.SetAsLastSibling();
				++m_Add;

				float width = viewport.sizeDelta.x;
				float scale = Screen.width / (UISystem.canvas.transform as RectTransform).sizeDelta.x;
				m_Offset = new Vector2(m_Add * width * scale, 0);
			}

			eventData.position += m_Offset;

			base.OnDrag(eventData);
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);

			isDraging = false;

			float temp = 1.0f / (content.childCount - 1);
			float position = horizontalNormalizedPosition % 1;
			if (position < 0)
			{
				position += 1;
			}

			int index = 0;
			for (int i = 0; i < content.childCount; ++i)
			{
				float delta = Mathf.Abs(position - (i * temp));
				if (Mathf.Abs(delta) <= 0.5f * temp)
				{
					index = i;

					break;
				}
			}

			m_TargetPosition = index * temp;

			index %= content.childCount;
			index = m_ChildList.IndexOf(content.GetChild(index));
			Page(index);
		}

		/// <summary>
		/// 翻页
		/// </summary>
		/// <param name="index">页面索引值</param>
		/// <param name="immediate">立即切换</param>
		public void Page(int index, bool immediate = false)
		{
			if (m_TogglePool == null)
			{
				return;
			}

			m_Immediate = immediate;
			index = Mathf.Clamp(index, 0, content.childCount - 1);

			for (int i = 0; i < m_ToggleList.Count; ++i)
			{
				Toggle toggle = m_ToggleList[i];
				toggle.isOn = i == index;
			}
		}

		/// <summary>
		/// 更新选项
		/// </summary>
		private void UpdateToggle()
		{
			if (m_TogglePool == null
				|| m_ToggleList.Count == content.childCount)
			{
				return;
			}

			m_ChildList.Clear();
			m_TogglePool.Clear(false);
			m_ToggleList.Clear();

			for (int i = 0; i < content.childCount; ++i)
			{
				m_ChildList.Add(content.GetChild(i));
				Toggle toggle = m_TogglePool.Get() as Toggle;
				toggle.transform.SetSiblingIndex(i);
				m_ToggleList.Add(toggle);
			}

			Page(page);
		}

		private void UpdatePosition()
		{
			if (isDraging)
			{
				return;
			}

			if (m_Immediate)
			{
				horizontalNormalizedPosition = m_TargetPosition;
				m_Immediate = false;
			}
			else
			{
				horizontalNormalizedPosition = Mathf.Lerp(horizontalNormalizedPosition, m_TargetPosition, elasticity);
			}
		}

		private object OnToggleConstruct(object obj)
		{
			Toggle toggle = Instantiate(m_ToggleAsset, transform);
			toggle.gameObject.SetActive(false);

			return toggle;
		}

		private void OnToggleDestroy(object obj, object o = null)
		{
			Toggle toggle = obj as Toggle;
			Destroy(toggle);
		}

		private void OnToggleEnabled(object obj, object o = null)
		{
			Toggle toggle = obj as Toggle;
			toggle.transform.SetParent(m_ToggleGroup.transform, false);
			toggle.gameObject.SetActive(true);
		}

		private void OnToggleDisabled(object obj, object o = null)
		{
			Toggle toggle = obj as Toggle;
			toggle.transform.SetParent(transform, false);
			toggle.gameObject.SetActive(false);
		}
	}
}