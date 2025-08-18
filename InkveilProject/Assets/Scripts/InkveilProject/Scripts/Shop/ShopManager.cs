using Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    // 已购买商品记录（商品ID -> 已购买次数）
    private Dictionary<int, int> m_PurchaseRecords = new Dictionary<int, int>();

    /// <summary>
    /// 初始化商店系统
    /// </summary>
    public void OnInit()
    {
        // 确保配置管理器初始化
        ShopDispositionManager.instance.OnInit();

        // 加载购买记录（实际项目中应从持久化存储加载）
        LoadPurchaseRecords();
    }

    /// <summary>
    /// 尝试购买商品
    /// </summary>
    /// <param name="shopId">商品ID</param>
    /// <param name="quantity">购买数量</param>
    /// <returns>购买结果状态</returns>
    public PurchaseResult PurchaseItem(int shopId,int isGuanGAO, int quantity = 1)
    {
        // 获取商品配置
        ShopInfo item = ShopDispositionManager.instance.GetShopById(shopId);
        if (item == null)
            return PurchaseResult.ItemNotFound;

        // 检查购买限制
        if (item.limitCount == -1)
        {
            int purchased = GetPurchasedCount(shopId);
            if (purchased + quantity > item.limitCount)
                return PurchaseResult.LimitExceeded;
        }

        if (isGuanGAO == 1)
        {
            AddPurchaseRecord(shopId, quantity);
            return PurchaseResult.Success;
        }

        // 检查货币是否充足（实际项目需接入货币系统）
        if (!PlayerDispositionManager.instance.HasEnoughCurrency(item.price * quantity))
            return PurchaseResult.NotEnoughCurrency;

        // 执行购买
        PlayerDispositionManager.instance.DeductCurrency(item.price * quantity);
        AddPurchaseRecord(shopId, quantity);

        //DistributeItem(item, quantity);

        return PurchaseResult.Success;
    }

    /// <summary>
    /// 获取商品已购买次数
    /// </summary>
    public int GetPurchasedCount(int shopId)
    {
        return m_PurchaseRecords.TryGetValue(shopId, out int count) ? count : 0;
    }

    /// <summary>
    /// 获取剩余可购买次数
    /// </summary>
    public int GetRemainingPurchases(int shopId)
    {
        ShopInfo item = ShopDispositionManager.instance.GetShopById(shopId);
        if (item == null) return 0;

        int purchased = GetPurchasedCount(shopId);
        return Mathf.Max(0, item.limitCount - purchased);
    }

    /// <summary>
    /// 获取商店所有商品
    /// </summary>
    public List<ShopInfo> GetAllShopItems()
    {
        return ShopDispositionManager.instance.ShopList;
    }

    /// <summary>
    /// 清理商店数据
    /// </summary>
    public void Clear()
    {
        m_PurchaseRecords.Clear();
        // 实际项目中应保存购买记录到持久化存储
    }

    // ========== 内部方法 ==========
    private void LoadPurchaseRecords()
    {
        // TODO: 从PlayerPrefs/云存储/本地文件加载购买记录
        // 示例：m_PurchaseRecords = SaveManager.Load<Dictionary<int, int>>("ShopRecords");
    }

    private void SavePurchaseRecords()
    {
        // TODO: 保存购买记录到持久化存储
        // SaveManager.Save("ShopRecords", m_PurchaseRecords);
    }

    private void AddPurchaseRecord(int shopId, int quantity)
    {
        if (m_PurchaseRecords.ContainsKey(shopId))
            m_PurchaseRecords[shopId] += quantity;
        else
            m_PurchaseRecords[shopId] = quantity;

        SavePurchaseRecords();
    }
}

/// <summary>
/// 购买结果状态
/// </summary>
public enum PurchaseResult
{
    Success,
    ItemNotFound,
    LimitExceeded,
    NotEnoughCurrency,
    InventoryFull
}