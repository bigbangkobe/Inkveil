using System;
using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class WeaponDispositionManager : Singleton<WeaponDispositionManager>
{
    // 多维度数据存储
    private Dictionary<int, WeaponInfo> m_IdWeaponDict = new Dictionary<int, WeaponInfo>();
    private Dictionary<string, WeaponInfo> m_NameWeaponDict = new Dictionary<string, WeaponInfo>();
    private Dictionary<int, List<WeaponInfo>> m_TypeWeaponDict = new Dictionary<int, List<WeaponInfo>>();

    // 资源管理
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    /// <summary>
    /// 初始化武器配置系统
    /// </summary>
    public async void OnItin()
    {
        if (m_IsInitialized) return;

        try
        {
            m_ConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.weaponInfo);
            if (m_ConfigAsset == null)
            {
                Debug.LogError($"武器配置文件加载失败：{ConfigDefine.weaponInfo}");
                return;
            }

            var weaponList = JsonHelper.ToObject<List<WeaponInfo>>(m_ConfigAsset.text);
            if (weaponList == null || weaponList.Count == 0)
            {
                Debug.LogError("武器配置数据解析失败");
                return;
            }

            foreach (var weapon in weaponList)
            {
                // 数据校验
                if (m_IdWeaponDict.ContainsKey(weapon.weaponID))
                {
                    Debug.LogError($"武器ID重复：{weapon.weaponID}");
                    continue;
                }

                if (string.IsNullOrEmpty(weapon.weaponName))
                {
                    Debug.LogWarning("发现无名武器配置，已跳过");
                    continue;
                }

                // 建立索引
                m_IdWeaponDict[weapon.weaponID] = weapon;
                m_NameWeaponDict[weapon.weaponName] = weapon;

                // 类型索引
                if (!m_TypeWeaponDict.ContainsKey(weapon.weaponType))
                {
                    m_TypeWeaponDict[weapon.weaponType] = new List<WeaponInfo>();
                }
                m_TypeWeaponDict[weapon.weaponType].Add(weapon);
            }

            m_IsInitialized = true;
            Debug.Log($"武器配置加载完成，总计{weaponList.Count}件武器");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"武器配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 按名称获取武器配置
    /// </summary>
    public WeaponInfo GetWeaponByName(string name)
    {
        if (!m_IsInitialized) return null;
        return m_NameWeaponDict.TryGetValue(name, out var weapon) ? weapon : null;
    }

    /// <summary>
    /// 按ID获取武器配置
    /// </summary>
    public WeaponInfo GetWeaponById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdWeaponDict.TryGetValue(id, out var weapon) ? weapon : null;
    }

    /// <summary>
    /// 获取指定类型的武器列表
    /// </summary>
    public List<WeaponInfo> GetWeaponsByType(int type)
    {
        if (!m_IsInitialized) return new List<WeaponInfo>();
        return m_TypeWeaponDict.TryGetValue(type, out var list) ? list : new List<WeaponInfo>();
    }

    /// <summary>
    /// 获取可升级的武器列表
    /// </summary>
    public List<WeaponInfo> GetUpgradableWeapons()
    {
        var result = new List<WeaponInfo>();
        foreach (var weapon in m_IdWeaponDict.Values)
        {
            if (weapon.CanUpgrade())
            {
                result.Add(weapon);
            }
        }
        return result;
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

        m_IdWeaponDict.Clear();
        m_NameWeaponDict.Clear();
        m_TypeWeaponDict.Clear();
        m_IsInitialized = false;
    }

    internal void GetCurWeaponInfo()
    {

    }
}