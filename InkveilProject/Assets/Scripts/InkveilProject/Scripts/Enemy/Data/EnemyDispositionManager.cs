using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;
using System;

public class EnemyDispositionManager : Singleton<EnemyDispositionManager>
{
    // 使用双重字典提高查询效率
    private Dictionary<string, EnemyInfo> m_NameEnemyDict = new Dictionary<string, EnemyInfo>();
    private Dictionary<int, EnemyInfo> m_IdEnemyDict = new Dictionary<int, EnemyInfo>();
    private Dictionary<int, List<EnemyInfo>> m_TypeEnemyDict = new Dictionary<int, List<EnemyInfo>>();

    // 资源引用跟踪
    private TextAsset m_EnemyConfigAsset;
    private bool m_IsInitialized = false;

    /// <summary>
    /// 初始化敌人配置系统
    /// </summary>
    public async void OnItin()
    {
        if (m_IsInitialized) return;
        try
        {
             // Resources同步加载
            m_EnemyConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.enemyInfo);

            if (m_EnemyConfigAsset == null)
            {
                Debug.LogError($"敌人配置文件加载失败：{ConfigDefine.enemyInfo}");
                return;
            }

            var enemyList = JsonMapper.ToObject<List<EnemyInfo>>(m_EnemyConfigAsset.text);
            if (enemyList == null || enemyList.Count == 0)
            {
                Debug.LogError("敌人配置数据解析失败");
                return;
            }

            // 多维度数据组织
            foreach (var enemy in enemyList)
            {
                // 数据校验
                if (string.IsNullOrEmpty(enemy.enemyName))
                {
                    Debug.LogWarning("发现无名敌人配置，已跳过");
                    continue;
                }

                if (m_NameEnemyDict.ContainsKey(enemy.enemyName))
                {
                    Debug.LogError($"敌人名称重复：{enemy.enemyName}");
                    continue;
                }

                // 主索引
                m_NameEnemyDict[enemy.enemyName] = enemy;
                m_IdEnemyDict[enemy.enemyID] = enemy;

                // 类型索引
                if (!m_TypeEnemyDict.ContainsKey(enemy.enemyType))
                {
                    m_TypeEnemyDict[enemy.enemyType] = new List<EnemyInfo>();
                }
                m_TypeEnemyDict[enemy.enemyType].Add(enemy);
            }

            m_IsInitialized = true;
            Debug.Log($"敌人配置加载完成，总计{enemyList.Count}条记录");
        }
        catch (Exception ex)
        {
            Debug.LogError($"敌人配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 按名称获取敌人基础配置
    /// </summary>
    public EnemyInfo GetBaseInfoByName(string name)
    {
        if (!m_IsInitialized)
        {
            Debug.LogWarning("配置系统未初始化");
            return null;
        }
        return m_NameEnemyDict.TryGetValue(name, out var info) ? info : null;
    }

    /// <summary>
    /// 按ID获取敌人基础配置
    /// </summary>
    public EnemyInfo GetBaseInfoById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdEnemyDict.TryGetValue(id, out var info) ? info : null;
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void OnClear()
    {
        // 释放资源引用
        if (m_EnemyConfigAsset != null)
        {
            Resources.UnloadAsset(m_EnemyConfigAsset);
            m_EnemyConfigAsset = null;
        }

        // 清空数据
        m_NameEnemyDict.Clear();
        m_IdEnemyDict.Clear();
        m_TypeEnemyDict.Clear();
        m_IsInitialized = false;
    }
}