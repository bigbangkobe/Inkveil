using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;
using OfficeOpenXml;

/// <summary>
/// 配置表处理器
/// </summary>
public sealed class ConfigProcessor : BaseProcessor
{
	private const string EXT_XLSX = "*.xlsx";
	private const string FORMAT_PATH = "{0}/{1}";
	private const string GUI_HELP_BOX = "HelpBox";
	private const string GUI_CONFIG_PATH = "Config Path";
	private const string GUI_SELECT_FOLDER = "Select Folder";
	private const string GUI_SELECT = "Select";
	private const string GUI_UPDATE = "Update";
	private const string GUI_TITLE = "Update Config [{0}/{1}]";
	private const string GUI_INFO = "Updating...{0}";
	private const string TYPE_INT = "int";
	private const string TYPE_FLOAT = "float";
    private const string TYPE_BOOL = "bool";
    private const string TYPE_NULL = "null";
	private const string TYPE_EMPTY = "\"\"";

	private static readonly Regex s_Regex = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");

	public override void OnGUI()
	{
		base.OnGUI();

		GUILayout.BeginVertical(GUI_HELP_BOX);

		GUILayout.BeginHorizontal();
		EditorGUILayout.TextField(GUI_CONFIG_PATH, EditorSettings.configPath);
		if (GUILayout.Button(GUI_SELECT, GUILayout.Width(BuildSettings.BUTTON_SIZE)))
		{
			SetConfigPath();
		}
		else if (GUILayout.Button(GUI_UPDATE, GUILayout.Width(BuildSettings.BUTTON_SIZE)))
		{
			UpdateConfig();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	/// <summary>
	/// 更新数据
	/// </summary>
	public void UpdateConfig()
	{
		if (string.IsNullOrEmpty(EditorSettings.configPath))
		{
			if (!SetConfigPath())
			{
				return;
			}
		}
        //Debug.Log("configPath:" + EditorSettings.configPath);
        //SVNTool.Clean(EditorSettings.configPath);
        //SVNTool.Revert(EditorSettings.configPath);
        //SVNTool.Update(EditorSettings.configPath);

        string ext = setting.searchPattern;
		DirectoryInfo directoryInfo = new DirectoryInfo(EditorSettings.configPath);
		FileInfo[] fileInfos = directoryInfo.GetFiles(EXT_XLSX, SearchOption.AllDirectories);
		for (int i = 0; i < fileInfos.Length; ++i)
		{
			FileInfo fileInfo = fileInfos[i];
			ExcelPackage excelPackage = new ExcelPackage(fileInfo);
			ExcelWorksheets excelSheets = excelPackage.Workbook.Worksheets;
			foreach (ExcelWorksheet excelSheet in excelSheets)
			{
				string title = string.Format(GUI_TITLE, i + 1, fileInfos.Length);
				string info = string.Format(GUI_INFO, excelSheet.Name);
				EditorUtility.DisplayProgressBar(title, info, (float)(i + 1) / fileInfos.Length);

				string path = fileInfo.FullName.Substring(directoryInfo.FullName.Length).Replace('\\', '/');
				path = setting.path + path;
				path = path.Remove(path.LastIndexOf('/'));
				path = string.Format(FORMAT_PATH, path, excelSheet.Name) + ext;

				FileInfo newFileInfo = new FileInfo(path);
				if (!newFileInfo.Directory.Exists)
				{
					newFileInfo.Directory.Create();
				}

				string content = ToJson(excelSheet);
				File.WriteAllText(path, content);
			}
		}
		EditorUtility.ClearProgressBar();

		AssetDatabase.Refresh();
	}

	/// <summary>
	/// 设置构建目录
	/// </summary>
	/// <returns>返回设置结果</returns>
	private bool SetConfigPath()
	{
		string path = EditorUtility.OpenFolderPanel(GUI_SELECT_FOLDER, EditorSettings.configPath, null);
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		EditorSettings.configPath = path;

		return true;
	}

	/// <summary>
	/// 转换Json
	/// </summary>
	/// <param name="excelSheet">Excel表</param>
	/// <returns>返回Json数据</returns>
	private string ToJson(ExcelWorksheet excelSheet)
	{
		JsonData jsonArray = new JsonData();
		//Debug.Log("Name:" + excelSheet.Name + ",Dimension:" + excelSheet.Dimension);
		for (int i = 5; i <= excelSheet.Dimension.Rows; ++i)
		{
			JsonData jsonData = new JsonData();
			bool isEmpty = true;

			for (int j = 1; j <= excelSheet.Dimension.Columns; ++j)
			{
				string key = excelSheet.GetValue<string>(1, j);
				if (string.IsNullOrEmpty(key))
				{
					break;
				}

				string type = excelSheet.GetValue<string>(2, j);
				string value = excelSheet.GetValue<string>(i, j);

				if (j == 1 && string.IsNullOrEmpty(value))
				{
					break;
				}

				if (type == TYPE_INT)
				{
					long intValue = default(long);
					long.TryParse(value, out intValue);
					jsonData[key] = intValue;
				}
				else if (type == TYPE_FLOAT)
				{
					double floatValue = default(double);
					double.TryParse(value, out floatValue);
					jsonData[key] = floatValue;
				}
                else if (type == TYPE_BOOL)
                {
                    bool boolValue = default(bool);
                    bool.TryParse(value, out boolValue);
                    jsonData[key] = boolValue;
                }
                else
				{
					jsonData[key] = value;
				}

				isEmpty = false;
			}

			if (!isEmpty)
			{
				jsonArray.Add(jsonData);
			}
		}

		string json = jsonArray.ToJson();
		json = s_Regex.Replace(json, (match) => { return ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString(); });
		json = json.Replace(TYPE_NULL, TYPE_EMPTY);

		return json;
	}
}