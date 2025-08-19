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
    [SerializeField] Text mSuipianText;
    [SerializeField] List<Toggle> mStarToggleList = new List<Toggle>();
    //伤害
    [SerializeField] Text mDemageText;
    //大招伤害
    [SerializeField] Text mBigDemageText;
    //持续时间
    [SerializeField] Text mTimeText;

    private GodInfo mCurGodInfo;

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
        mCurGodInfo = GodDispositionManager.instance.curGod;
        OnUpdateLevelUI();
        switch (mCurGodInfo.godName)
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
    /// 更新神将等级UI
    /// </summary>
    private void OnUpdateLevelUI()
    {
        //更新当前碎片以及下一等级所需要的碎片
        BagItemInfo bagItemInfo =  BagManager.instance.GetItem(mCurGodInfo.propertyID);
        int curSuipianNum = 0;
        if (bagItemInfo != null)
        {
            curSuipianNum = bagItemInfo.propertyInfo.number;
        }
        mSuipianText.text = curSuipianNum + "/" + mCurGodInfo.fragRequire;
        //如果当前碎片数量小于升级到下一个等级所需要的数量，按钮不点击 
        if (curSuipianNum >= mCurGodInfo.fragRequire)
        {
            mLevelUpBtn.interactable = true;
        }
        else
        {
            mLevelUpBtn.interactable = false;
        }
        for (int i = 0; i < mStarToggleList.Count; i++)
        {
            Toggle startToggle = mStarToggleList[i];
            if (i + 1 <= mCurGodInfo.level)
            {
                startToggle.isOn = true;
            }
            else
            {
                startToggle.isOn = false;
            }
        }
        mDemageText.text = mCurGodInfo.baseAttack.ToString();
        mBigDemageText.text = mCurGodInfo.skillDamageMulti.ToString();
        mTimeText.text = mCurGodInfo.maxDuration.ToString();
    }

    /// <summary>
    /// 点击杨戬
    /// </summary>
    /// <param name="isOn"></param>
    public void OnYJToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            int curNeZaLevel = PlayerPrefs.GetInt(mYJInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mYJInfo.mName, curNeZaLevel);
            mName.text = mYJInfo.mName;
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mYJInfo.mSprite;
            //设置当前神将
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //更新UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mGYInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mGYInfo.mName, curLevel);
            mName.text = mGYInfo.mName;
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mGYInfo.mSprite;
            //设置当前神将
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //更新UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mNZInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mNZInfo.mName, curLevel);
            mName.text = mNZInfo.mName;
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mNZInfo.mSprite;
            //设置当前神将
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //更新UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mWKInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mWKInfo.mName, curLevel);
            mName.text = mWKInfo.mName;
            //mMessage.text = "伤害：" + mDemage + "   大招伤害：" + mBigDemage
            //    + "\n 持续时间：" + mTime;
            mIcon.sprite = mWKInfo.mSprite;
            //设置当前神将
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //更新UI
            OnUpdateLevelUI();
        }
    }

    /// <summary>
    /// 提升星级
    /// </summary>
    public void OnLevelUpButtonClick()
    {
        //进行提供升级
        mCurGodInfo.level++;
        //扣除碎片
        BagItemInfo bagItemInfo = BagManager.instance.GetItem(mCurGodInfo.propertyID);
        if (bagItemInfo != null) {
            BagManager.instance.UseItem(bagItemInfo.propertyInfo.propertyID, mCurGodInfo.fragRequire);
        }
        else
        {
            Debug.LogError("错误，不存在碎片道具");
        }
        //设置下一个等级的神将配置
        GodDispositionManager.instance.SetCurGod(mCurGodInfo.godName, mCurGodInfo.level);
        //重置获得当前神将的数据
        mCurGodInfo = GodDispositionManager.instance.curGod;
        //还需要做把神将的数据等级保留下来
        PlayerPrefs.SetInt(mCurGodInfo.godName, mCurGodInfo.level);
        //刷新下个等级需要的碎片
        OnUpdateLevelUI();
    }
}
