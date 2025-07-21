using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class PropertyDispositionManager : Singleton<PropertyDispositionManager>
{
    // 多维度数据存储
    private Dictionary<int, PropertyInfo> m_IdPropertyDict = new Dictionary<int, PropertyInfo>();
    private Dictionary<string, PropertyInfo> m_NamePropertyDict = new Dictionary<string, PropertyInfo>();
    private Dictionary<int, List<PropertyInfo>> m_TypePropertyDict = new Dictionary<int, List<PropertyInfo>>();

    // 资源管理
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    /// <summary>
    /// 初始化物品配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;

        try
        {
            m_ConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.propertyInfo);

            if (m_ConfigAsset == null)
            {
                Debug.LogError($"物品配置文件加载失败：{ConfigDefine.propertyInfo}");
                return;
            }

            var propertyList = JsonMapper.ToObject<List<PropertyInfo>>(m_ConfigAsset.text);
            if (propertyList == null || propertyList.Count == 0)
            {
                Debug.LogError("物品配置数据解析失败");
                return;
            }

            foreach (var property in propertyList)
            {
                // 数据校验
                if (m_IdPropertyDict.ContainsKey(property.propertyID))
                {
                    Debug.LogError($"物品ID重复：{property.propertyID}");
                    continue;
                }

                if (string.IsNullOrEmpty(property.propertyName))
                {
                    Debug.LogWarning("发现无名物品配置，已跳过");
                    continue;
                }

                // 建立索引
                m_IdPropertyDict[property.propertyID] = property;
                m_NamePropertyDict[property.propertyName] = property;

                // 类型索引
                if (!m_TypePropertyDict.ContainsKey(property.propertyGrade))
                {
                    m_TypePropertyDict[property.propertyGrade] = new List<PropertyInfo>();
                }
                m_TypePropertyDict[property.propertyGrade].Add(property);
            }

            m_IsInitialized = true;
            Debug.Log($"物品配置加载完成，总计{propertyList.Count}件物品");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"物品配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 按名称获取物品配置
    /// </summary>
    public PropertyInfo GetPropertyByName(string name)
    {
        if (!m_IsInitialized) return null;
        return m_NamePropertyDict.TryGetValue(name, out var property) ? property : null;
    }

    /// <summary>
    /// 按ID获取物品配置
    /// </summary>
    public PropertyInfo GetPropertyById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdPropertyDict.TryGetValue(id, out var property) ? property : null;
    }

    /// <summary>
    /// 获取指定类型的物品列表
    /// </summary>
    public List<PropertyInfo> GetPropertysByType(int type)
    {
        if (!m_IsInitialized) return new List<PropertyInfo>();
        return m_TypePropertyDict.TryGetValue(type, out var list) ? list : new List<PropertyInfo>();
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void OnClear()
    {
        if (m_ConfigAsset != null)
        {
            Resources.UnloadAsset(m_ConfigAsset);
            m_ConfigAsset = null;
        }

        m_IdPropertyDict.Clear();
        m_NamePropertyDict.Clear();
        m_TypePropertyDict.Clear();
        m_IsInitialized = false;
    }
}