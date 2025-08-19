using Framework;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : Singleton<BagManager>
{
    private List<BagItemInfo> bagItemInfos = new List<BagItemInfo>();
    private const string BAG_KEY = "Bag";

    // 使用属性而不是公共字段
    public IReadOnlyList<BagItemInfo> ItemInfos => bagItemInfos.AsReadOnly();

    public void OnInit()
    {
        string str = PlayerPrefs.GetString(BAG_KEY);
        if (!string.IsNullOrEmpty(str))
        {
            try
            {
                bagItemInfos = JsonMapper.ToObject<List<BagItemInfo>>(str) ?? new List<BagItemInfo>();
                SortBag();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load bag data: {ex.Message}");
                bagItemInfos = new List<BagItemInfo>();
            }
        }
    }

    // 添加物品到背包（由 PropertyInfo）
    public void AddItem(PropertyInfo property)
    {
        if (property == null)
        {
            Debug.LogWarning("Attempted to add null property to bag");
            return;
        }

        // 处理特殊 ID
        if (property.propertyID == 1)
        {
            PlayerDispositionManager.instance?.AddXianH(property.number);
        }

        // 查找已有条目
        BagItemInfo existingItem = bagItemInfos.Find(item =>
            item?.propertyInfo != null && item.propertyInfo.propertyID == property.propertyID);

        if (existingItem != null)
        {
            existingItem.propertyInfo.number += property.number;
        }
        else
        {
            // 创建新物品项
            bagItemInfos.Add(new BagItemInfo
            {
                propertyInfo = property.Clone(), // 需要确保 Clone 方法正确实现
                isLock = false,
                isNew = true
            });
        }

        SortAndSaveBag();
    }

    // 添加物品到背包（由 BagItemInfo）
    public void AddItem(BagItemInfo itemToAdd)
    {
        if (itemToAdd == null || itemToAdd.propertyInfo == null)
        {
            Debug.LogWarning("Attempted to add null item to bag");
            return;
        }

        if (itemToAdd.propertyInfo.propertyID == 1)
        {
            PlayerDispositionManager.instance?.AddXianH(itemToAdd.propertyInfo.number);
        }

        BagItemInfo existingItem = bagItemInfos.Find(item =>
            item?.propertyInfo != null &&
            item.propertyInfo.propertyID == itemToAdd.propertyInfo.propertyID);

        if (existingItem != null)
        {
            existingItem.propertyInfo.number += itemToAdd.propertyInfo.number;
        }
        else
        {
            // 创建新物品项
            bagItemInfos.Add(new BagItemInfo
            {
                propertyInfo = itemToAdd.propertyInfo.Clone(), // 需要确保 Clone 方法正确实现
                isLock = itemToAdd.isLock,
                isNew = true // 新添加的物品总是标记为新
            });
        }

        SortAndSaveBag();
    }

    // 移除物品
    public bool RemoveItem(int propertyID, int amount = 1)
    {
        if (amount <= 0) return false;

        BagItemInfo item = bagItemInfos.Find(i =>
            i?.propertyInfo != null && i.propertyInfo.propertyID == propertyID);

        if (item == null) return false;

        if (item.isLock)
        {
            Debug.LogWarning($"Cannot remove locked item: {propertyID}");
            return false;
        }

        if (item.propertyInfo.number > amount)
        {
            item.propertyInfo.number -= amount;
        }
        else
        {
            bagItemInfos.Remove(item);
        }

        SortAndSaveBag();
        return true;
    }

    // 使用物品
    public bool UseItem(int propertyID, int amount = 1)
    {
        if (amount <= 0) return false;

        BagItemInfo item = bagItemInfos.Find(i =>
            i?.propertyInfo != null && i.propertyInfo.propertyID == propertyID);

        if (item == null)
        {
            HintPopPanelManager.instance?.ShowHintPop(
                $"{HintPopPanelManager.instance?.PropertyIDTypeName(propertyID) ?? "物品"}不足！");
            return false;
        }

        if (item.propertyInfo.number >= amount)
        {
            item.propertyInfo.number -= amount;
            if (item.propertyInfo.number <= 0)
                bagItemInfos.Remove(item);
        }
        else
        {
            HintPopPanelManager.instance?.ShowHintPop($"{item.propertyInfo.propertyDes}不足！");
            return false;
        }

        SortAndSaveBag();
        return true;
    }

    // 更新物品状态
    public void UpdateItemState(int propertyID, bool? isLock = null, bool? isNew = null)
    {
        BagItemInfo item = bagItemInfos.Find(i =>
            i?.propertyInfo != null && i.propertyInfo.propertyID == propertyID);

        if (item != null)
        {
            if (isLock.HasValue) item.isLock = isLock.Value;
            if (isNew.HasValue) item.isNew = isNew.Value;
            SortAndSaveBag();
        }
    }

    // 获取特定物品
    public BagItemInfo GetItem(int propertyID)
    {
        return bagItemInfos.Find(item =>
            item?.propertyInfo != null && item.propertyInfo.propertyID == propertyID);
    }

    // 改进的排序比较器
    private static readonly Comparison<BagItemInfo> BagComparison = (a, b) =>
    {
        // 处理 null 情况
        if (a == null && b == null) return 0;
        if (a == null) return 1;
        if (b == null) return -1;

        // 处理 propertyInfo 为 null 的情况
        if (a.propertyInfo == null && b.propertyInfo == null) return 0;
        if (a.propertyInfo == null) return 1;
        if (b.propertyInfo == null) return -1;

        // 先按品质降序
        int gradeCompare = b.propertyInfo.propertyGrade.CompareTo(a.propertyInfo.propertyGrade);
        if (gradeCompare != 0) return gradeCompare;

        // 再按 ID 降序
        return b.propertyInfo.propertyID.CompareTo(a.propertyInfo.propertyID);
    };

    private void SortBag()
    {
        // 先移除所有 null 项
        bagItemInfos.RemoveAll(item => item == null || item.propertyInfo == null);
        bagItemInfos.Sort(BagComparison);
    }

    private void SortAndSaveBag()
    {
        SortBag();
        SaveBag();
    }

    // 保存背包数据
    private void SaveBag()
    {
        try
        {
            PlayerDispositionManager.instance?.onPlayerAssetsChangde?.Invoke();
            string json = JsonMapper.ToJson(bagItemInfos);
            PlayerPrefs.SetString(BAG_KEY, json);
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save bag data: {ex.Message}");
        }
    }

    // 批量操作优化
    public void BatchOperation(Action<BagManager> operation)
    {
        operation?.Invoke(this);
        SortAndSaveBag();
    }

    // 清空测试数据
    public void ClearTestData()
    {
        PlayerPrefs.DeleteKey(BAG_KEY);
        bagItemInfos.Clear();
        PlayerDispositionManager.instance?.onPlayerAssetsChangde?.Invoke();
    }

    internal void Clear()
    {
        bagItemInfos.Clear();
        PlayerPrefs.DeleteKey(BAG_KEY);
        PlayerDispositionManager.instance?.onPlayerAssetsChangde?.Invoke();
    }
}