using System;

[Serializable]
public class PropertyInfo
{
    public int propertyID { get; set; }
    public string propertyName { get; set; }
    public string propertyDes { get; set; }
    public int propertyGrade { get; set; }
    public string imagePath { get; set; }
    public int number { get; set; } = 0;

    public PropertyInfo() { }

    public PropertyInfo(int propertyID, string propertyName, string propertyDes, int propertyGrade, string imagePath, int number)
    {
        this.propertyID = propertyID;
        this.propertyName = propertyName;
        this.propertyDes = propertyDes;
        this.propertyGrade = propertyGrade;
        this.imagePath = imagePath;
        this.number = number;
    }

    // 在 PropertyInfo 类中
    public PropertyInfo Clone()
    {
        return new PropertyInfo
        {
            propertyID = this.propertyID,
            propertyName = this.propertyName,
            propertyDes = this.propertyDes,
            propertyGrade = this.propertyGrade,
            imagePath = this.imagePath,
            number = this.number
        };
    }
}

/// <summary>
/// 背包道具类型
/// </summary>
public enum PropertyIDType
{
    theFairyFate = 1,  //仙缘
    soulInk = 999,        //魂墨
    pleaseDivineOrder = 2,      //请神令
    refinedIron = 3,        //精铁
    shard = 4,          //法宝碎片
    sunWuKongHeroShard = 5,         //孙悟空神将碎片
    neZhaHeroShard = 6,         //哪吒神将碎片
    yangJianHeroShard = 7,          //杨戬神将碎片
    guanYuHeroShard = 8,            //关羽神将碎片
    shangGuangWanErHeroShard = 9,      //上官婉儿神将碎片
    shiWeiTianHuHeroShard = 10,         //十尾天狐神将碎片
}
