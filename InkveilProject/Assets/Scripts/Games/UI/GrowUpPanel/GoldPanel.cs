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

    //伤害
    public int mDemage;
    //大招伤害
    public int mBigDemage;
    //持续时间
    public int mTime;

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

        switch (GodDispositionManager.instance.curGod.godName)
        {
            case "关羽":
                mGYInfo.mToggle.isOn = true;
                break;
            case "悟空":
                mWKInfo.mToggle.isOn = true;
                break;
            case "杨戬":
                mYJInfo.mToggle.isOn = true;
                break;
            case "哪吒":
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
    /// 隐藏界面函数
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
    /// 点击杨戬
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
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mYJInfo.mSprite;
        }
    }

    /// <summary>
    /// 点击关羽
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
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mGYInfo.mSprite;
        }
    }

    /// <summary>
    /// 点击哪吒
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
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mNZInfo.mSprite;
        }
    }

    /// <summary>
    /// 点击悟空
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
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mWKInfo.mSprite;
        }
    }

    /// <summary>
    /// 提升星级
    /// </summary>
    public void OnLevelUpButtonClick()
    {
        
    }
}
