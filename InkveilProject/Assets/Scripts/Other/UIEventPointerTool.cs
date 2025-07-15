using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI�¼�ָ�빤��
/// </summary>
public class UIEventPointerTool : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,IPointerEnterHandler ,IPointerExitHandler
{
    #region UI״̬����
    /// <summary>
    /// UI�¼�״̬����
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

    #region ״̬����
    /// <summary>
    /// �Ƿ����
    /// </summary>
    private bool isEnter;
    #endregion

    #region �¼�
    private Dictionary<PointState, Action> eventDic = new Dictionary<PointState, Action>();
    private Dictionary<PointState, Action<PointerEventData>> eventDataDic = new Dictionary<PointState, Action<PointerEventData>>();
    #endregion

    #region �ⲿ���ýӿ�
    /// <summary>
    /// ���ͼ�񲢹��ؽű�
    /// </summary>
    /// <param name="go">������</param>
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
    /// ����¼�
    /// </summary>
    /// <param name="go">��ע�����</param>
    /// <param name="action">�ص�����</param>
    public static void AddEvent(GameObject go, PointState state, Action action)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        //���û������ӣ������滻
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
    /// ����¼�
    /// </summary>
    /// <param name="go">��ע�����</param>
    /// <param name="action">�ص�����</param>
    public static void AddEvent(GameObject go, PointState state, Action<PointerEventData> action)
    {
        UIEventPointerTool uIEvent = GetUIEvent(go);

        if (!uIEvent) return;

        //���û������ӣ������滻
        uIEvent.eventDataDic[state] = action;
    }

    /// <summary>
    /// �Ƴ��¼�
    /// </summary>
    /// <param name="go">��ע�����</param>
    /// <param name="state">ָ��״̬</param>
    /// <param name="isParameter">�Ƿ����</param>
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
    /// �Ƴ������¼�
    /// </summary>
    /// <param name="go">��ע�����</param>
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

    #region �¼�����ʵ��

    /// <summary>
    /// �¼�����
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
    /// ���¼���
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        OnEventHandler(PointState.Down, eventData);
    }

    /// <summary>
    /// ̧�����
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
    /// �������
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
    /// ��ʼ��ק
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnEventHandler(PointState.BeginDrag, eventData);
    }

    /// <summary>
    /// ��ק����
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
    /// �������
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;

        OnEventHandler(PointState.Enter, eventData);
    }

    /// <summary>
    /// �˳�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;

        OnEventHandler(PointState.Exit, eventData);
    }
    #endregion
}