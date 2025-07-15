[System.Serializable()]
public class ActivityInfo
{
    
    // id
    public int TemplateId;
    
    // 名字
    public string Name;
    
    // 描述
    public string Desc;
    
    // 时间
    public string Time;
    
    // 按钮描述
    public string Button;
    
    // 图标Id
    public int IconId;
    
    // 精灵Id
    public int SpriteId;
    
    // 场景Id
    public int SceneId;
    
    // 面板Id
    public int PanelId;
    
    // UI事件(id,msg|id,msg...)
    public string UIEvent;
}
