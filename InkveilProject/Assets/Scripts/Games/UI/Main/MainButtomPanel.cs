using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainButtomPanel : BaseUI
{
    [SerializeField]
    private ToggleGroup m_ToogleGroup;
    /// <summary>
    /// ��ʼ������
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// ��ʾ���溯��
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }
}
