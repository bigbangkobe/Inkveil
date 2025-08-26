using Framework;
using UnityEngine;

public class HintPopPanelManager : MonoSingleton<HintPopPanelManager>
{
    private HintPopPanel hintPopPanel;

    public void OnInit() { }

    protected async override void Awake()
    {
        base.Awake();

        GameObject hint = (await ResourceService.LoadAsync<GameObject>("UI/HintPopPanel"));
        hintPopPanel = Instantiate(hint, transform).GetComponent<HintPopPanel>();
        hintPopPanel.gameObject.SetActive(false);
    }

    public void ShowHintPop(string content) 
    {
        hintPopPanel?.ShowHint(content);
    }

    public string PropertyIDTypeName(int propertyID)
    {
        string propertyName = "";
        switch ((PropertyIDType)propertyID)
        {
            case PropertyIDType.theFairyFate:
                propertyName = "��Ե";
                break;
            case PropertyIDType.soulInk:
                propertyName = "��ī";
                break;
            case PropertyIDType.pleaseDivineOrder:
                propertyName = "��Ը��";
                break;
            case PropertyIDType.refinedIron:
                propertyName = "����";
                break;
            case PropertyIDType.shard:
                propertyName = "������Ƭ";
                break;
            case PropertyIDType.sunWuKongHeroShard:
                propertyName = "���������Ƭ";
                break;
            case PropertyIDType.neZhaHeroShard:
                propertyName = "��߸����Ƭ";
                break;
            case PropertyIDType.yangJianHeroShard:
                propertyName = "�������Ƭ";
                break;
            case PropertyIDType.guanYuHeroShard:
                propertyName = "��������Ƭ";
                break;
            case PropertyIDType.shangGuangWanErHeroShard:
                propertyName = "�Ϲ��������Ƭ";
                break;
            case PropertyIDType.shiWeiTianHuHeroShard:
                propertyName = "ʮβ�������Ƭ";
                break;
        }
        return propertyName;
    }
}