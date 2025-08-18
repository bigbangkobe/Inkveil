using System;
using System.Collections.Generic;
using Framework;
using LitJson;
using UnityEngine;

public class ShopDispositionManager : Singleton<ShopDispositionManager>
{
    private const string SHOP_PROGRESS_KEY = "ShopProgress";
    private const string SHOP_INFO_KEY = "ShopInfo";

    // 多维度数据存储
    private Dictionary<int, ShopInfo> m_IdShopDict = new Dictionary<int, ShopInfo>();
    private Dictionary<string, ShopInfo> m_NameShopDict = new Dictionary<string, ShopInfo>();

    // 资源管理
    private TextAsset m_ConfigAsset;
    private bool m_IsInitialized = false;

    public List<ShopInfo> ShopList { get; private set; }

    /// <summary>
    /// 初始化商品配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;

        string shopProgess = PlayerPrefs.GetString(SHOP_PROGRESS_KEY);
        string ShopInfo = PlayerPrefs.GetString(SHOP_INFO_KEY);

        bool isUpdate = !string.IsNullOrEmpty(shopProgess) && !shopProgess.Equals(DateTime.Now.ToString("yyyy-MM-dd"));
        if (isUpdate) PlayerPrefs.SetString(SHOP_PROGRESS_KEY, DateTime.Now.ToString("yyyy-MM-dd"));

        try
        {
            m_ConfigAsset = await ResourceService.LoadAsync<TextAsset>(ConfigDefine.shopInfo);

            if (m_ConfigAsset == null && string.IsNullOrEmpty(ShopInfo))
            {
                Debug.LogError($"商品配置文件加载失败：{ConfigDefine.shopInfo}");
                return;
            }

            ShopList = JsonMapper.ToObject<List<ShopInfo>>( m_ConfigAsset.text);
            List<ShopInfo> ShopListLocal = JsonMapper.ToObject<List<ShopInfo>>(!string.IsNullOrEmpty(ShopInfo) ? ShopInfo : m_ConfigAsset.text);
            if (ShopList == null || ShopList.Count == 0)
            {
                Debug.LogError("商品配置数据解析失败");
                return;
            }

            for (int i = 0; i < ShopList.Count; i++) {
                // 数据校验
                if (m_IdShopDict.ContainsKey(ShopList[i].shopID))
                {
                    Debug.LogError($"商品ID重复：{ShopList[i].shopID}");
                    continue;
                }

                if (string.IsNullOrEmpty(ShopList[i].itemName))
                {
                    Debug.LogWarning("发现无名商品配置，已跳过");
                    continue;
                }
                if (isUpdate) ShopList[i].isTodayBuy = false;
                else  
                {
                    if(ShopListLocal.Count > i)
                    ShopList[i].isTodayBuy = ShopListLocal[i].isTodayBuy;
                }
               
                // 建立索引
                m_IdShopDict[ShopList[i].shopID] = ShopList[i];
                m_NameShopDict[ShopList[i].itemName] = ShopList[i];
            }
            PlayerPrefs.SetString(SHOP_INFO_KEY, JsonMapper.ToJson(ShopList));
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
        return m_NameShopDict.TryGetValue(name, out var Shop) ? Shop : null;
    }

    /// <summary>
    /// 按ID获取商品配置
    /// </summary>
    public ShopInfo GetShopById(int id)
    {
        if (!m_IsInitialized) return null;
        return m_IdShopDict.TryGetValue(id, out var Shop) ? Shop : null;
    }

    public void SaveShopInfo() 
    {
        PlayerPrefs.SetString(SHOP_INFO_KEY, JsonMapper.ToJson(ShopList));
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

        m_IdShopDict.Clear();
        m_NameShopDict.Clear();
        m_IsInitialized = false;
    }
}