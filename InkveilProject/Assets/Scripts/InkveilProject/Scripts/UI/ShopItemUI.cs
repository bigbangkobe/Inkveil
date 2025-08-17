using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] Text m_Title;
    [SerializeField] Image m_Icon;
    [SerializeField] Text m_Number;
    [SerializeField] Text m_PriceNumber;
    [SerializeField] Button m_Button;
    [SerializeField] GameObject m_luck;

    public bool isIcon = true;

    private ShopInfo shopInfo;
    private PropertyInfo propertyInfo;

    private void Awake()
    {
      
        m_Button.onClick.AddListener(OnBuyClick);
    }

    private void OnBuyClick()
    {
        PurchaseResult purchaseResult = ShopManager.instance.PurchaseItem(shopInfo.shopID);

        switch (purchaseResult)
        {
            case PurchaseResult.Success:
                propertyInfo.number = shopInfo.number;
                shopInfo.isTodayBuy= true;
                BagManager.instance.AddItem(propertyInfo);
                ShopDispositionManager.instance.SaveShopInfo();
                Debug.Log("¹ºÂò³É¹¦");
                if (shopInfo.limitCount == 2)
                {
                    m_luck.gameObject.SetActive(true);
                }
              
                break;
            case PurchaseResult.ItemNotFound:
                break;
            case PurchaseResult.LimitExceeded:
                break;
            case PurchaseResult.NotEnoughCurrency:
                break;
            case PurchaseResult.InventoryFull:
                break;
            default:
                break;
        }
    }

    public async void OnInit(ShopInfo shop) 
    {
        shopInfo = shop;
        propertyInfo = shop.GetPropertyInfo();
        if (shopInfo.isTodayBuy && shopInfo.limitCount == 2) 
        {
            m_luck.gameObject.SetActive(true);
        }
        m_Title.text = shopInfo.itemName;
        m_Number.text = "X" + shopInfo.number.ToString();
        m_PriceNumber.text = "X" + shopInfo.price.ToString();
        try
        {
            Debug.Log("imagePath:" + propertyInfo.imagePath);
            Sprite iconSprite = await ResourceService.LoadAsync<Sprite>(propertyInfo.imagePath);
            if (iconSprite != null && isIcon)
            {
                m_Icon.sprite = iconSprite;
            }
        }
        catch (Exception e)
        {
            Debug.Log("imagePath:" + propertyInfo.imagePath);
            Debug.Log(e.ToString());
        }
    }
}