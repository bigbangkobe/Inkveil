
using System;

/// <summary>
/// 资源json数据类
/// </summary>
[Serializable]
public class ResourceInfo
{
    /// <summary>
    /// 模板ID
    /// </summary>
    public int TemplateId;

    /// <summary>
    /// 资源名称
    /// </summary>
    public string Name;

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc;

    /// <summary>
    /// UniStormID
    /// </summary>
    public int UniStormId;

    /// <summary>
    /// 资源路径(Resource资源目录下)
    /// </summary>
    public string Path;

    public ResourceInfo() { }
}