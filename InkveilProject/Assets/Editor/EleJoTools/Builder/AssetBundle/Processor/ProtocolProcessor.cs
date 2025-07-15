using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 协议处理器
/// </summary>
public sealed class ProtocolProcessor : BaseProcessor
{
	private const string EXT_PROTOCOL = "jar";
	private const string CMD_JAVA = "java";
	private const string CMD_UPDATE_PROTOCOL = "-jar proto_tool.jar ./proto {0} \"\" json";
	private const string GUI_HELP_BOX = "HelpBox";
	private const string GUI_PROTOCOL_PATH = "Protocol Path";
	private const string GUI_SELECT_FOLDER = "Select Folder";
	private const string GUI_SELECT = "Select";
	private const string GUI_UPDATE = "Update";

	public override void OnGUI()
	{
		base.OnGUI();

		GUILayout.BeginVertical(GUI_HELP_BOX);

		GUILayout.BeginHorizontal();
		EditorGUILayout.TextField(GUI_PROTOCOL_PATH, EditorSettings.protocolPath);
		if (GUILayout.Button(GUI_SELECT, GUILayout.Width(BuildSettings.BUTTON_SIZE)))
		{
			SetProtocolPath();
		}
		else if (GUILayout.Button(GUI_UPDATE, GUILayout.Width(BuildSettings.BUTTON_SIZE)))
		{
			UpdateProtocol();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	/// <summary>
	/// 更新协议
	/// </summary>
	public void UpdateProtocol()
	{
		if (string.IsNullOrEmpty(EditorSettings.protocolPath))
		{
			if (!SetProtocolPath())
			{
				return;
			}
		}

		SVNTool.Clean(EditorSettings.protocolPath);
		SVNTool.Revert(EditorSettings.protocolPath);
		SVNTool.Update(EditorSettings.protocolPath);

		DirectoryInfo directoryInfo = new DirectoryInfo(setting.path);
		string cmd = string.Format(CMD_UPDATE_PROTOCOL, directoryInfo.FullName);
		CMDTool.Excute(CMD_JAVA, cmd, EditorSettings.protocolPath);
	}

	/// <summary>
	/// 设置构建目录
	/// </summary>
	/// <returns>返回设置结果</returns>
	private bool SetProtocolPath()
	{
		string path = EditorUtility.OpenFolderPanel(GUI_SELECT_FOLDER, EditorSettings.protocolPath, null);
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		EditorSettings.protocolPath = path;

		return true;
	}
}