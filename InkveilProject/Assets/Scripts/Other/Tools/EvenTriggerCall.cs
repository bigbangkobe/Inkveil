using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvenTriggerCall : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerDownHandler,IPointerUpHandler
{
	/// <summary>
	/// 开始拖拽回调事件
	/// </summary>
	public Action<PointerEventData> OnBeginDragCallBack = null;
	public void OnBeginDrag(PointerEventData pointerEventData)
	{
		if (OnBeginDragCallBack != null)
			OnBeginDragCallBack.Invoke(pointerEventData);
	}

	/// <summary>
	/// 拖拽中回调事件
	/// </summary>
	public Action<PointerEventData> OnDragCallBack;
	public void OnDrag(PointerEventData pointerEventData)
	{
		if (OnDragCallBack != null)
			OnDragCallBack.Invoke(pointerEventData);
	}

	/// <summary>
	/// 结束拖拽回调事件
	/// </summary>
	public Action<PointerEventData> OnEndDragCallBack;
	public void OnEndDrag(PointerEventData pointerEventData)
	{
		if (OnEndDragCallBack != null)
			OnEndDragCallBack.Invoke(pointerEventData);
	}

	/// <summary>
	/// 按下回调事件
	/// </summary>
	public Action<PointerEventData> OnPointerDownCallBack;
	public void OnPointerDown(PointerEventData pointerEventData)
	{
		if (OnPointerDownCallBack != null)
			OnPointerDownCallBack.Invoke(pointerEventData);
	}

	/// <summary>
	/// 弹起回调事件
	/// </summary>
	public Action<PointerEventData> OnPointerUpCallBack;
	public void OnPointerUp(PointerEventData pointerEventData)
	{
		if (OnPointerUpCallBack != null)
			OnPointerUpCallBack.Invoke(pointerEventData);
	}
}
