using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : BaseUI
{
    /// <summary>
    /// 倚天神剑
    /// </summary>
    [SerializeField] WeaponSlider mYTSJWeaponSlider;

    /// <summary>
    /// 屠龙宝刀
    /// </summary>
    [SerializeField] WeaponSlider mTLBDWeaponSlider;

    /// <summary>
    /// 霸王神枪
    /// </summary>
    [SerializeField] WeaponSlider mBWSQWeaponSlider;

    /// <summary>
    /// 横扫千军
    /// </summary>
    [SerializeField] WeaponSlider mHSQJWeaponSlider;

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
