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
}

/// <summary>
/// 背包道具类型
/// </summary>
public enum PropertyIDType
{
    theFairyFate = 1,  //仙缘
    soulInk = 2,        //魂墨
    pleaseDivineOrder = 3,      //请神令
    refinedIron = 4,        //精铁
    shard = 5,          //法宝碎片
    sunWuKongHeroShard = 6,         //孙悟空神将碎片
    neZhaHeroShard = 7,         //哪吒神将碎片
    yangJianHeroShard = 8,          //杨戬神将碎片
    guanYuHeroShard = 9,            //关羽神将碎片
    shangGuangWanErHeroShard = 10,      //上官婉儿神将碎片
    shiWeiTianHuHeroShard = 11,         //十尾天狐神将碎片
}
