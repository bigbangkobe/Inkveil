using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    //============= 配置参数 =============
    [Header("运行时状态")]
    public LevelInfo m_CurLevelInfo;     // 当前关卡配置信息
    public int m_CurWave;                // 当前波次索引
    public int m_SumWave;                // 总波次数
    public float m_GameTime;             // 游戏累计时间
    public bool isVictory = false;       // 是否已胜利
    public event Action OnVictory;       // 胜利事件

    //============= 内部状态 =============
    private List<WaveConfig> m_WaveConfigs = new List<WaveConfig>();
    // LevelManager 核心修改部分
    private List<WaveState> m_WaveStates = new List<WaveState>();
    private Coroutine m_WaveCoroutine;
    private bool m_AllWavesSpawned;
    public int m_CurLevel = 1;
    public int curGrade = 1;
    private Transform m_EnemyPoint;

    private List<Transform> m_EnemyPointList;

    //============= 核心逻辑 =============

    internal void OnInit()
    {
        LevelDispositionManager.instance.OnInit();
        StageRewardsDispositionManager.instance.OnInit();
        EnemyManager.instance.OnEnemyDestroyed += CheckVictoryCondition;
    }

    internal void Clear()
    {
        EnemyManager.instance.OnEnemyDestroyed -= CheckVictoryCondition;
        StopAllCoroutines();
        ResetLevel();
    }

    public void StartLevel() 
    {
        SetCurLevel(m_CurLevel);
    }



    public void SetCurLevel(int level = 1)
    {
        m_CurLevel=  level;
        // 重置状态
        m_AllWavesSpawned = false;
        isVictory = false;

        // 获取关卡配置
        m_CurLevelInfo = LevelDispositionManager.instance.GetLevelInfoByLevel(level);
        if (m_CurLevelInfo == null)
        {
            Debug.LogError($"无法加载关卡配置: Level={level}");
            return;
        }

        // 解析配置数据
        m_WaveConfigs.Clear();
        int[][] rawWaveData = m_CurLevelInfo.GetEnemyGroup();
        foreach (int[] config in rawWaveData)
        {
            if (config.Length < 3) continue;
            m_WaveConfigs.Add(new WaveConfig(config[0], config[1], config[2]));
        }
        m_SumWave = m_WaveConfigs.Count;
    }

    public void StartGame() 
    {
        if (m_WaveCoroutine != null) StopCoroutine(m_WaveCoroutine);
        m_WaveCoroutine = StartCoroutine(ProcessWaves());
    }

    private IEnumerator ProcessWaves()
    {
        m_WaveStates.Clear();

        for (int waveIndex = 0; waveIndex < m_WaveConfigs.Count; waveIndex++)
        {
           
            WaveConfig currentWave = m_WaveConfigs[waveIndex];
            var waveState = new WaveState { config = currentWave };
            m_WaveStates.Add(waveState);

            // 计算触发时间
            float triggerTime = CalculateTriggerTime(waveIndex);
            yield return new WaitUntil(() => m_GameTime >= triggerTime);
            if (waveIndex == 1 && !GuideDispositionManager.instance.isGuide)
            {
                OnboardingGuidePanel.instance.StartGuide();
            }
            // 生成敌人
            EnemyGroupInfo group = LevelDispositionManager.instance.GetEnemyGroupInfoByID(currentWave.enemyGroupID);
            if (group != null)
            {
                yield return StartCoroutine(SpawnEnemyGroup(group, currentWave.spawnMode, waveState));
            }

            // 记录生成完成时间
            waveState.spawnCompleteTime = m_GameTime;

            // 如果当前波次需要等待敌人死亡才能继续（模式1/3）
            if (currentWave.spawnMode == 1 || currentWave.spawnMode == 3)
            {
                yield return new WaitUntil(() => waveState.IsEnemiesCleared);
                waveState.enemiesClearedTime = m_GameTime;
            }
        }

        m_AllWavesSpawned = true;
        CheckVictoryCondition();
    }

    private float CalculateTriggerTime(int currentWaveIndex)
    {
        // 第一波立即触发
        if (currentWaveIndex == 0) return 0f;
        
        WaveState prevWave = m_WaveStates[currentWaveIndex - 1];

        switch (prevWave.config.spawnMode)
        {
            case 1: // 等上一波全灭后按间隔出现
                return prevWave.enemiesClearedTime + prevWave.config.triggerTime;

            case 2: // 上一波后按间隔出现
                return prevWave.spawnCompleteTime + prevWave.config.triggerTime;

            case 3: // 上一波全灭后立即出现
                return prevWave.enemiesClearedTime;

            default:
                return 0f;
        }
    }

    private IEnumerator SpawnEnemyGroup(EnemyGroupInfo group, int spawnMode, WaveState waveState)
    {
        int[][] enemies = group.GetEnemyGroup();

        switch (group.AppearanceMode)
        {
            case 1: // 批量生成
                foreach (int[] config in enemies)
                {
                    for (int i = 0; i < config[1]; i++)
                    {
                        yield return new WaitForFixedUpdate();
                        SpawnAndTrackEnemy(config[0], waveState);
                    }
                }
                break;

            case 2: // 间隔生成
                foreach (int[] config in enemies)
                {
                    for (int i = 0; i < config[1]; i++)
                    {
                        SpawnAndTrackEnemy(config[0], waveState);
                        yield return new WaitForSeconds((float)group.AppearanceContent);
                    }
                }
                break;
        }

        // 处理模式2的特殊等待
        if (spawnMode == 2)
        {
            yield return new WaitForSeconds((float)group.AppearanceContent);
        }
    }

    private async void SpawnAndTrackEnemy(int enemyID, WaveState waveState)
    {
        EnemyBase enemy = await EnemyManager.instance.SpawnEnemy(enemyID, GetRandomSpawnPoint());
        waveState.enemies.Add(enemy);
        enemy.OnDestroyed += () => {
            waveState.enemies.Remove(enemy);
            if (waveState.IsEnemiesCleared)
            {
                waveState.enemiesClearedTime = m_GameTime;
            }
        };
    }

    private void CheckVictoryCondition()
    {
        if (isVictory || !m_AllWavesSpawned) return;
        if (EnemyManager.instance.ActiveEnemyCount == 0)
        {
            isVictory = true;
            OnVictory?.Invoke();
            GameManager.instance.GameStateEnum = GameConfig.GameState.State.Victory;
            Debug.Log("游戏胜利！");
        }
    }

    private Transform GetSpawnPoint()
    {
        return m_EnemyPoint;
    }

    /// <summary>
    /// 获得随机一个生成点
    /// </summary>
    /// <returns></returns>
    private Transform GetRandomSpawnPoint()
    {
        if (m_EnemyPointList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, m_EnemyPointList.Count);
            return m_EnemyPointList[randomIndex];

        }
        return m_EnemyPoint;
    }


    /// <summary>
    /// 获得生成点列表
    /// </summary>
    /// <returns></returns>
    private List<Transform> GetSpawnPointList()
    {
        return m_EnemyPointList;
    }

    public void ResetLevel()
    {
        if (m_WaveCoroutine != null)
        {
            StopCoroutine(m_WaveCoroutine);
            m_WaveCoroutine = null;
        }
        EnemyManager.instance.ClearAllEnemies();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void OnUpdate()
    {
        if (!isVictory)
        {
            m_GameTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 设置单独的生成点
    /// </summary>
    /// <param name="m_EnemyPoint">生成点</param>
    internal void SetEnemyPoint(Transform m_EnemyPoint)
    {
        this.m_EnemyPoint = m_EnemyPoint;
    }

    /// <summary>
    /// 设置生成点列表
    /// </summary>
    /// <param name="m_EnemyPoint">生成点列表</param>
    internal void SetEnemyPointList(List<Transform> m_EnemyPointList)
    {
        this.m_EnemyPointList = m_EnemyPointList;
    }
}

// 新增波次状态跟踪类
public class WaveState
{
    public WaveConfig config;
    public List<EnemyBase> enemies = new List<EnemyBase>();
    public float spawnCompleteTime;    // 波次生成完成时间
    public float enemiesClearedTime;   // 敌人全灭时间
    public bool IsEnemiesCleared => enemies.Count == 0;
}

[System.Serializable]
public class WaveConfig
{
    public int enemyGroupID;
    public float triggerTime;
    public int spawnMode;

    public WaveConfig(int id, float time, int mode)
    {
        enemyGroupID = id;
        triggerTime = time;
        spawnMode = mode;
    }
}