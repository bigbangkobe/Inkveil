using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using LitJson;
using UnityEngine;

public class MagicDispositionManager : Singleton<MagicDispositionManager>
{
    // 多维度数据存储
    private Dictionary<int, MagicInfo> m_IdMagicDict = new Dictionary<int, MagicInfo>();
    private Dictionary<string, MagicInfo> m_NameMagicDict = new Dictionary<string, MagicInfo>();
    private Dictionary<MagicInfo.MagicType, List<MagicInfo>> m_TypeMagicDict = new Dictionary<MagicInfo.MagicType, List<MagicInfo>>();

    // 配置加载状态
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    /// <summary>
    /// 异步初始化法宝配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        try
        {
            // 异步加载配置文件
            var handle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.magicInfo);
            m_ConfigAsset = await handle.Task;

            if (m_ConfigAsset == null)
            {
                Debug.LogError($"法宝配置文件加载失败：{ConfigDefine.magicInfo}");
                return;
            }

            // 解析 JSON 列表
            var magicList = JsonMapper.ToObject<List<MagicInfo>>(m_ConfigAsset.text) ?? new List<MagicInfo>();
            if (magicList.Count == 0)
            {
                Debug.LogError("法宝配置数据解析失败");
                return;
            }

            // 建立多维索引
            foreach (var magic in magicList)
            {
                if (m_IdMagicDict.ContainsKey(magic.magicID))
                {
                    Debug.LogError($"法宝ID重复：{magic.magicID}");
                    continue;
                }
                if (string.IsNullOrEmpty(magic.magicName))
                {
                    Debug.LogWarning("发现无名法宝配置，已跳过");
                    continue;
                }

                m_IdMagicDict[magic.magicID] = magic;
                m_NameMagicDict[magic.magicName] = magic;

                if (!m_TypeMagicDict.TryGetValue(magic.magicType, out var list))
                {
                    list = new List<MagicInfo>();
                    m_TypeMagicDict[magic.magicType] = list;
                }
                list.Add(magic);
            }

            m_IsInitialized = true;
            Debug.Log($"法宝配置加载完成，总计{magicList.Count}件法宝");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"法宝配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 按名称获取法宝配置
    /// </summary>
    public MagicInfo GetMagicByName(string name)
    {
        if (!m_IsInitialized) return null;
        return m_NameMagicDict.TryGetValue(name, out var magic) ? magic : null;
    }

    /// <summary>
    /// 按ID获取法宝配置
    /// </summary>
    public MagicInfo GetMagicById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdMagicDict.TryGetValue(id, out var magic) ? magic : null;
    }

    /// <summary>
    /// 获取指定类型的法宝列表
    /// </summary>
    public List<MagicInfo> GetMagicsByType(MagicInfo.MagicType type)
    {
        if (!m_IsInitialized) return new List<MagicInfo>();
        return m_TypeMagicDict.TryGetValue(type, out var list) ? list : new List<MagicInfo>();
    }

    /// <summary>
    /// 获取可升级的法宝列表
    /// </summary>
    public List<MagicInfo> GetUpgradableMagics()
    {
        var result = new List<MagicInfo>();
        if (!m_IsInitialized) return result;
        foreach (var magic in m_IdMagicDict.Values)
        {
            if (magic.CanUpgrade())
                result.Add(magic);
        }
        return result;
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        if (m_ConfigAsset != null)
        {
            ResourceService.UnloadAsset(m_ConfigAsset);
            m_ConfigAsset = null;
        }
        m_IdMagicDict.Clear();
        m_NameMagicDict.Clear();
        m_TypeMagicDict.Clear();
        m_IsInitialized = false;
    }
}
