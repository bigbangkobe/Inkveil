using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using LitJson;
using UnityEngine;

public class BagManager : Singleton<BagManager>
{
    private List<BagItemInfo> bagItemInfos = new List<BagItemInfo>();
    private const string BAG_KEY = "Bag";

    public List<BagItemInfo> ItemInfos => bagItemInfos; // 只读访问接口

    public void OnInit()
    {
        string str = PlayerPrefs.GetString(BAG_KEY);
        if (!string.IsNullOrEmpty(str))
        {
            bagItemInfos = JsonMapper.ToObject<List<BagItemInfo>>(str);
            SortBag(); // 初始化时排序
        }
    }

    // 添加物品到背包
    public void AddItem(PropertyInfo property)
    {
        // 检查是否存在相同ID的物品
        BagItemInfo existingItem = bagItemInfos.Find(item =>
            item.propertyInfo.propertyID == property.propertyID);

        if (existingItem != null)
        {
            // 叠加数量
            existingItem.propertyInfo.number += property.number;
        }
        else
        {
            if (property.propertyID == 1)
            {
                PlayerDispositionManager.instance.AddXianH(property.number);
            }
            else
            {  
                // 创建新物品项
                bagItemInfos.Add(new BagItemInfo()
                {
                    propertyInfo = property,
                    isLock = false,
                    isNew = true
                });
            }   
        }
        SortAndSaveBag(); // 添加后排序并保存
    }

    // 添加物品到背包
    public void AddItem(BagItemInfo property)
    {
        // 检查是否存在相同ID的物品
        BagItemInfo existingItem = bagItemInfos.Find(item =>
            item.propertyInfo.propertyID == property.propertyInfo.propertyID);

        if (existingItem != null)
        {
            // 叠加数量
            existingItem.propertyInfo.number += property.propertyInfo.number;
        }
        else
        {
            // 创建新物品项
            bagItemInfos.Add(new BagItemInfo()
            {
                propertyInfo = property.propertyInfo,
                isLock = false,
                isNew = true
            });
        }
        SortAndSaveBag(); // 添加后排序并保存
    }

    // 移除物品
    public bool RemoveItem(int propertyID, int amount = 1)
    {
        BagItemInfo item = bagItemInfos.Find(i =>
            i.propertyInfo.propertyID == propertyID);

        if (item == null) return false;

        if (item.isLock)
        {
            Debug.LogWarning($"Cannot remove locked item: {propertyID}");
            return false;
        }

        if (item.propertyInfo.number > amount)
        {
            // 减少数量
            item.propertyInfo.number -= amount;
        }
        else
        {
            // 完全移除
            bagItemInfos.Remove(item);
        }

        SortAndSaveBag(); // 移除后排序并保存
        return true;
    }

    public bool UserItem(int propertyID, int amount = 1)
    {
        BagItemInfo item = bagItemInfos.Find(i =>
            i.propertyInfo.propertyID == propertyID);

        if (item == null) 
        {
            HintPopPanelManager.instance.ShowHintPop($"{item.propertyInfo.propertyDes}不足！");
            return false; 
        }

        if (item.propertyInfo.number >= amount)
        {
            // 减少数量
            item.propertyInfo.number -= amount;

            // 完全移除
            if (item.propertyInfo.number <= 0) bagItemInfos.Remove(item);
        }
        else
        {
            HintPopPanelManager.instance.ShowHintPop($"{item.propertyInfo.propertyDes}不足！");
            return false;
        }

        PlayerDispositionManager.instance.onPlayerAssetsChangde?.Invoke();
        SortAndSaveBag(); // 移除后排序并保存
        return true;
    }

    // 更新物品状态
    public void UpdateItemState(int propertyID, bool? isLock = null, bool? isNew = null)
    {
        BagItemInfo item = bagItemInfos.Find(i =>
            i.propertyInfo.propertyID == propertyID);

        if (item != null)
        {
            if (isLock.HasValue) item.isLock = isLock.Value;
            if (isNew.HasValue) item.isNew = isNew.Value;
            SortAndSaveBag(); // 状态更新后排序并保存
        }
    }

    // 获取特定物品
    public BagItemInfo GetItem(int propertyID)
    {
        return bagItemInfos.Find(item =>
            item.propertyInfo.propertyID == propertyID);
    }

    // 排序背包（propertyGrade大的排前面）
    private void SortBag()
    {
        bagItemInfos = bagItemInfos
            .OrderByDescending(item => item.propertyInfo.propertyGrade) // 按品质降序
            .ThenByDescending(item => item.propertyInfo.propertyID)    // 次排序条件：ID降序
            .ToList();
    }

    // 排序并保存
    private void SortAndSaveBag()
    {
        SortBag();
        SaveBag();
    }

    // 保存背包数据
    private void SaveBag()
    {
        string json = JsonMapper.ToJson(bagItemInfos);
        PlayerPrefs.SetString(BAG_KEY, json);
        PlayerPrefs.Save();
    }

    // 清空测试数据
    public void ClearTestData()
    {
        PlayerPrefs.DeleteKey(BAG_KEY);
        bagItemInfos.Clear();
    }

    internal void Clear()
    {
        
    }
}