using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using LitJson;
using UnityEngine;

public class ShopDispositionManager : Singleton<ShopDispositionManager>
{
    // 多维度数据存储
    private Dictionary<int, ShopInfo> m_IdShopDict = new Dictionary<int, ShopInfo>();
    private Dictionary<string, ShopInfo> m_NameShopDict = new Dictionary<string, ShopInfo>();

    // 配置数据
    public List<ShopInfo> ShopList { get; private set; } = new List<ShopInfo>();

    // 加载状态
    private bool m_IsInitialized = false;

    /// <summary>
    /// 异步初始化商品配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;
        try
        {
            // 异步加载配置文件
            var handle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.shopInfo);
            TextAsset asset = await handle.Task;

            if (asset == null)
            {
                Debug.LogError($"商品配置文件加载失败：{ConfigDefine.shopInfo}");
                return;
            }

            // 解析配置列表
            ShopList = JsonMapper.ToObject<List<ShopInfo>>(asset.text) ?? new List<ShopInfo>();
            if (ShopList.Count == 0)
            {
                Debug.LogError("商品配置数据解析失败");
                return;
            }

            // 建立索引
            m_IdShopDict.Clear();
            m_NameShopDict.Clear();
            foreach (var item in ShopList)
            {
                if (m_IdShopDict.ContainsKey(item.shopID))
                {
                    Debug.LogError($"商品ID重复：{item.shopID}");
                    continue;
                }
                if (string.IsNullOrEmpty(item.itemName))
                {
                    Debug.LogWarning("发现无名商品配置，已跳过");
                    continue;
                }
                m_IdShopDict[item.shopID] = item;
                m_NameShopDict[item.itemName] = item;
            }

            m_IsInitialized = true;
            Debug.Log($"商品配置加载完成，总计{ShopList.Count}件商品");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"商品配置初始化异常：{ex}");
        }
    }

    /// <summary>
    /// 按名称获取商品配置
    /// </summary>
    public ShopInfo GetShopByName(string name)
    {
        if (!m_IsInitialized) return null;
        return m_NameShopDict.TryGetValue(name, out var info) ? info : null;
    }

    /// <summary>
    /// 按ID获取商品配置
    /// </summary>
    public ShopInfo GetShopById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdShopDict.TryGetValue(id, out var info) ? info : null;
    }

    /// <summary>
    /// 清理系统资源
    /// </summary>
    public void Clear()
    {
        m_IdShopDict.Clear();
        m_NameShopDict.Clear();
        ShopList.Clear();
        m_IsInitialized = false;
    }
}
