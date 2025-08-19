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

    // �� PropertyInfo ����
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
/// ������������
/// </summary>
public enum PropertyIDType
{
    theFairyFate = 1,  //��Ե
    soulInk = 999,        //��ī
    pleaseDivineOrder = 2,      //������
    refinedIron = 3,        //����
    shard = 4,          //������Ƭ
    sunWuKongHeroShard = 5,         //���������Ƭ
    neZhaHeroShard = 6,         //��߸����Ƭ
    yangJianHeroShard = 7,          //�������Ƭ
    guanYuHeroShard = 8,            //��������Ƭ
    shangGuangWanErHeroShard = 9,      //�Ϲ��������Ƭ
    shiWeiTianHuHeroShard = 10,         //ʮβ�������Ƭ
}
