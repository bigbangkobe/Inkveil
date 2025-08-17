using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanelUI : MonoBehaviour
{
    private ShopItemUI[] shopItems;
    private List<ShopInfo> shopInfos;

    private void Awake()
    {
        shopItems = GetComponentsInChildren<ShopItemUI>();
        shopInfos = ShopManager.instance.GetAllShopItems();


        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].OnInit(shopInfos[i]);
        }
    }

    public void UpdateShop()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopInfos[i].isTodayBuy = false;
            shopItems[i].OnInit(shopInfos[i]);
            ShopDispositionManager.instance.SaveShopInfo();
        }
    }
}