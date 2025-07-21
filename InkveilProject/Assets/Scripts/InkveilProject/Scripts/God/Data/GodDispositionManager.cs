using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class GodDispositionManager : Singleton<GodDispositionManager>
{
    // 多维度数据存储
    private Dictionary<int, GodInfo> m_IdGodDict = new Dictionary<int, GodInfo>();
    private Dictionary<string, GodInfo> m_NameGodDict = new Dictionary<string, GodInfo>();

    public GodInfo curGod;

    // 资源管理
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    /// <summary>
    /// 初始化神明配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;

        try
        {
            m_ConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.godInfo);

            if (m_ConfigAsset == null)
            {
                Debug.LogError($"神明配置文件加载失败：{ConfigDefine.godInfo}");
                return;
            }

            var godList = JsonMapper.ToObject<List<GodInfo>>(m_ConfigAsset.text);
            if (godList == null || godList.Count == 0)
            {
                Debug.LogError("神明配置数据解析失败");
                return;
            }

            foreach (var god in godList)
            {
                // 数据校验
                if (m_IdGodDict.ContainsKey(god.godID))
                {
                    Debug.LogError($"神明ID重复：{god.godID}");
                    continue;
                }

                if (string.IsNullOrEmpty(god.godName))
                {
                    Debug.LogWarning("发现无名神明配置，已跳过");
                    continue;
                }

                // 建立索引
                m_IdGodDict[god.godID] = god;
                m_NameGodDict[god.godName] = god;
            }

            m_IsInitialized = true;
            Debug.Log($"神明配置加载完成，总计{godList.Count}位神明");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"神明配置初始化异常：{ex}");
        }

        SetCurGod("哪吒");
    }

    public void SetCurGod(string name) 
    {
        if (m_NameGodDict.TryGetValue(name, out var god))
        {
            curGod = god;        }
    }

    /// <summary>
    /// 按名称获取神明配置
    /// </summary>
    public GodInfo GetGodByName(string name)
    {
        if (!m_IsInitialized) return null;
        return m_NameGodDict.TryGetValue(name, out var god) ? god : null;
    }

    /// <summary>
    /// 按ID获取神明配置
    /// </summary>
    public GodInfo GetGodById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdGodDict.TryGetValue(id, out var god) ? god : null;
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        if (m_ConfigAsset != null)
        {
            Resources.UnloadAsset(m_ConfigAsset);
            m_ConfigAsset = null;
        }

        m_IdGodDict.Clear();
        m_NameGodDict.Clear();
        m_IsInitialized = false;
    }
}