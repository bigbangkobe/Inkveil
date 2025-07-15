using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using System;

public class BagPanel : BaseUI
{
    [SerializeField] Button mBackBtn;

    private BagItemUI[] mList;

    private List<BagItemInfo> bagItemInfos;

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
        mList = GetComponentsInChildren<BagItemUI>();

        OninitItems();

        mBackBtn.onClick.AddListener(OnBackButtonClick);
    }

    private void OninitItems()
    {
        bagItemInfos = BagManager.instance.ItemInfos;

        for (int i = 0; i < mList.Length; i++) 
        {
            if (i < bagItemInfos.Count) 
            {
                mList[i].OnInit(bagItemInfos[i]);
            }
            else
            {
                mList[i].UnInit();
            }
        }
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
        mBackBtn.onClick.RemoveListener(OnBackButtonClick);
        base.OnHideDisable();
    }

    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}
