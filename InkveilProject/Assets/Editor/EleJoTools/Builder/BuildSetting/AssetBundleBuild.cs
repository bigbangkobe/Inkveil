using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 资源包设定
/// </summary>
[Serializable]
public sealed class AssetBundleSetting
{
	private const string GUI_NAME = "Name";
	private const string GUI_PATH = "Path";
	private const string GUI_SELECT = "Select";
	private const string GUI_SELECT_FOLDER = "Select Folder";
	private const string GUI_SEARCH_PATTERN = "Search Pattern";
	private const string GUI_SEARCH_OPTION = "Search Option";
	private const string GUI_BUILD_TYPE = "Build Type";
	private const string GUI_PROCESSOR_TYPE = "Processor Type";

	/// <summary>
	/// 构建名
	/// </summary>
	public string name = string.Empty;
	/// <summary>
	/// 处理路径
	/// </summary>
	public string path = string.Empty;
	/// <summary>
	/// 搜索参数
	/// </summary>
	public string searchPattern = string.Empty;
	/// <summary>
	/// 搜索选项
	/// </summary>
	public SearchOption searchOption = SearchOption.AllDirectories;
	/// <summary>
	/// 是否单个文件
	/// </summary>
	public BuildType buildType = BuildType.Packages;
	/// <summary>
	/// 处理器类型
	/// </summary>
	public string processorType = typeof(BaseProcessor).Name;
	/// <summary>
	/// 处理器对象
	/// </summary>
	public BaseProcessor processor
	{
		get
		{
			if (m_Processor == null || m_Processor.GetType().Name != processorType)
			{
				m_Processor = BaseProcessor.GetProcessor(this);
			}

			return m_Processor;
		}
	}
	private BaseProcessor m_Processor;

	/// <summary>
	/// 处理类型列表
	/// </summary>
	public static List<string> processorTypeList
	{
		get
		{
			if (s_ProcessorTypeList == null)
			{
				s_ProcessorTypeList = new List<string>();
				Type baseType = typeof(BaseProcessor);
				Type[] types = Assembly.GetExecutingAssembly().GetTypes();
				for (int i = 0; i < types.Length; ++i)
				{
					Type type = types[i];
					if (baseType.IsAssignableFrom(type))
					{
						s_ProcessorTypeList.Add(type.Name);
					}
				}
			}

			return s_ProcessorTypeList;
		}
	}
	private static List<string> s_ProcessorTypeList;

	public void OnGUI()
	{
		name = EditorGUILayout.TextField(GUI_NAME, name);
		GUILayout.BeginHorizontal();
		EditorGUILayout.TextField(GUI_PATH, path);
		if (GUILayout.Button(GUI_SELECT, GUILayout.Width(BuildSettings.BUTTON_SIZE)))
		{
			SetBuildPath();
		}
		GUILayout.EndHorizontal();

		searchPattern = EditorGUILayout.TextField(GUI_SEARCH_PATTERN, searchPattern);
		searchOption = (SearchOption)EditorGUILayout.EnumPopup(GUI_SEARCH_OPTION, searchOption);
		buildType = (BuildType)EditorGUILayout.EnumPopup(GUI_BUILD_TYPE, buildType);

		int index = processorTypeList.IndexOf(processorType);
		index = EditorGUILayout.Popup(GUI_PROCESSOR_TYPE, index, processorTypeList.ToArray());
		processorType = processorTypeList[index];

		processor.OnGUI();
	}

	/// <summary>
	/// 设置构建路径
	/// </summary>
	public void SetBuildPath()
	{
		string temp = EditorUtility.SaveFolderPanel(GUI_SELECT_FOLDER, path, null);
		if (!string.IsNullOrEmpty(temp))
		{
			path = temp.Substring(Directory.GetCurrentDirectory().Length + 1);
		}
	}

	/// <summary>
	/// 构建类型
	/// </summary>
	[Serializable]
	public enum BuildType
	{
		Packages,       //所有文件打包称一个
		Directorys,     //一个目录打包一个
		Files,          //一个文件打包一个
	}
}