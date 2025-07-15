using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanelUI : MonoBehaviour
{
    [SerializeField] private Button m_LeftBtn;
    [SerializeField] private Button m_RightBtn;

    private void Start()
    {
        UIEventPointerTool.AddEvent(m_LeftBtn.gameObject, UIEventPointerTool.PointState.Down,PlayerController.instance.OnLeftButtonDown);
        UIEventPointerTool.AddEvent(m_LeftBtn.gameObject, UIEventPointerTool.PointState.Up,PlayerController.instance.OnLeftButtonUp);

        UIEventPointerTool.AddEvent(m_RightBtn.gameObject, UIEventPointerTool.PointState.Down, PlayerController.instance.OnRightButtonDown);
        UIEventPointerTool.AddEvent(m_RightBtn.gameObject, UIEventPointerTool.PointState.Up, PlayerController.instance.OnRightButtonUp);
    }
}