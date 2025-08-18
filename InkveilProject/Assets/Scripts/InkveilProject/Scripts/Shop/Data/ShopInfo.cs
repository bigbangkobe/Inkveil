public class ShopInfo 
{
    public int shopID;
    public string itemName;
    public int price;
    public int number;
    public int limitCount;
    public int propertyID;
    public int isGuanGAO;
    public string obtainMethod;
    public string description;
    public int buyCount = 0;
    public bool isTodayBuy = false;

    private PropertyInfo propertyInfo;
    public PropertyInfo GetPropertyInfo()
    {
        if (propertyInfo == null)
        {
            propertyInfo = PropertyDispositionManager.instance.GetPropertyById(propertyID);
        }
        return propertyInfo;
    }
}