using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : BaseUI
{
    /// <summary>
    /// 背景图片
    /// </summary>
    [SerializeField] Image mBgImage;

    [SerializeField] List<Sprite> mBgSpList = new List<Sprite>();

    /// <summary>
    /// 物品图标
    /// </summary>
    [SerializeField] Image mIconImage;

    [SerializeField] List<Sprite> mIconSpList = new List<Sprite>();


    /// <summary>
    /// 数量
    /// </summary>
    [SerializeField] TextMeshProUGUI mTMP;

    public RewardCardInfo mRewardCardInfo = new RewardCardInfo();

    /// <summary>
    /// 初始化函数
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        mRewardCardInfo.mCardId = 0;
        mRewardCardInfo.mCardIconId = 0;
        mRewardCardInfo.mCardNum = 1;
    }

    internal void OnInit(PropertyInfo propertyInfo)
    {
       
        mRewardCardInfo.mCardId = propertyInfo.propertyGrade - 1;
        mRewardCardInfo.mCardIconId = propertyInfo.propertyID - 1;
        mRewardCardInfo.mCardNum = propertyInfo.number;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示界面函数
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        print("mRewardCardInfo.mCardId:" + mRewardCardInfo.mCardId);
        mBgImage.sprite = mBgSpList[mRewardCardInfo.mCardId];
        mIconImage.sprite = mIconSpList[mRewardCardInfo.mCardIconId];
        mTMP.text = mRewardCardInfo.mCardNum.ToString();
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }
}
