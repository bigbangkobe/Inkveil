using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class LotteryDispositionManager : Singleton<LotteryDispositionManager>
{
    // 资源管理
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    public List<LotteryProbabilityInfo> LotteryList { get; private set; }
    public Dictionary<int, List<LotteryCardPools>> LotteryCardPools = new Dictionary<int, List<LotteryCardPools>>();

    /// <summary>
    /// 初始化抽奖配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;

        try
        {
            m_ConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.lottery_probability_info);
            string m_ConfigAssetCardPools = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.lottery_CardPools)).text;

            if (m_ConfigAsset == null)
            {
                Debug.LogError($"抽奖配置文件加载失败：{ConfigDefine.lottery_probability_info}");
                return;
            } 

            LotteryList = JsonHelper.ToObject<List<LotteryProbabilityInfo>>(m_ConfigAsset.text);
            List<LotteryCardPools> lotteryCardPools = JsonHelper.ToObject<List<LotteryCardPools>>(m_ConfigAssetCardPools);
            if (LotteryList == null || LotteryList.Count == 0)
            {
                Debug.LogError("抽奖配置数据解析失败");
                return;
            }


            for (int i = 0; i < lotteryCardPools.Count; i++)
            {
                if (!LotteryCardPools.ContainsKey(lotteryCardPools[i].lotteryGrade))
                {
                    LotteryCardPools[lotteryCardPools[i].lotteryGrade] = new List<LotteryCardPools>();
                }

                LotteryCardPools[lotteryCardPools[i].lotteryGrade].Add(lotteryCardPools[i]);
            }
           
            m_IsInitialized = true;
            Debug.Log($"抽奖配置加载完成，总计{LotteryList.Count}件抽奖");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"抽奖配置初始化异常：{ex}");
        }
    }

   
    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        //m_IsInitialized = false;
    }
}