using UnityEngine;
using UnityEngine.Profiling;
using Framework;

/// <summary>
/// 统计信息
/// </summary>
public sealed class DebugStats : DebugObject
{
    private const string TITLE = "Stats";
    /// <summary>
    /// 存储单位
    /// </summary>
    private static string[] UNITS = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
    private const string FORMAT_UNITS = "{0}{1}";
    private const string FORMAT_STRING = "f1";
    private const string FORMAT_DEVICE = "Device:\t{0}";
    private const string FORMAT_OS = "OS:\t{0}";
    private const string FORMAT_CPU = "CPU:\t{0} ({1}Ghz)";
    private const string FORMAT_GPU = "GPU:\t{0} ({1})";
    private const string FORMAT_RAM = "RAM:\t{0} (Used:{1})";
    private const string FORMAT_GRAM = "GRAM:\t{0}";
    private const string FORMAT_FPS = "FPS:\t{0} (Frames:{1}\tTime:{2}s)";
    private const string FORMAT_PING = "PING:\t{0}ms\t(Receive:{1}\tSend:{2})";
    private const string FORMAT_BTY = "BTY:\t{0}\tWIFI:\t{1}";
    private const string FORMAT_PATH = "PATH:\t{0}";
    private const string FORMAT_S_PATH = "S_PATH:\t{0}";
    private const string FORMAT_P_PATH = "P_PATH:\t{0}";
    private const string GUI_BOX = "Box";
    private const int UNIT_MOD = 1024;
    private const float UNIT_GHZ = 0.001f;
    private const float UNIT_MSSECOND = 1000;
    private const long UNIT_MB = 1024L * 1024L;

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
    private Vector2 m_ScrollPosition = Vector2.zero;

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

        m_Time += Time.deltaTime;
        ++m_Frame;
        if (m_Time < interval)
        {
            return;
        }

        fps = (int)(m_Frame / m_Time);
        m_Time = 0;
        m_Frame = 0;
    }

    /// <summary>
    /// 绘制回调
    /// </summary>
    public override void OnGUI()
    {
        base.OnGUI();

        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUI_BOX);

        GUILayout.TextArea(string.Format(FORMAT_DEVICE, SystemInfo.deviceModel));
        GUILayout.TextArea(string.Format(FORMAT_OS, SystemInfo.operatingSystem));
        GUILayout.TextArea(string.Format(FORMAT_CPU, SystemInfo.processorType, (SystemInfo.processorFrequency * UNIT_GHZ).ToString(FORMAT_STRING)));
        GUILayout.TextArea(string.Format(FORMAT_GPU, SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion));
        GUILayout.TextArea(string.Format(FORMAT_RAM, GetUnit(SystemInfo.systemMemorySize * UNIT_MB, FORMAT_STRING), GetUnit(Profiler.GetTotalAllocatedMemoryLong(), FORMAT_STRING)));
        GUILayout.TextArea(string.Format(FORMAT_GRAM, GetUnit(SystemInfo.graphicsMemorySize * UNIT_MB, FORMAT_STRING)));
        GUILayout.TextArea(string.Format(FORMAT_FPS, fps, Time.frameCount, Time.time.ToString(FORMAT_STRING)));
        GUILayout.TextArea(string.Format(FORMAT_PING, GetPing(), GetUnit(GetReceiveBytes(), FORMAT_STRING), GetUnit(GetSendBytes(), FORMAT_STRING)));
        //GUILayout.TextArea(string.Format(FORMAT_BTY, NativeAPI.GetBatteryLevel(), NativeAPI.GetWifiLevel()));
        GUILayout.TextArea(string.Format(FORMAT_PATH, Application.dataPath));
        GUILayout.TextArea(string.Format(FORMAT_S_PATH, Application.streamingAssetsPath));
        GUILayout.TextArea(string.Format(FORMAT_P_PATH, Application.persistentDataPath));

        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 获取网络延迟
    /// </summary>
    /// <returns></returns>
    private int GetPing()
    {
        NetworkServer[] networkServers = NetworkSystem.GetAll();
        if (networkServers == null)
        {
            return 0;
        }

        int ping = 0;
        for (int i = 0; i < networkServers.Length; ++i)
        {
            NetworkServer networkServer = networkServers[i];
            ping += networkServer.ping;
        }
        ping /= networkServers.Length;

        return ping;
    }

    /// <summary>
    /// 获取接收字节数
    /// </summary>
    /// <returns></returns>
    private long GetReceiveBytes()
    {
        NetworkServer[] networkServers = NetworkSystem.GetAll();
        if (networkServers == null)
        {
            return 0;
        }

        long bytes = 0;
        for (int i = 0; i < networkServers.Length; ++i)
        {
            NetworkServer networkServer = networkServers[i];
            bytes += networkServer.receiveBytes;
        }

        return bytes;
    }

    /// <summary>
    /// 获取发送字节数
    /// </summary>
    /// <returns></returns>
    private long GetSendBytes()
    {
        NetworkServer[] networkServers = NetworkSystem.GetAll();
        if (networkServers == null)
        {
            return 0;
        }

        long bytes = 0;
        for (int i = 0; i < networkServers.Length; ++i)
        {
            NetworkServer networkServer = networkServers[i];
            bytes += networkServer.sendBytes;
        }

        return bytes;
    }

    /// <summary>
    /// 获取字节存储单位
    /// </summary>
    /// <param name="value">字节</param>
    /// <param name="format">字符串格式</param>
    /// <returns>返回字符串</returns>
    private string GetUnit(double value, string format = null)
    {
        int i = 0;
        while (value >= UNIT_MOD)
        {
            value /= UNIT_MOD;
            ++i;
        }

        return string.Format(FORMAT_UNITS, value.ToString(format), UNITS[i]);
    }
}

