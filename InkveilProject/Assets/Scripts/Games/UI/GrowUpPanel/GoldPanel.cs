using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldPanel : BaseUI
{
    [SerializeField] Text mName;
    [SerializeField] Image mIcon;
    [SerializeField] HeroInfo mYJInfo;
    [SerializeField] HeroInfo mNZInfo;
    [SerializeField] HeroInfo mWKInfo;
    [SerializeField] HeroInfo mGYInfo;
    [SerializeField] Button mLevelUpBtn;
    [SerializeField] Text mMessage;

    //�˺�
    public int mDemage;
    //�����˺�
    public int mBigDemage;
    //����ʱ��
    public int mTime;

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

        switch (GodDispositionManager.instance.curGod.godName)
        {
            case "����":
                mGYInfo.mToggle.isOn = true;
                break;
            case "���":
                mWKInfo.mToggle.isOn = true;
                break;
            case "���":
                mYJInfo.mToggle.isOn = true;
                break;
            case "��߸":
                mNZInfo.mToggle.isOn = true;
                break;
            default:
                break;
        }


        mYJInfo.mToggle.onValueChanged.AddListener(OnYJToggleValueChanged);
        mNZInfo.mToggle.onValueChanged.AddListener(OnNZToggleValueChanged);
        mWKInfo.mToggle.onValueChanged.AddListener(OnWKToggleValueChanged);
        mGYInfo.mToggle.onValueChanged.AddListener(OnGYToggleValueChanged);
        mLevelUpBtn.onClick.AddListener(OnLevelUpButtonClick);
        //mYJInfo.mToggle.isOn = true;
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        mYJInfo.mToggle.onValueChanged.RemoveListener(OnYJToggleValueChanged);
        mNZInfo.mToggle.onValueChanged.RemoveListener(OnNZToggleValueChanged);
        mWKInfo.mToggle.onValueChanged.RemoveListener(OnWKToggleValueChanged);
        mGYInfo.mToggle.onValueChanged.RemoveListener(OnGYToggleValueChanged);
        mLevelUpBtn.onClick.RemoveListener(OnLevelUpButtonClick);
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="isOn"></param>
    public void OnYJToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GodDispositionManager.instance.SetCurGod(mYJInfo.mName);
            mName.text = mYJInfo.mName;
            mDemage = mYJInfo.mDemage;
            mBigDemage = mYJInfo.mBigDemage;
            mTime = mYJInfo.mTime;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mYJInfo.mSprite;
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="isOn"></param>
    public void OnGYToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GodDispositionManager.instance.SetCurGod(mGYInfo.mName);
            mName.text = mGYInfo.mName;
            mDemage = mGYInfo.mDemage;
            mBigDemage = mGYInfo.mBigDemage;
            mTime = mGYInfo.mTime;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mGYInfo.mSprite;
        }
    }

    /// <summary>
    /// �����߸
    /// </summary>
    /// <param name="isOn"></param>
    public void OnNZToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GodDispositionManager.instance.SetCurGod(mNZInfo.mName);
            mName.text = mNZInfo.mName;
            mDemage = mNZInfo.mDemage;
            mBigDemage = mNZInfo.mBigDemage;
            mTime = mNZInfo.mTime;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mNZInfo.mSprite;
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="isOn"></param>
    public void OnWKToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            GodDispositionManager.instance.SetCurGod(mWKInfo.mName);
            mName.text = mWKInfo.mName;
            mDemage = mWKInfo.mDemage;
            mBigDemage = mWKInfo.mBigDemage;
            mTime = mWKInfo.mTime;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mWKInfo.mSprite;
        }
    }

    /// <summary>
    /// �����Ǽ�
    /// </summary>
    public void OnLevelUpButtonClick()
    {
        
    }
}
