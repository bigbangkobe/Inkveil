using Framework;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SignPanel : BaseUI
{
    [SerializeField] private List<SignToggle> mSignDayList = new List<SignToggle>();

    private const string LAST_SIGN_DATE_KEY = "LastSignDate";
    private const string SIGN_PROGRESS_KEY = "SignProgress";

    private DateTime lastSignDate;
    private int signProgress; // ��ǰǩ�����ȣ�0~7����ʾ����ɶ��ٸ���
    private Dictionary<int, SignRewardInfo> m_SignRewardInfoDic = new Dictionary<int, SignRewardInfo>();

    protected override void OnAwake()
    {
        base.OnAwake();
        InitConfig();
        LoadData();

        if (IsNewCycle())
        {
            Debug.Log("���������ڣ�����ǩ������");
            ResetSignProgress();
        }

        UpdateSignUI();
    }

    private async void InitConfig()
    {
        string signInfoAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.signInfo)).text;

        if (signInfoAsset == null)
        {
            Debug.LogError($"�����ļ�����ʧ�ܣ�{ConfigDefine.signInfo}");
            return;
        }

        var signInfoList = JsonMapper.ToObject<List<SignRewardInfo>>(signInfoAsset);
        if (signInfoList == null || signInfoList.Count == 0)
        {
            Debug.LogError("�������ݽ���ʧ��");
            return;
        }

        for (int i = 0; i < signInfoList.Count; i++)
        {
            // ��������
            SignRewardInfo info = signInfoList[i];
            m_SignRewardInfoDic[info.signID] = info;
            mSignDayList[i].mSignRewardInfo = info;
        }
    }

    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        UpdateSignUI();
    }

    private void LoadData()
    {
        string dateStr = PlayerPrefs.GetString(LAST_SIGN_DATE_KEY, "");
        if (!string.IsNullOrEmpty(dateStr))
        {
            DateTime.TryParse(dateStr, out lastSignDate);
        }
        else
        {
            lastSignDate = DateTime.MinValue;
        }

        signProgress = PlayerPrefs.GetInt(SIGN_PROGRESS_KEY, 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(LAST_SIGN_DATE_KEY, DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt(SIGN_PROGRESS_KEY, signProgress);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �ж��Ƿ���Ҫ���ã�����7�����ã�
    /// </summary>
    private bool IsNewCycle()
    {
        if (lastSignDate == DateTime.MinValue) return false;

        TimeSpan diff = DateTime.Now.Date - lastSignDate.Date;

        // ����ϴ�ǩ���Ѿ���7��ǰ����磬������
        return signProgress >= 7 || diff.TotalDays >= 7;
    }

    /// <summary>
    /// ����ǩ������
    /// </summary>
    private void ResetSignProgress()
    {
        signProgress = 0;
        lastSignDate = DateTime.MinValue;

        foreach (var signToggle in mSignDayList)
        {
            signToggle.SetGetState(false);
            signToggle.SetActiveState(false);
        }

        if (mSignDayList.Count > 0)
        {
            mSignDayList[0].SetActiveState(true);
        }

        SaveData();
    }

    /// <summary>
    /// ˢ�����и��ӵ���ʾ
    /// </summary>
    private void UpdateSignUI()
    {
        for (int i = 0; i < mSignDayList.Count; i++)
        {
            var signToggle = mSignDayList[i];
            signToggle.mPropType = i + 1;
            signToggle.OnClaimReward = OnRewardClaimed;
            Debug.Log("signProgress:" + signProgress);
            bool isGet = i < signProgress;
            bool isActive = i == signProgress;
            Debug.Log("isGet:" + isGet);

            signToggle.SetGetState(isGet);
            signToggle.SetActiveState(isActive);
        }
    }

    /// <summary>
    /// ������ȡ��������
    /// </summary>
    public void OnRewardClaimed(int propType)
    {
        int todayIndex = signProgress + 1;

        if (propType != todayIndex)
        {
            Debug.LogWarning($"ֻ����ȡ���콱������ {todayIndex} �죩�����������죡");
            return;
        }

        // �������Ƿ��Ѿ�ǩ��������ֹһ�����ظ������
        if (lastSignDate.Date == DateTime.Now.Date)
        {
            Debug.LogWarning("�����Ѿ�ǩ�����������ظ���ȡ��");
            return;
        }

        Debug.Log($"��ȡ�ɹ����� {propType} �콱��");

        // ����״̬
        signProgress++;
        lastSignDate = DateTime.Now;

        SaveData();
        UpdateSignUI();

        // TODO��������Ե�����Ľ��������߼������磺
        // RewardManager.GiveReward(propType);
        SignToggle signToggle = mSignDayList[propType - 1];
        PropertyInfo info = PropertyDispositionManager.instance.GetPropertyById(signToggle.mSignRewardInfo.propType);
        info.number = signToggle.mSignRewardInfo.count;
        BagManager.instance.AddItem(info);
    }

    /// <summary>
    /// �ⲿ�����ã�ǿ������
    /// </summary>
    public void ForceReset()
    {
        PlayerPrefs.DeleteKey(LAST_SIGN_DATE_KEY);
        PlayerPrefs.DeleteKey(SIGN_PROGRESS_KEY);

        ResetSignProgress();
        UpdateSignUI();
    }
}

public class SignRewardInfo
{
    public int signID;
    public int propType;  // ��Ӧ propertyID
    public int signType;  // �ڼ���
    public int count;     // ����
}
