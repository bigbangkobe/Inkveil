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
                propertyName = "仙缘";
                break;
            case PropertyIDType.soulInk:
                propertyName = "魂墨";
                break;
            case PropertyIDType.pleaseDivineOrder:
                propertyName = "祈愿令";
                break;
            case PropertyIDType.refinedIron:
                propertyName = "精铁";
                break;
            case PropertyIDType.shard:
                propertyName = "法宝碎片";
                break;
            case PropertyIDType.sunWuKongHeroShard:
                propertyName = "孙悟空神将碎片";
                break;
            case PropertyIDType.neZhaHeroShard:
                propertyName = "哪吒神将碎片";
                break;
            case PropertyIDType.yangJianHeroShard:
                propertyName = "杨戬神将碎片";
                break;
            case PropertyIDType.guanYuHeroShard:
                propertyName = "关羽神将碎片";
                break;
            case PropertyIDType.shangGuangWanErHeroShard:
                propertyName = "上官婉儿神将碎片";
                break;
            case PropertyIDType.shiWeiTianHuHeroShard:
                propertyName = "十尾天狐神将碎片";
                break;
        }
        return propertyName;
    }
}