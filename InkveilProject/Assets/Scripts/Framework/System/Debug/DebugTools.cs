using UnityEngine;
using UnityEngine.Profiling;
using Framework;

/// <summary>
/// Debug工具栏
/// </summary>
public sealed class DebugTools : DebugObject
{
    private const string TITLE = "Tools";

    /// <summary>
    /// Debug工具枚举型
    /// </summary>

    public enum DebugToolsType
    {
        Profiler = 0,       //生成手机Profiler
    }

    /// <summary>
    /// 生成手机Profiler按钮
    /// </summary>
    private const string GUI_PROFILER = "Profiler";

    public override string title { get { return TITLE; } }
    /// <summary>
    /// 间隔时间
    /// </summary>
    public float interval { get; set; }
    /// <summary>
    /// 当前帧率
    /// </summary>
    public int fps { get; private set; }

    /// <summary>
    /// 累积时间
    /// </summary>
    private float m_Time;
    /// <summary>
    /// 累积帧数
    /// </summary>
    private int m_Frame;
    /// <summary>
    /// 滚动位置
    /// </summary>
    //private Vector2 m_ScrollPosition = Vector2.zero;

    /// <summary>
    /// 初始化回调
    /// </summary>
    public override void OnInit()
    {
        interval = 1.0f / 3;
    }

    /// <summary>
    /// 更新回调
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    /// <summary>
    /// 绘制回调
    /// </summary>
    public override void OnGUI()
    {
        base.OnGUI();

        OnDrawMenu();
    }

    /// <summary>
    /// 绘制菜单
    /// </summary>
    private void OnDrawMenu()
    {
        if (GUILayout.Button(GUI_PROFILER))
        {
            ProfilerUtils.instance.BeginRecord();
            Debug.Log("按钮");
        }
    }

 

}

