using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using LitJson;
using UnityEngine;

public class LotteryDispositionManager : Singleton<LotteryDispositionManager>
{
    // 异步加载标志
    private bool m_IsInitialized = false;

    // 配置数据
    public List<LotteryProbabilityInfo> LotteryList { get; private set; } = new List<LotteryProbabilityInfo>();
    public Dictionary<int, List<LotteryCardPools>> LotteryCardPools { get; private set; } = new Dictionary<int, List<LotteryCardPools>>();

    /// <summary>
    /// 异步初始化抽奖配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        try
        {
            // 异步加载概率和卡池配置
            var probHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.lottery_probability_info);
            var poolHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.lottery_CardPools);

            TextAsset probAsset = await probHandle.Task;
            TextAsset poolAsset = await poolHandle.Task;

            if (probAsset == null)
            {
                Debug.LogError($"抽奖概率配置加载失败：{ConfigDefine.lottery_probability_info}");
                return;
            }
            if (poolAsset == null)
            {
                Debug.LogError($"抽奖卡池配置加载失败：{ConfigDefine.lottery_CardPools}");
                return;
            }

            // 解析配置
            LotteryList = JsonMapper.ToObject<List<LotteryProbabilityInfo>>(probAsset.text)
                          ?? new List<LotteryProbabilityInfo>();

            var cardPools = JsonMapper.ToObject<List<LotteryCardPools>>(poolAsset.text)
                            ?? new List<LotteryCardPools>();

            // 建立卡池索引
            LotteryCardPools.Clear();
            foreach (var entry in cardPools)
            {
                if (!LotteryCardPools.TryGetValue(entry.lotteryGrade, out var list))
                {
                    list = new List<LotteryCardPools>();
                    LotteryCardPools[entry.lotteryGrade] = list;
                }
                list.Add(entry);
            }

            m_IsInitialized = true;
            Debug.Log($"抽奖配置加载完成: 概率项{LotteryList.Count}条, 卡池等级{LotteryCardPools.Count}个");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"抽奖配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Clear()
    {
        LotteryList.Clear();
        LotteryCardPools.Clear();
        m_IsInitialized = false;
    }
}
