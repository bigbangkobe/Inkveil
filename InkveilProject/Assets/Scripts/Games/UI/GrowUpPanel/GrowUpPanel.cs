using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowUpPanel : BaseUI
{
    [SerializeField] Toggle mFuShenBtn;
    [SerializeField] Toggle mWuQiBtn;
    [SerializeField] WeaponPanel mWeaponPanel;
    [SerializeField] GoldPanel mGoldPanel;
    [SerializeField] Button mBackBtn;


    /// <summary>
    /// ��ʼ������
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        mWeaponPanel.gameObject.SetActive(false);
        mGoldPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// ��ʾ���溯��
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        mFuShenBtn.onValueChanged.AddListener(OnFuShenButtonClick);
        mWuQiBtn.onValueChanged.AddListener(OnWuQiButtonClick);
        mBackBtn.onClick.AddListener(OnBackButtonClick);
    }


    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        mFuShenBtn.onValueChanged.RemoveListener(OnFuShenButtonClick);
        mWuQiBtn.onValueChanged.RemoveListener(OnWuQiButtonClick);
        mBackBtn.onClick.RemoveListener(OnBackButtonClick);
    }

    /// <summary>
    /// �������ť
    /// </summary>
    public void OnFuShenButtonClick(bool isOn)
    {
        mGoldPanel.gameObject.SetActive(isOn);
    }

    /// <summary>
    /// ���������ť
    /// </summary>
    public void OnWuQiButtonClick(bool isOn)
    {
        mWeaponPanel.gameObject.SetActive(isOn);
    }

    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}
