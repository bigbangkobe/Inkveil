using System.Collections.Generic;
using Framework;
using LitJson;
using System.Threading.Tasks;
using UnityEngine;

public class LevelDispositionManager : Singleton<LevelDispositionManager>
{
    private Dictionary<int, LevelInfo> m_IdLevelDict = new Dictionary<int, LevelInfo>();
    private Dictionary<int, EnemyGroupInfo> m_IdEnemyGroupDict = new Dictionary<int, EnemyGroupInfo>();
    private bool m_IsInitialized = false;

    /// <summary>
    /// 异步初始化关卡配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        try
        {
            // 异步加载配置文本
            var levelHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.levelInfo);
            var groupHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.enemyGroupInfo);

            TextAsset levelAsset = await levelHandle.Task;
            TextAsset groupAsset = await groupHandle.Task;

            if (levelAsset == null)
            {
                Debug.LogError($"关卡配置文件加载失败：{ConfigDefine.levelInfo}");
                return;
            }
            if (groupAsset == null)
            {
                Debug.LogError($"敌人组合配置文件加载失败：{ConfigDefine.enemyGroupInfo}");
                return;
            }

            // 解析并索引关卡数据
            var levelList = JsonMapper.ToObject<List<LevelInfo>>(levelAsset.text) ?? new List<LevelInfo>();
            foreach (var level in levelList)
            {
                if (m_IdLevelDict.ContainsKey(level.levelID))
                {
                    Debug.LogError($"关卡ID重复：{level.levelID}");
                    continue;
                }
                m_IdLevelDict[level.levelID] = level;
            }

            // 解析并索引敌人组合数据
            var groupList = JsonMapper.ToObject<List<EnemyGroupInfo>>(groupAsset.text) ?? new List<EnemyGroupInfo>();
            foreach (var group in groupList)
            {
                if (m_IdEnemyGroupDict.ContainsKey(group.enemyGroupID))
                {
                    Debug.LogError($"敌人组合ID重复：{group.enemyGroupID}");
                    continue;
                }
                m_IdEnemyGroupDict[group.enemyGroupID] = group;
            }

            m_IsInitialized = true;
            Debug.Log($"关卡配置加载完成: 共{levelList.Count}个关卡, {groupList.Count}个敌人组合");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"关卡配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 获取关卡信息
    /// </summary>
    public LevelInfo GetLevelInfoByLevel(int level)
    {
        if (!m_IsInitialized) return null;
        return m_IdLevelDict.TryGetValue(level, out var info) ? info : null;
    }

    /// <summary>
    /// 获取敌人组合信息
    /// </summary>
    public EnemyGroupInfo GetEnemyGroupInfoByID(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdEnemyGroupDict.TryGetValue(id, out var info) ? info : null;
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        m_IdLevelDict.Clear();
        m_IdEnemyGroupDict.Clear();
        m_IsInitialized = false;
    }
}
