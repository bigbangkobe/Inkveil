using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GodManager : MonoBehaviour
{
    private readonly List<GodBase> m_GodList = new List<GodBase>();

    /// <summary>
    /// God对象池
    /// </summary>
    private readonly Dictionary<string, ObjectPool> m_GodPoolDic = new Dictionary<string, ObjectPool>();

    /// <summary>
    /// God预制体
    /// </summary>
    public Dictionary<string, GameObject> m_GodPrefabDic = new Dictionary<string, GameObject>();

    Transform m_GodPoint;

    internal void OnInitPoint(Transform godPoint)
    {
        m_GodPoint = godPoint;
    }

    public List<GodBase> GodList => m_GodList;

    public static GodManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<GodManager>() ?? new GameObject().AddComponent<GodManager>();
            }

            return s_Instance;
        }
    }
    private static GodManager s_Instance;

    protected void Awake()
    {
        
    }

    /// <summary>
    /// 内存释放函数
    /// </summary>
    public void OnClear()
    {
        
    }

    /// <summary>
    /// 从对象池中获取God对象
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns></returns>
    public async Task<GodBase> GetGodByName(string name)
    {
        GodInfo godInfo = GodDispositionManager.instance.GetGodByName(name);
        ObjectPool GodPool = GetGodPool(name);
        GodBase God = await GodPool.GetAsync(name) as GodBase;
        God.transform.position = m_GodPoint.position;
        God.OnInit(godInfo);
        GodList.Add(God);
        return God;
    }

    internal void OnUpdate()
    {

    }

    /// <summary>
    /// 从对象池中移除God对象
    /// </summary>
    /// <param name="name">对象池ID名称</param>
    /// <param GodBase="God">移除的对象</param>
    public void RemoveGod(string name, GodBase God)
    {
        ObjectPool GodPool = GetGodPool(name);
        GodList.Remove(God);
        GodPool.Remove(God);
    }

    /// <summary>
    /// 获得God对象池
    /// </summary>
    /// <param name="name">God对象池的名称</param>
    /// <returns></returns>
    private ObjectPool GetGodPool(string name)
    {
        if (!m_GodPoolDic.TryGetValue(name, out ObjectPool GodPool))
        {
            GodPool = new ObjectPool(OnGodConstruct, OnGodDestroy, OnGodEnabled, OnGodDisabled);
            m_GodPoolDic.Add(name, GodPool);
        }

        return GodPool;
    }

    /// <summary>
    /// 取消显示God
    /// </summary>
    /// <param name="arg1">God对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnGodDisabled(object arg1, object arg2)
    {
        GodBase God = arg1 as GodBase;
        God.gameObject.SetActive(false);
        God.transform.SetParent(transform);
    }

    /// <summary>
    /// 显示God
    /// </summary>
    /// <param name="arg1">God对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnGodEnabled(object arg1, object arg2)
    {
        GodBase God = arg1 as GodBase;
        God.gameObject.SetActive(true);
    }

    /// <summary>
    /// 摧毁God
    /// </summary>
    /// <param name="arg1">God对象</param>
    /// <param name="arg2">额外参数</param>
    private void OnGodDestroy(object arg1, object arg2)
    {
        GodBase God = arg1 as GodBase;
        Destroy(God);
    }

    /// <summary>
    /// God生成构造函数
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private async Task<object> OnGodConstruct(object arg)
    {
        string name = arg.ToString();
        GameObject gameObject;
        GodInfo God = GodDispositionManager.instance.GetGodByName(name);

        if (!m_GodPrefabDic.ContainsKey(name))
        {
            string path = God.prefabPath;
            if (!string.IsNullOrEmpty(path))
            {
                gameObject = await ResourceService.LoadAsync<GameObject>(path);
                if (gameObject) m_GodPrefabDic.Add(name, gameObject);
                else return null;
            }
        }
        gameObject = m_GodPrefabDic[name];
        if (gameObject == null)
        {
            Debug.LogError("从资源中读取God资源失败");
            return null;
        }
        GameObject go = Instantiate(gameObject, transform);
        go.SetActive(false);
        GodBase god = go.GetComponent<GodBase>();
        return god;
    }

    internal Transform GetInitPoint()
    {
        return m_GodPoint;
    }
}