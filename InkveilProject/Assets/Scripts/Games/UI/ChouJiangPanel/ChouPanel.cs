using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChouPanel : BaseUI
{


    /// <summary>
    /// ˢ�°�ť
    /// </summary>
    [SerializeField] Button mRefleshBtn;

    /// <summary>
    /// ˢ�°�ť
    /// </summary>
    [SerializeField] Button mFreehBtn;

    /// <summary>
    /// �Ƿ񱾴����
    /// </summary>
    public bool mIsFree = true;

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
        UpdateView();
    }

    /// <summary>
    /// ���ؽ��溯��
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
    /// ˢ�°�ť
    /// </summary>
    public void OnRefleshButtonClick()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
