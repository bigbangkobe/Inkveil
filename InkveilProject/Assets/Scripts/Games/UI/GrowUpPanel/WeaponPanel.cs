using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : BaseUI
{
    /// <summary>
    /// ������
    /// </summary>
    [SerializeField] WeaponSlider mYTSJWeaponSlider;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] WeaponSlider mTLBDWeaponSlider;

    /// <summary>
    /// ������ǹ
    /// </summary>
    [SerializeField] WeaponSlider mBWSQWeaponSlider;

    /// <summary>
    /// ��ɨǧ��
    /// </summary>
    [SerializeField] WeaponSlider mHSQJWeaponSlider;

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
