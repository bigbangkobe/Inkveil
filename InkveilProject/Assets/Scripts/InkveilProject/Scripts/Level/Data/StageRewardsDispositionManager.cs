using Framework;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StageRewardsDispositionManager : Singleton<StageRewardsDispositionManager>
{
    Dictionary<int, List<StageRewardsInfo>> m_IdStageRewardsDict = new Dictionary<int, List<StageRewardsInfo>>();
    List<List<StageRewardsInfo>> stageRewardList = new List<List<StageRewardsInfo>>();

    private bool m_IsInitialized = false;
    private int m_CurOpenLevel;
    private int m_UnlockLevel;
    private List<StageRewardsInfo> m_CurStageRewardsInfo;

    public int CurOpenLevel { get => m_CurOpenLevel; set => m_CurOpenLevel = value; }
    public List<StageRewardsInfo> CurStageRewardsInfo => m_IdStageRewardsDict[m_CurOpenLevel + 1];
    public int SumLevel => m_IdStageRewardsDict.Count;

    public int UnlockLevel { get => m_UnlockLevel; set => m_UnlockLevel = value; }

    /// <summary>
    /// 初始化关卡配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;
        string stageRewardsStr = PlayerPrefs.GetString("StageRewards");
        //Test by cpGo
        UnlockLevel = PlayerPrefs.GetInt("StageRewards Level");
        m_CurOpenLevel = UnlockLevel;
        List<List<StageRewardsInfo>> stageRewards = new List<List<StageRewardsInfo>>();
        if (string.IsNullOrEmpty(stageRewardsStr))
        {
            string str1 = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsCommonInfo)).text;
            string str2 = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsDifficultyInfo)).text;
            string str3 = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsAbyssInfo)).text;

            stageRewards.Add(JsonMapper.ToObject<List<StageRewardsInfo>>(str1));
            stageRewards.Add(JsonMapper.ToObject<List<StageRewardsInfo>>(str2));
            stageRewards.Add(JsonMapper.ToObject<List<StageRewardsInfo>>(str3));
        }
        else
        {
            stageRewards = JsonMapper.ToObject<List<List<StageRewardsInfo>>>(stageRewardsStr);
        }


        stageRewardList = stageRewards;
        if (stageRewardList == null || stageRewardList.Count == 0)
        {
            Debug.LogError("关卡配置数据解析失败");
            return;
        }

        foreach (List<StageRewardsInfo> stageRewardList in stageRewards)
        {

            foreach (var item in stageRewardList)
            {
                // 数据校验
                if (!m_IdStageRewardsDict.ContainsKey(item.stageID))
                {
                    m_IdStageRewardsDict[item.stageID] = new List<StageRewardsInfo>();
                }

                // 建立索引
                m_IdStageRewardsDict[item.stageID].Add(item);
            }

        }
        string data = JsonMapper.ToJson(stageRewardList);
        Debug.Log($"关卡奖励数据：{data}");
        m_IsInitialized = true;
        try
        {
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"配置初始化异常：{ex}");
        }
    }

    public List<StageRewardsInfo> GetStageRewardsInfoByStageRewards(int stageRewards)
    {
        if (!m_IsInitialized) return null;
        return m_IdStageRewardsDict.TryGetValue(stageRewards, out var stageRewardsInfo) ? stageRewardsInfo : null;
    }

    public void Save()
    {
        string data = JsonMapper.ToJson(stageRewardList);
        Debug.Log($"关卡奖励数据：{data}");
        PlayerPrefs.SetString("StageRewards", data);

        Debug.Log("保存关卡记录表");
    }

    public void PassLevel(int level)
    {
        if (!m_IsInitialized) return;
        if (level == UnlockLevel && CurOpenLevel < stageRewardList.Count - 1)
        {
            UnlockLevel++;
            PlayerPrefs.SetInt("StageRewards Level", UnlockLevel);
            Debug.Log($"玩家已解锁{CurOpenLevel}关卡");
        }
    }
}