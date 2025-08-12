using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    //============= ���ò��� =============
    [Header("����ʱ״̬")]
    public LevelInfo m_CurLevelInfo;     // ��ǰ�ؿ�������Ϣ
    public int m_CurWave;                // ��ǰ��������
    public int m_SumWave;                // �ܲ�����
    public float m_GameTime;             // ��Ϸ�ۼ�ʱ��
    public bool isVictory = false;       // �Ƿ���ʤ��
    public event Action OnVictory;       // ʤ���¼�

    //============= �ڲ�״̬ =============
    private List<WaveConfig> m_WaveConfigs = new List<WaveConfig>();
    // LevelManager �����޸Ĳ���
    private List<WaveState> m_WaveStates = new List<WaveState>();
    private Coroutine m_WaveCoroutine;
    private bool m_AllWavesSpawned;
    public int m_CurLevel = 1;
    public int curGrade = 1;
    private Transform m_EnemyPoint;

    private List<Transform> m_EnemyPointList;

    //============= �����߼� =============

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
        // ����״̬
        m_AllWavesSpawned = false;
        isVictory = false;

        // ��ȡ�ؿ�����
        m_CurLevelInfo = LevelDispositionManager.instance.GetLevelInfoByLevel(level);
        if (m_CurLevelInfo == null)
        {
            Debug.LogError($"�޷����عؿ�����: Level={level}");
            return;
        }

        // ������������
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

            // ���㴥��ʱ��
            float triggerTime = CalculateTriggerTime(waveIndex);
            yield return new WaitUntil(() => m_GameTime >= triggerTime);
            if (waveIndex == 1 && !GuideDispositionManager.instance.isGuide)
            {
                OnboardingGuidePanel.instance.StartGuide();
            }
            // ���ɵ���
            EnemyGroupInfo group = LevelDispositionManager.instance.GetEnemyGroupInfoByID(currentWave.enemyGroupID);
            if (group != null)
            {
                yield return StartCoroutine(SpawnEnemyGroup(group, currentWave.spawnMode, waveState));
            }

            // ��¼�������ʱ��
            waveState.spawnCompleteTime = m_GameTime;

            // �����ǰ������Ҫ�ȴ������������ܼ�����ģʽ1/3��
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
        // ��һ����������
        if (currentWaveIndex == 0) return 0f;
        
        WaveState prevWave = m_WaveStates[currentWaveIndex - 1];

        switch (prevWave.config.spawnMode)
        {
            case 1: // ����һ��ȫ��󰴼������
                return prevWave.enemiesClearedTime + prevWave.config.triggerTime;

            case 2: // ��һ���󰴼������
                return prevWave.spawnCompleteTime + prevWave.config.triggerTime;

            case 3: // ��һ��ȫ�����������
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
            case 1: // ��������
                foreach (int[] config in enemies)
                {
                    for (int i = 0; i < config[1]; i++)
                    {
                        yield return new WaitForFixedUpdate();
                        SpawnAndTrackEnemy(config[0], waveState);
                    }
                }
                break;

            case 2: // �������
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

        // ����ģʽ2������ȴ�
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
            Debug.Log("��Ϸʤ����");
        }
    }

    private Transform GetSpawnPoint()
    {
        return m_EnemyPoint;
    }

    /// <summary>
    /// ������һ�����ɵ�
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
    /// ������ɵ��б�
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
    /// ���õ��������ɵ�
    /// </summary>
    /// <param name="m_EnemyPoint">���ɵ�</param>
    internal void SetEnemyPoint(Transform m_EnemyPoint)
    {
        this.m_EnemyPoint = m_EnemyPoint;
    }

    /// <summary>
    /// �������ɵ��б�
    /// </summary>
    /// <param name="m_EnemyPoint">���ɵ��б�</param>
    internal void SetEnemyPointList(List<Transform> m_EnemyPointList)
    {
        this.m_EnemyPointList = m_EnemyPointList;
    }
}

// ��������״̬������
public class WaveState
{
    public WaveConfig config;
    public List<EnemyBase> enemies = new List<EnemyBase>();
    public float spawnCompleteTime;    // �����������ʱ��
    public float enemiesClearedTime;   // ����ȫ��ʱ��
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