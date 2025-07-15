using Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoSingleton<EnemyUIManager>
{
    private readonly List<EnemyUI> m_EnemyUIList = new List<EnemyUI>();

    /// <summary>
    /// EnemyUI对象池
    /// </summary>
    private readonly Dictionary<string, ObjectPool> m_EnemyUIPoolDic = new Dictionary<string, ObjectPool>();

    /// <summary>
    /// EnemyUI预制体字典
    /// </summary>
    public Dictionary<string, GameObject> m_EnemyUIPrefabDic = new Dictionary<string, GameObject>();

    public List<EnemyUI> EnemyUIList => m_EnemyUIList;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        foreach(EnemyUI enemyUI in m_EnemyUIList)
        {
            enemyUI.OnUpdate();
        }
    }


    /// <summary>
    /// 从对象池中获取 EnemyUI 对象
    /// </summary>
    /// <param name="name">EnemyUI名称</param>
    /// <returns></returns>
    public EnemyUI GetEnemyUIByName(string name)
    {
        ObjectPool enemyUIPool = GetEnemyUIPool(name);
        EnemyUI enemyUI = enemyUIPool.Get(name) as EnemyUI;
        m_EnemyUIList.Add(enemyUI);
        return enemyUI;
    }

    /// <summary>
    /// 回收 EnemyUI 到对象池
    /// </summary>
    /// <param name="name">对象池ID</param>
    /// <param name="enemyUI">回收的对象</param>
    public void RemoveEnemyUI(string name, EnemyUI enemyUI)
    {
        ObjectPool enemyUIPool = GetEnemyUIPool(name);
        m_EnemyUIList.Remove(enemyUI);
        enemyUIPool.Remove(enemyUI);
    }

    /// <summary>
    /// 获取 EnemyUI 对象池
    /// </summary>
    /// <param name="name">池名称</param>
    /// <returns></returns>
    private ObjectPool GetEnemyUIPool(string name)
    {
        if (!m_EnemyUIPoolDic.TryGetValue(name, out ObjectPool enemyUIPool))
        {
            enemyUIPool = new ObjectPool(OnEnemyUIConstruct, OnEnemyUIDestroy, OnEnemyUIEnabled, OnEnemyUIDisabled);
            m_EnemyUIPoolDic.Add(name, enemyUIPool);
        }

        return enemyUIPool;
    }

    /// <summary>
    /// 关闭 EnemyUI
    /// </summary>
    private void OnEnemyUIDisabled(object obj, object extra)
    {
        EnemyUI enemyUI = obj as EnemyUI;
        enemyUI.gameObject.SetActive(false);
        enemyUI.transform.SetParent(transform);
    }

    /// <summary>
    /// 启用 EnemyUI
    /// </summary>
    private void OnEnemyUIEnabled(object obj, object extra)
    {
        EnemyUI enemyUI = obj as EnemyUI;
        enemyUI.gameObject.SetActive(true);
        enemyUI.OnReset();
    }

    /// <summary>
    /// 销毁 EnemyUI
    /// </summary>
    private void OnEnemyUIDestroy(object obj, object extra)
    {
        EnemyUI enemyUI = obj as EnemyUI;
        Destroy(enemyUI.gameObject);
    }

    /// <summary>
    /// 构造 EnemyUI
    /// </summary>
    private object OnEnemyUIConstruct(object arg)
    {
        string name = arg.ToString();
        GameObject prefab;

        if (!m_EnemyUIPrefabDic.ContainsKey(name))
        {
            string path = $"Enemy/{name}";
            prefab = ResourceService.Load<GameObject>(path);
            if (prefab)
            {
                m_EnemyUIPrefabDic.Add(name, prefab);
            }
            else
            {
                Debug.LogError($"[EnemyUIManager] 加载 EnemyUI 预制体失败: {path}");
                return null;
            }
        }
        prefab = m_EnemyUIPrefabDic[name];
        GameObject go = Instantiate(prefab, transform);
        go.name = name;
        go.SetActive(false);
        EnemyUI enemyUI = go.GetComponent<EnemyUI>();
        return enemyUI;
    }

    /// <summary>
    /// 将 EnemyUI 回收到对象池，恢复到管理器节点下
    /// </summary>
    /// <param name="name">UI 名称（池标识）</param>
    /// <param name="enemyUI">要回收的 EnemyUI 对象</param>
    public void ReturnEnemyUIToPool(string name, EnemyUI enemyUI)
    {
        if (enemyUI == null)
        {
            Debug.LogWarning("[EnemyUIManager] 尝试回收的 EnemyUI 为 null");
            return;
        }

        ObjectPool enemyUIPool = GetEnemyUIPool(name);

        if (!m_EnemyUIList.Contains(enemyUI))
        {
            Debug.LogWarning("[EnemyUIManager] 尝试回收的 EnemyUI 不在启用列表中");
        }
        else
        {
            m_EnemyUIList.Remove(enemyUI);
        }

        // 重置父节点回到管理器下
        enemyUI.transform.SetParent(transform);

        // 回收到池
        enemyUIPool.Remove(enemyUI);

        Debug.Log($"[EnemyUIManager] 已回收到池: {name}");
    }


    /// <summary>
    /// 清理内存
    /// </summary>
    public void Clear()
    {
        Debug.Log("[EnemyUIManager] 开始清理");

        // 清理所有对象池
        foreach (var kvp in m_EnemyUIPoolDic)
        {
            string poolName = kvp.Key;
            ObjectPool pool = kvp.Value;

            Debug.Log($"[EnemyUIManager] 清理池：{poolName}");
            pool.Clear(destroy: true);
        }

        m_EnemyUIPoolDic.Clear();
        m_EnemyUIPrefabDic.Clear();
        m_EnemyUIList.Clear();

        Debug.Log("[EnemyUIManager] 清理完毕");

        // 如果需要完全销毁管理器本体
        Destroy(gameObject);
    }
}