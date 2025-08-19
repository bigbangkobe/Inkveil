using System;

[Serializable]
public class BagItemInfo
{
    public PropertyInfo propertyInfo;

    public bool isLock;
    public bool isNew;

    public BagItemInfo Clone()
    {
        return new BagItemInfo
        {
            propertyInfo = propertyInfo?.Clone(),
            isLock = isLock,
            isNew = isNew
        };
    }
}
