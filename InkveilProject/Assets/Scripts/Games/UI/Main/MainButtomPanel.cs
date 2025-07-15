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
    /// 初始化函数
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// 显示界面函数
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }
}
