using System.Collections.Generic;
using Framework;
using LitJson;
using System.Threading.Tasks;
using UnityEngine;

public class StageRewardsDispositionManager : Singleton<StageRewardsDispositionManager>
{
    private Dictionary<int, List<StageRewardsInfo>> m_IdStageRewardsDict = new Dictionary<int, List<StageRewardsInfo>>();
    private List<List<StageRewardsInfo>> stageRewardList = new List<List<StageRewardsInfo>>();

    private bool m_IsInitialized = false;
    private int m_CurOpenLevel;
    private int m_UnlockLevel;

    public int CurOpenLevel { get => m_CurOpenLevel; set => m_CurOpenLevel = value; }
    public List<StageRewardsInfo> CurStageRewardsInfo =>
        m_IdStageRewardsDict.TryGetValue(m_CurOpenLevel + 1, out var list) ? list : new List<StageRewardsInfo>();
    public int SumLevel => m_IdStageRewardsDict.Count;
    public int UnlockLevel { get => m_UnlockLevel; set => m_UnlockLevel = value; }

    /// <summary>
    /// 异步初始化关卡奖励配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        // 从本地读取存档
        string saved = PlayerPrefs.GetString("StageRewards");
        m_UnlockLevel = PlayerPrefs.GetInt("StageRewards Level", 0);
        m_CurOpenLevel = m_UnlockLevel;

        // 如果有保存数据，直接解析
        if (!string.IsNullOrEmpty(saved))
        {
            stageRewardList = JsonMapper.ToObject<List<List<StageRewardsInfo>>>(saved) ?? new List<List<StageRewardsInfo>>();
        }
        else
        {
            // 异步加载默认配置
            var handle1 = ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsCommonInfo);
            var handle2 = ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsDifficultyInfo);
            var handle3 = ResourceService.LoadAsync<TextAsset>(ConfigDefine.stageRewardsAbyssInfo);

            TextAsset asset1 = await handle1.Task;
            TextAsset asset2 = await handle2.Task;
            TextAsset asset3 = await handle3.Task;

            if (asset1 == null || asset2 == null || asset3 == null)
            {
                Debug.LogError("关卡奖励配置文件加载失败");
                return;
            }

            stageRewardList = new List<List<StageRewardsInfo>>
            {
                JsonMapper.ToObject<List<StageRewardsInfo>>(asset1.text) ?? new List<StageRewardsInfo>(),
                JsonMapper.ToObject<List<StageRewardsInfo>>(asset2.text) ?? new List<StageRewardsInfo>(),
                JsonMapper.ToObject<List<StageRewardsInfo>>(asset3.text) ?? new List<StageRewardsInfo>()
            };
        }

        // 建立索引
        foreach (var rewardsList in stageRewardList)
        {
            foreach (var info in rewardsList)
            {
                if (!m_IdStageRewardsDict.TryGetValue(info.stageID, out var list))
                {
                    list = new List<StageRewardsInfo>();
                    m_IdStageRewardsDict[info.stageID] = list;
                }
                list.Add(info);
            }
        }

        m_IsInitialized = true;
        Debug.Log($"关卡奖励配置加载完成，共{m_IdStageRewardsDict.Count}个关卡奖励");
    }

    /// <summary>
    /// 获取指定关卡奖励
    /// </summary>
    public List<StageRewardsInfo> GetStageRewardsInfoByStage(int stage)
    {
        if (!m_IsInitialized) return new List<StageRewardsInfo>();
        return m_IdStageRewardsDict.TryGetValue(stage, out var list) ? list : new List<StageRewardsInfo>();
    }

    /// <summary>
    /// 保存当前奖励进度
    /// </summary>
    public void Save()
    {
        if (!m_IsInitialized) return;
        string data = JsonMapper.ToJson(stageRewardList);
        PlayerPrefs.SetString("StageRewards", data);
        PlayerPrefs.SetInt("StageRewards Level", m_UnlockLevel);
        PlayerPrefs.Save();
        Debug.Log("关卡奖励数据已保存");
    }

    /// <summary>
    /// 标记关卡已通过并解锁下一关
    /// </summary>
    public void PassLevel(int level)
    {
        if (!m_IsInitialized) return;
        if (level == m_UnlockLevel && m_CurOpenLevel < stageRewardList.Count - 1)
        {
            m_UnlockLevel++;
            PlayerPrefs.SetInt("StageRewards Level", m_UnlockLevel);
            Debug.Log($"玩家已解锁第{m_UnlockLevel}关卡");
        }
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        m_IdStageRewardsDict.Clear();
        stageRewardList.Clear();
        m_IsInitialized = false;
    }
}