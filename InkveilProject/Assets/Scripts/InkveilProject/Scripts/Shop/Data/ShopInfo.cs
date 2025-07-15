public class ShopInfo 
{
    public int shopID;
    public string itemName;
    public int price;
    public int number;
    public int limitCount;
    public int propertyID;
    public string obtainMethod;
    public string description;

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