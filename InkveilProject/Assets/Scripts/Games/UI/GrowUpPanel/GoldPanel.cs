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
    //�˺�
    [SerializeField] Text mDemageText;
    //�����˺�
    [SerializeField] Text mBigDemageText;
    //����ʱ��
    [SerializeField] Text mTimeText;

    private GodInfo mCurGodInfo;

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
        mCurGodInfo = GodDispositionManager.instance.curGod;
        OnUpdateLevelUI();
        switch (mCurGodInfo.godName)
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
    /// �����񽫵ȼ�UI
    /// </summary>
    private void OnUpdateLevelUI()
    {
        //���µ�ǰ��Ƭ�Լ���һ�ȼ�����Ҫ����Ƭ
        BagItemInfo bagItemInfo =  BagManager.instance.GetItem(mCurGodInfo.propertyID);
        int curSuipianNum = 0;
        if (bagItemInfo != null)
        {
            curSuipianNum = bagItemInfo.propertyInfo.number;
        }
        mSuipianText.text = curSuipianNum + "/" + mCurGodInfo.fragRequire;
        //�����ǰ��Ƭ����С����������һ���ȼ�����Ҫ����������ť����� 
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
    /// ������
    /// </summary>
    /// <param name="isOn"></param>
    public void OnYJToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            int curNeZaLevel = PlayerPrefs.GetInt(mYJInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mYJInfo.mName, curNeZaLevel);
            mName.text = mYJInfo.mName;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mYJInfo.mSprite;
            //���õ�ǰ��
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //����UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mGYInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mGYInfo.mName, curLevel);
            mName.text = mGYInfo.mName;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mGYInfo.mSprite;
            //���õ�ǰ��
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //����UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mNZInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mNZInfo.mName, curLevel);
            mName.text = mNZInfo.mName;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mNZInfo.mSprite;
            //���õ�ǰ��
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //����UI
            OnUpdateLevelUI();
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
            int curLevel = PlayerPrefs.GetInt(mWKInfo.mName, 1);
            GodDispositionManager.instance.SetCurGod(mWKInfo.mName, curLevel);
            mName.text = mWKInfo.mName;
            //mMessage.text = "�˺���" + mDemage + "   �����˺���" + mBigDemage
            //    + "\n ����ʱ�䣺" + mTime;
            mIcon.sprite = mWKInfo.mSprite;
            //���õ�ǰ��
            mCurGodInfo = GodDispositionManager.instance.curGod;
            //����UI
            OnUpdateLevelUI();
        }
    }

    /// <summary>
    /// �����Ǽ�
    /// </summary>
    public void OnLevelUpButtonClick()
    {
        //�����ṩ����
        mCurGodInfo.level++;
        //�۳���Ƭ
        BagItemInfo bagItemInfo = BagManager.instance.GetItem(mCurGodInfo.propertyID);
        if (bagItemInfo != null) {
            BagManager.instance.UseItem(bagItemInfo.propertyInfo.propertyID, mCurGodInfo.fragRequire);
        }
        else
        {
            Debug.LogError("���󣬲�������Ƭ����");
        }
        //������һ���ȼ���������
        GodDispositionManager.instance.SetCurGod(mCurGodInfo.godName, mCurGodInfo.level);
        //���û�õ�ǰ�񽫵�����
        mCurGodInfo = GodDispositionManager.instance.curGod;
        //����Ҫ�����񽫵����ݵȼ���������
        PlayerPrefs.SetInt(mCurGodInfo.godName, mCurGodInfo.level);
        //ˢ���¸��ȼ���Ҫ����Ƭ
        OnUpdateLevelUI();
    }
}
