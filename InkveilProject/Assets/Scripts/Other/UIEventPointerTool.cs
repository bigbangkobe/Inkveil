using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI事件指针工具
/// </summary>
public class UIEventPointerTool : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,IPointerEnterHandler ,IPointerExitHandler
{
    #region UI状态类型
    /// <summary>
    /// UI事件状态类型
    /// </summary>
    public enum PointState
    {
        None,
        Down,
        Up,
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Enter,
        Exit
    }
    #endregion

    #region 状态属性
    /// <summary>
    /// 是否进入
    /// </summary>
    private bool isEnter;
    #endregion

    #region 事件
    private Dictionary<PointState, Action> eventDic = new Dictionary<PointState, Action>();
    private Dictionary<PointState, Action<PointerEventData>> eventDataDic = new Dictionary<PointState, Action<PointerEventData>>();
    #endregion

    #region 外部调用接口
    /// <summary>
    /// 检查图像并挂载脚本
    /// </summary>
    /// <param name="go">检测对象</param>
    /// <returns></returns>
    public static UIEventPointerTool GetUIEvent(GameObject go)
    {
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic) graphic.raycastTarget = true;

        UIEventPointerTool uiEvent = go.GetComponent<UIEventPointerTool>();
        if (!uiEvent) uiEvent = go.AddComponent<UIEventPointerTool>();

        return uiEvent;
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="go">被注册对象</param>
    /// <param name="action">回调函数</param>
    public static void AddEvent(GameObject go, PointState state, Action action)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        //如果没有则添加，有则替换
        if (!uIEvent.eventDic.ContainsKey(state))
        {
            uIEvent.eventDic.Add(state, action);
        }
        else
        {
            uIEvent.eventDic[state] += action;
        }
       
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="go">被注册对象</param>
    /// <param name="action">回调函数</param>
    public static void AddEvent(GameObject go, PointState state, Action<PointerEventData> action)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        //如果没有则添加，有则替换
        uIEvent.eventDataDic[state] = action;
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="go">被注册对象</param>
    /// <param name="state">指定状态</param>
    /// <param name="isParameter">是否带参</param>
    public static void RemoveEvent(GameObject go, PointState state,bool isParameter = false)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        if (isParameter)
        {
            if (uIEvent.eventDataDic.ContainsKey(state))
            {
                uIEvent.eventDataDic.Remove(state);
            }
        }
        else
        {
            if (uIEvent.eventDic.ContainsKey(state))
            {
                uIEvent.eventDic.Remove(state);
            }
        }
    }

    /// <summary>
    /// 移除所有事件
    /// </summary>
    /// <param name="go">被注册对象</param>
    public static void RemoveAllEvent(GameObject go)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        foreach (var key in uIEvent.eventDic.Keys)
        {
            uIEvent.eventDic.Remove(key);
        }

        foreach (var key in uIEvent.eventDataDic.Keys)
        {
            uIEvent.eventDataDic.Remove(key);
        }
    }
    #endregion

    #region 事件监听实现

    /// <summary>
    /// 事件处理
    /// </summary>
    /// <param name="state"></param>
    /// <param name="eventData"></param>
    private void OnEventHandler(PointState state, PointerEventData eventData)
    {
        if (eventDic.ContainsKey(state))
            eventDic[state]?.Invoke();

        if (eventDataDic.ContainsKey(state))
            eventDataDic[state]?.Invoke(eventData);
    }

    /// <summary>
    /// 按下监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        OnEventHandler(PointState.Down, eventData);
    }

    /// <summary>
    /// 抬起监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isEnter)
        {
            OnEventHandler(PointState.Up, eventData);
        } 
    }

    /// <summary>
    /// 点击监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEnter)
        {
            OnEventHandler(PointState.Click, eventData);
        }   
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnEventHandler(PointState.BeginDrag, eventData);
    }

    /// <summary>
    /// 拖拽监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        OnEventHandler(PointState.Drag, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEventHandler(PointState.EndDrag, eventData);
    }

    /// <summary>
    /// 进入监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;

        OnEventHandler(PointState.Enter, eventData);
    }

    /// <summary>
    /// 退出监听
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;

        OnEventHandler(PointState.Exit, eventData);
    }
    #endregion
}