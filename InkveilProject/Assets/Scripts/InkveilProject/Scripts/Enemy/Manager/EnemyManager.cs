using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    private readonly List<EnemyBase> m_EnemyList = new List<EnemyBase>();
    private readonly Dictionary<string, ObjectPool> m_EnemyPoolDic = new Dictionary<string, ObjectPool>();
    public Dictionary<string, GameObject> m_EnemyPrefabDic = new Dictionary<string, GameObject>();

    // Endless mode variables
    private bool m_IsEndlessMode = false;
    private float m_ElapsedTime = 0f;
    private float m_NextSpawnTime = 0f;
    private int m_CurrentWave = 0;
    private float m_DifficultyScale = 1f;
    private float m_LastDifficultyIncreaseTime = 0f;
    private bool m_ElitesUnlocked = false;

    public event Action OnEnemyDestroyed;
    public int ActiveEnemyCount => m_EnemyList.Count;

    //[SerializeField] private float m_SpawnRadius = 5f;
    //[SerializeField] private float m_InitialSpawnInterval = 10f;
    //[SerializeField] private float m_MinSpawnInterval = 3f;
    //[SerializeField] private float m_SpawnIntervalDecreaseRate = 0.05f;
    //[SerializeField] private int m_InitialEnemiesPerWave = 3;
    //[SerializeField] private float m_EnemiesIncreaseRate = 0.5f;
    //[SerializeField] private List<string> m_EndlessModeEnemyTypes = new List<string>();

    //// Wave settings
    //[Header("Wave Settings")]
    //[SerializeField] private float m_WaveInterval = 120f; // 2 minutes between difficulty increases
    //[SerializeField] private float m_EliteUnlockTime = 180f; // 3 minutes until elites appear
    //[SerializeField] private int m_EliteSplitIndex = 3; // First half are normal, second half are elites
    //[SerializeField] private float m_EliteSpawnChance = 0.1f; // Starting chance to spawn elite
    //[SerializeField] private float m_EliteChanceIncrease = 0.05f; // Increase per wave after unlock

    public List<EnemyBase> EnemyList => m_EnemyList;
    public bool IsEndlessMode => m_IsEndlessMode;
    public float ElapsedTime => m_ElapsedTime;
    public int CurrentWave => m_CurrentWave;

    /// <summary>
    /// 初始化函数
    /// </summary>
    public void OnInit()
    {
        EnemyDispositionManager.instance.OnItin();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        //StartEndlessMode();
    }

    /// <summary>
    /// 内存释放函数
    /// </summary>
    public void Clear()
    {
        for (int i = m_EnemyList.Count - 1;i >= 0; i--)
        {
            RemoveEnemy(m_EnemyList[i].gameObject.name, m_EnemyList[i]);
            //m_EnemyList.RemoveAt(i);
        }
    }

    internal async Task<EnemyBase> SpawnEnemy(int id, Transform transform)
    {
        EnemyBase enemyBase = await GetEnemyByID(id);
        enemyBase.transform.position = transform.position;
        enemyBase.OnDestroyed = HandleEnemyDestroyed;
        return enemyBase;
    }
    private void HandleEnemyDestroyed()
    {
        OnEnemyDestroyed?.Invoke();
    }
    /// <summary>
    /// 从对象池中获取Enemy对象并在随机位置生成
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns></returns>
    public async Task<EnemyBase> GetEnemyByID(int id)
    {
        EnemyInfo enemyInfo = EnemyDispositionManager.instance.GetBaseInfoById(id);
        return (await GetEnemyByName(enemyInfo.enemyName));
    }

    /// <summary>
    /// 从对象池中获取Enemy对象并在随机位置生成
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns></returns>
    public async Task<EnemyBase> GetEnemyByName(string name)
    {
        EnemyInfo enemy = EnemyDispositionManager.instance.GetBaseInfoByName(name);
        ObjectPool EnemyPool = GetEnemyPool(name);
        EnemyBase Enemy = await EnemyPool.GetAsync(name) as EnemyBase;
        Enemy.OnInit(enemy);

        // Set random position around spawn point
        //SetRandomPosition(Enemy.transform);

        EnemyList.Add(Enemy);
        return Enemy;
    }

    internal void OnUpdate()
    {
        foreach (var enemy in EnemyList) 
        {
            enemy.OnUpdate();
        }
        //if (m_IsEndlessMode)
        //{
        //    UpdateEndlessMode();
        //}
    }

    ///// <summary>
    ///// 更新无尽模式逻辑
    ///// </summary>
    //private void UpdateEndlessMode()
    //{
    //    m_ElapsedTime += Time.deltaTime;

    //    // Check if we should unlock elites
    //    if (!m_ElitesUnlocked && m_ElapsedTime >= m_EliteUnlockTime)
    //    {
    //        m_ElitesUnlocked = true;
    //        Debug.Log("Elite enemies unlocked!");
    //    }

    //    if (m_ElapsedTime - m_LastDifficultyIncreaseTime >= m_WaveInterval)
    //    {
    //        m_LastDifficultyIncreaseTime = m_ElapsedTime;
    //        IncreaseDifficulty();
    //    }

    //    // Spawn enemies on timer
    //    if (m_ElapsedTime >= m_NextSpawnTime)
    //    {
    //        SpawnWave();
    //        CalculateNextSpawnTime();
    //    }
    //}

    /// <summary>
    /// 增加游戏难度
    /// </summary>
    //private void IncreaseDifficulty()
    //{
    //    m_CurrentWave++;
    //    Debug.Log($"Wave {m_CurrentWave} - Difficulty increased!");

    //    if (m_ElitesUnlocked)
    //    {
    //        m_EliteSpawnChance = Mathf.Min(0.8f, m_EliteSpawnChance + m_EliteChanceIncrease);
    //    }
    //}

    /// <summary>
    /// 生成一波敌人
    /// </summary>
    //private void SpawnWave()
    //{
    //    // Calculate how many enemies to spawn this wave
    //    int enemiesToSpawn = Mathf.FloorToInt(m_InitialEnemiesPerWave + (m_CurrentWave * m_EnemiesIncreaseRate));

    //    for (int i = 0; i < enemiesToSpawn; i++)
    //    {
    //        if (m_EndlessModeEnemyTypes.Count == 0) return;

    //        bool spawnElite = m_ElitesUnlocked &&
    //                        m_EndlessModeEnemyTypes.Count > m_EliteSplitIndex &&
    //                        Random.value < m_EliteSpawnChance;

    //        string enemyType;
    //        if (spawnElite)
    //        {
    //            int eliteIndex = Random.Range(m_EliteSplitIndex, m_EndlessModeEnemyTypes.Count);
    //            enemyType = m_EndlessModeEnemyTypes[eliteIndex];
    //        }
    //        else
    //        {
    //            int normalIndex = Random.Range(0, Mathf.Min(m_EliteSplitIndex, m_EndlessModeEnemyTypes.Count));
    //            enemyType = m_EndlessModeEnemyTypes[normalIndex];
    //        }

    //        GetEnemyByName(enemyType);
    //    }
    //}

    /// <summary>
    /// 计算下一次生成时间
    /// </summary>
    //private void CalculateNextSpawnTime()
    //{
    //    float spawnInterval = Mathf.Max(
    //        m_MinSpawnInterval,
    //        m_InitialSpawnInterval - (m_CurrentWave * m_SpawnIntervalDecreaseRate * m_DifficultyScale)
    //    );

    //    m_NextSpawnTime = m_ElapsedTime + spawnInterval;
    //}

    /// <summary>
    /// 开始无尽模式
    /// </summary>
    //public void StartEndlessMode(float difficultyScale = 1f)
    //{
    //    m_IsEndlessMode = true;
    //    m_ElapsedTime = 0f;
    //    m_CurrentWave = 0;
    //    m_LastDifficultyIncreaseTime = 0f;
    //    m_DifficultyScale = difficultyScale;
    //    m_ElitesUnlocked = false;
    //    m_EliteSpawnChance = 0f;
    //    m_NextSpawnTime = m_InitialSpawnInterval; // First wave after initial interval
    //}

    /// <summary>
    /// 停止无尽模式
    /// </summary>
    public void StopEndlessMode()
    {
        m_IsEndlessMode = false;
    }

    /// <summary>
    /// 从对象池中移除Enemy对象
    /// </summary>
    /// <param name="name">对象池ID名称</param>
    /// <param name="Enemy">移除的对象</param>
    public void RemoveEnemy(string name, EnemyBase Enemy)
    {
        ObjectPool EnemyPool = GetEnemyPool(name);
        EnemyList.Remove(Enemy);
        EnemyPool.Remove(Enemy);
    }

    /// <summary>
    /// 获得Enemy对象池
    /// </summary>
    /// <param name="name">Enemy对象池的名称</param>
    /// <returns></returns>
    private ObjectPool GetEnemyPool(string name)
    {
        if (!m_EnemyPoolDic.TryGetValue(name, out ObjectPool EnemyPool))
        {
            EnemyPool = new ObjectPool(OnEnemyConstruct, OnEnemyDestroy, OnEnemyEnabled, OnEnemyDisabled);
            m_EnemyPoolDic.Add(name, EnemyPool);
        }

        return EnemyPool;
    }

    /// <summary>
    /// 设置随机生成位置
    /// </summary>
    /// <param name="enemyTransform">敌人的Transform</param>
    //private void SetRandomPosition(Transform enemyTransform)
    //{
    //    if (m_EnemyPoint != null)
    //    {
    //        Vector2 randomCircle = Random.insideUnitCircle * m_SpawnRadius;
    //        Vector3 spawnPosition = m_EnemyPoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    //        enemyTransform.position = spawnPosition;
    //    }
    //}

    /// <summary>
    /// 取消显示Enemy
    /// </summary>
    /// <param name="arg1">Enemy对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnEnemyDisabled(object arg1, object arg2)
    {
        //if (arg1 == null) return;
        EnemyBase Enemy = arg1 as EnemyBase;
        Enemy.gameObject.SetActive(false);
        Enemy.transform.SetParent(transform);
    }

    /// <summary>
    /// 显示Enemy
    /// </summary>
    /// <param name="arg1">Enemy对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnEnemyEnabled(object arg1, object arg2)
    {
        //if (arg1 == null) return;
        EnemyBase Enemy = arg1 as EnemyBase;
        Enemy.gameObject.SetActive(true);
        //SetRandomPosition(Enemy.transform);
    }

    /// <summary>
    /// 摧毁Enemy
    /// </summary>
    /// <param name="arg1">Enemy对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnEnemyDestroy(object arg1, object arg2)
    {
        if (arg1 == null) return;
        EnemyBase Enemy = arg1 as EnemyBase;
        Destroy(Enemy);
    }

    private readonly object _prefabDicLock = new object();
    /// <summary>
    /// Enemy生成构造函数
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private async Task<object> OnEnemyConstruct(object arg)
    {
        string name = arg.ToString();
        GameObject gameObject;
        EnemyInfo enemy = EnemyDispositionManager.instance.GetBaseInfoByName(name);

        // 双重检查 + 加锁保护
        if (!m_EnemyPrefabDic.TryGetValue(name, out gameObject))
        {
            string path = enemy.prefabPath;
            if (!string.IsNullOrEmpty(path))
            {
                gameObject = await ResourceService.LoadAsync<GameObject>(path);
                if (gameObject)
                {
                    lock (_prefabDicLock)
                    {
                        if (!m_EnemyPrefabDic.ContainsKey(name)) // 二次确认
                        {
                            m_EnemyPrefabDic.Add(name, gameObject);
                        }
                    }
                }
                else
                {
                    Debug.LogError($"加载敌人Prefab失败: {name}");
                    return null;
                }
            }
        }
        else
        {
            if (gameObject == null)
            {
                Debug.LogError("从资源中读取Enemy资源失败");
                return null;
            }
        }

        // 实例化
        GameObject go = UnityEngine.Object.Instantiate(m_EnemyPrefabDic[name], transform);
        go.name = name;
        go.SetActive(false);
        return go.GetComponent<EnemyBase>();
    }

    internal void ClearAllEnemies()
    {
        // 遍历列表中的所有敌人
        for (int i = m_EnemyList.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = m_EnemyList[i];
            if (enemy == null) continue;

            // 从对象池移除（会触发 OnEnemyDisabled → 隐藏，或者最终销毁）
            RemoveEnemy(enemy.name, enemy);
        }

        // 清空列表记录
        m_EnemyList.Clear();
    }
}