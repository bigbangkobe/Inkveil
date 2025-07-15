using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlider : BaseUI
{
    [SerializeField] Text mName;
    [SerializeField] Image mIconImage;
    [SerializeField] Button mAddBtn;
    [SerializeField] Image mMHTBIcon;
    [SerializeField] TextMeshProUGUI mMHTBTMP;
    [SerializeField] Image mSliderImg;

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
        mAddBtn.onClick.AddListener(OnAddButtonClick);
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        mAddBtn.onClick.RemoveListener(OnAddButtonClick);
        base.OnHideDisable();
    }

    private void OnAddButtonClick()
    {
        mSliderImg.fillAmount += 0.04f;
    }
}
