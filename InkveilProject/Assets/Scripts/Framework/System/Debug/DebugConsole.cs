using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

/// <summary>
/// 后台信息
/// </summary>
public sealed class DebugConsole : DebugObject
{
    private const string TITLE = "Console";
    private const string FORMAT_CONSOLE = "[{0}] {1}\n{2}";
    private const string FORMAT_MENU = "{0} ({1})";
    private const string GUI_BOX = "Box";
    private const string GUI_OK = "OK";
    private const string GUI_CLEAR = "Clear";

    /// <summary>
    /// 日志数据表 [键:日志类型 值:日志数据]
    /// </summary>
    private static readonly Dictionary<LogType, LogData> s_LogDataMap = new Dictionary<LogType, LogData>()
    {
        { LogType.Log, new LogData(){ color = Color.white, open = true } },
        { LogType.Warning, new LogData(){ color = Color.yellow, open = false } },
        { LogType.Assert, new LogData(){ color = Color.yellow, open = false } },
        { LogType.Exception, new LogData(){ color = Color.red, open = true } },
        { LogType.Error, new LogData(){ color = Color.red, open = true } },
    };

    public override string title { get { return TITLE; } }

    /// <summary>
    /// 最大行数
    /// </summary>
    public int maxLine = 100;
    /// <summary>
    /// 输入提交回调
    /// </summary>
    public event Action<string> onInputSubmit;

    /// <summary>
    /// 日志链表
    /// </summary>
    private List<LogInfo> m_LogList = new List<LogInfo>();
    /// <summary>
    /// 文本内容
    /// </summary>
    private string m_InputText = string.Empty;
    /// <summary>
    /// 滚动位置
    /// </summary>
    private Vector2 m_ScrollPosition = Vector2.zero;

    /// <summary>
    /// 初始化回调
    /// </summary>
    public override void OnInit()
    {
        Application.logMessageReceivedThreaded += OnReceivedLog;
    }

    /// <summary>
    /// 销毁回调
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();

        Application.logMessageReceivedThreaded -= OnReceivedLog;
    }

    /// <summary>
    /// 绘制回调
    /// </summary>
    public override void OnGUI()
    {
        base.OnGUI();

        OnDrawMenu();
        OnDrawLog();
        OnDrawInput();
    }

    /// <summary>
    /// 绘制菜单
    /// </summary>
    private void OnDrawMenu()
    {
        GUILayout.BeginHorizontal(GUI_BOX);

        foreach (KeyValuePair<LogType, LogData> log in s_LogDataMap)
        {
            GUI.skin.button.normal.textColor = log.Value.open ? log.Value.color : Color.gray;
            if (GUILayout.Button(string.Format(FORMAT_MENU, log.Key, log.Value.count)))
            {
                log.Value.open = !log.Value.open;
            }

            log.Value.count = 0;
        }

        GUILayout.EndHorizontal();

        GUI.skin.button.normal.textColor = Color.white;
    }

    /// <summary>
    /// 绘制日志
    /// </summary>
    private void OnDrawLog()
    {
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUI_BOX);
        for (int i = 0; i < m_LogList.Count; ++i)
        {
            LogInfo logInfo = m_LogList[i];
            LogData logData = s_LogDataMap[logInfo.type];
            ++logData.count;
            if (logData.open)
            {
                GUI.skin.label.normal.textColor = logData.color;
                GUILayout.Label(logInfo.content);
            }
        }
        GUILayout.EndScrollView();

        GUI.skin.label.normal.textColor = Color.white;
    }

    /// <summary>
    /// 绘制输入框
    /// </summary>
    private void OnDrawInput()
    {
        GUILayout.BeginHorizontal(GUI_BOX);
        m_InputText = GUILayout.TextArea(m_InputText);
        GUILayoutOption option = GUILayout.Width(DebugSystem.size * 4);
        if (GUILayout.Button(GUI_OK, option))
        {
            if (onInputSubmit != null)
            {
                onInputSubmit.Invoke(m_InputText);
            }

            m_InputText = string.Empty;
        }
        else if (GUILayout.Button(GUI_CLEAR, option))
        {
            m_LogList.Clear();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 日志回调
    /// </summary>
    /// <param name="condition">日志信息</param>
    /// <param name="stackTrace">堆栈信息</param>
    /// <param name="type">日志类型</param>
    private void OnReceivedLog(string condition, string stackTrace, LogType type)
    {
        if (maxLine <= m_LogList.Count)
        {
            m_LogList.RemoveAt(0);
        }

        LogInfo logInfo = new LogInfo();
        logInfo.type = type;
        logInfo.content = string.Format(FORMAT_CONSOLE, type, condition, stackTrace);

        m_LogList.Add(logInfo);
    }

    /// <summary>
    /// 日志数据
    /// </summary>
    private class LogData
    {
        public Color color;
        public int count;
        public bool open;
    }

    /// <summary>
    /// 日志信息
    /// </summary>
    private class LogInfo
    {
        public LogType type;
        public string content;
    }
}
