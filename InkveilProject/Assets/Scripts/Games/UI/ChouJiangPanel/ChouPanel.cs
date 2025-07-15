using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChouPanel : BaseUI
{


    /// <summary>
    /// 刷新按钮
    /// </summary>
    [SerializeField] Button mRefleshBtn;

    /// <summary>
    /// 刷新按钮
    /// </summary>
    [SerializeField] Button mFreehBtn;

    /// <summary>
    /// 是否本次免费
    /// </summary>
    public bool mIsFree = true;

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
        UpdateView();
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }


    public void UpdateView()
    {
        if (mIsFree)
        {
            mFreehBtn.gameObject.SetActive(true);
        }
        else
        {
            mFreehBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新按钮
    /// </summary>
    public void OnRefleshButtonClick()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
