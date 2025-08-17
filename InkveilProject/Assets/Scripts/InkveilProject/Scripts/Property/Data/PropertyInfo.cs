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
/// ������������
/// </summary>
public enum PropertyIDType
{
    theFairyFate = 1,  //��Ե
    soulInk = 2,        //��ī
    pleaseDivineOrder = 3,      //������
    refinedIron = 4,        //����
    shard = 5,          //������Ƭ
    sunWuKongHeroShard = 6,         //���������Ƭ
    neZhaHeroShard = 7,         //��߸����Ƭ
    yangJianHeroShard = 8,          //�������Ƭ
    guanYuHeroShard = 9,            //��������Ƭ
    shangGuangWanErHeroShard = 10,      //�Ϲ��������Ƭ
    shiWeiTianHuHeroShard = 11,         //ʮβ�������Ƭ
}
