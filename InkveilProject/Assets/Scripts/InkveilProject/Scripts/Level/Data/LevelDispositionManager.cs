using System.Collections;
using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class LevelDispositionManager : Singleton<LevelDispositionManager>
{

    Dictionary<int, LevelInfo> m_IdLevelDict = new Dictionary<int, LevelInfo>();
    Dictionary<int, EnemyGroupInfo> m_IdEnemyGroupDict = new Dictionary<int, EnemyGroupInfo>();

    private bool m_IsInitialized = false;

    /// <summary>
    /// 初始化关卡配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;

        try
        {
            string m_LevelConfigAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.levelInfo)).text;
            string m_EnemyGroupConfigAsset = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.enemyGroupInfo)).text;

            if (m_LevelConfigAsset == null)
            {
                Debug.LogError($"关卡配置文件加载失败：{ConfigDefine.levelInfo}");
                return;
            }

            var levelList = JsonMapper.ToObject<List<LevelInfo>>(m_LevelConfigAsset);
            if (levelList == null || levelList.Count == 0)
            {
                Debug.LogError("关卡配置数据解析失败");
                return;
            }

            foreach (LevelInfo level in levelList)
            {
                // 数据校验
                if (m_IdLevelDict.ContainsKey(level.levelID))
                {
                    Debug.LogError($"关卡ID重复：{level.levelID}");
                    continue;
                }

                // 建立索引
                m_IdLevelDict[level.levelID] = level;
            }

            if (m_EnemyGroupConfigAsset == null)
            {
                Debug.LogError($"关卡配置文件加载失败：{ConfigDefine.enemyGroupInfo}");
                return;
            }

            var enemyGroupList = JsonMapper.ToObject<List<EnemyGroupInfo>>(m_EnemyGroupConfigAsset);
            if (enemyGroupList == null || enemyGroupList.Count == 0)
            {
                Debug.LogError("敌人组合配置数据解析失败");
                return;
            }

            foreach (EnemyGroupInfo enemyGroup in enemyGroupList)
            {
                // 数据校验
                if (m_IdEnemyGroupDict.ContainsKey(enemyGroup.enemyGroupID))
                {
                    Debug.LogError($"敌人组合ID重复：{enemyGroup.enemyGroupID}");
                    continue;
                }

                // 建立索引
                m_IdEnemyGroupDict[enemyGroup.enemyGroupID] = enemyGroup;
            }

            m_IsInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"配置初始化异常：{ex}");
        }
    }

    public LevelInfo GetLevelInfoByLevel(int level) 
    {
        if (!m_IsInitialized) return null;
        return m_IdLevelDict.TryGetValue(level, out var levelInfo) ? levelInfo : null;
    }

    public EnemyGroupInfo GetEnemyGroupInfoByID(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdEnemyGroupDict.TryGetValue(id, out var enemyGroupInfo) ? enemyGroupInfo : null;
    }
}