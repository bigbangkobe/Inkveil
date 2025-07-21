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
    private int signProgress; // 当前签到进度：0~7（表示已完成多少个）
    private Dictionary<int, SignRewardInfo> m_SignRewardInfoDic = new Dictionary<int, SignRewardInfo>();

    protected override void OnAwake()
    {
        base.OnAwake();
        InitConfig();
        LoadData();

        if (IsNewCycle())
        {
            Debug.Log("进入新周期，重置签到进度");
            ResetSignProgress();
        }

        UpdateSignUI();
    }

    private async void InitConfig()
    {
        string signInfoAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.signInfo)).text;

        if (signInfoAsset == null)
        {
            Debug.LogError($"配置文件加载失败：{ConfigDefine.signInfo}");
            return;
        }

        var signInfoList = JsonMapper.ToObject<List<SignRewardInfo>>(signInfoAsset);
        if (signInfoList == null || signInfoList.Count == 0)
        {
            Debug.LogError("配置数据解析失败");
            return;
        }

        for (int i = 0; i < signInfoList.Count; i++)
        {
            // 建立索引
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
    /// 判断是否需要重置（超过7天重置）
    /// </summary>
    private bool IsNewCycle()
    {
        if (lastSignDate == DateTime.MinValue) return false;

        TimeSpan diff = DateTime.Now.Date - lastSignDate.Date;

        // 如果上次签到已经是7天前或更早，就重置
        return signProgress >= 7 || diff.TotalDays >= 7;
    }

    /// <summary>
    /// 重置签到进度
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
    /// 刷新所有格子的显示
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
    /// 处理领取奖励请求
    /// </summary>
    public void OnRewardClaimed(int propType)
    {
        int todayIndex = signProgress + 1;

        if (propType != todayIndex)
        {
            Debug.LogWarning($"只能领取当天奖励（第 {todayIndex} 天），不能跳着领！");
            return;
        }

        // 检查今天是否已经签过到（防止一天内重复点击）
        if (lastSignDate.Date == DateTime.Now.Date)
        {
            Debug.LogWarning("今天已经签过到，不能重复领取！");
            return;
        }

        Debug.Log($"领取成功：第 {propType} 天奖励");

        // 更新状态
        signProgress++;
        lastSignDate = DateTime.Now;

        SaveData();
        UpdateSignUI();

        // TODO：这里可以调用你的奖励发放逻辑，例如：
        // RewardManager.GiveReward(propType);
        SignToggle signToggle = mSignDayList[propType - 1];
        PropertyInfo info = PropertyDispositionManager.instance.GetPropertyById(signToggle.mSignRewardInfo.propType);
        info.number = signToggle.mSignRewardInfo.count;
        BagManager.instance.AddItem(info);
    }

    /// <summary>
    /// 外部调试用：强制重置
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
    public int propType;  // 对应 propertyID
    public int signType;  // 第几天
    public int count;     // 数量
}
