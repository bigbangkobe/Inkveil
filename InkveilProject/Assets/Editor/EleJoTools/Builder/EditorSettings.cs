using UnityEditor;
using System.IO;

/// <summary>
/// 编辑器设定
/// </summary>
public static class EditorSettings
{
	private static readonly string PREFS_PREFIX = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
	private static readonly string PREFS_CONFIG_PATH = PREFS_PREFIX + "_ConfigPath";
	private static readonly string PREFS_PROTOCOL_PATH = PREFS_PREFIX + "_ProtocolPath";
	private static readonly string PREFS_AUTO_RUN_PLAYER = PREFS_PREFIX + "_AutoRunPlayer";

	/// <summary>
	/// 数据路径
	/// </summary>
	public static string configPath
	{
		get { return EditorPrefs.GetString(PREFS_CONFIG_PATH, string.Empty); }
		set { EditorPrefs.SetString(PREFS_CONFIG_PATH, value); }
	}

	/// <summary>
	/// 协议路径
	/// </summary>
	public static string protocolPath
	{
		get { return EditorPrefs.GetString(PREFS_PROTOCOL_PATH, string.Empty); }
		set { EditorPrefs.SetString(PREFS_PROTOCOL_PATH, value); }
	}

	/// <summary>
	/// 自动运行
	/// </summary>
	public static bool autoRunPlayer
	{
		get { return EditorPrefs.GetBool(PREFS_AUTO_RUN_PLAYER, true); }
		set { EditorPrefs.SetBool(PREFS_AUTO_RUN_PLAYER, value); }
	}
}