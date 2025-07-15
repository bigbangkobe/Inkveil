using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>
/// 构建配置
/// </summary>
[CustomEditor(typeof(Main))]
public sealed class BuildSettingData : ScriptableObject
{
	/// <summary>
	/// 储存路径
	/// </summary>
	public const string PATH = "Assets/Editor Default Resources/BuildSettingData.asset";
	/// <summary>
	/// 实例对象
	/// </summary>
	public static BuildSettingData instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = AssetDatabase.LoadAssetAtPath<BuildSettingData>(PATH);
				if (s_Instance == null)
				{
					s_Instance = new BuildSettingData();
					FileInfo fileInfo = new FileInfo(PATH);
					if (!fileInfo.Directory.Exists)
					{
						fileInfo.Directory.Create();
					}
					AssetDatabase.CreateAsset(s_Instance, PATH);
				}
			}

			return s_Instance;
		}
	}
	private static BuildSettingData s_Instance;

	/// <summary>
	/// 安装包SVN
	/// </summary>
	public string packageSVN = "";
    /// <summary>
    /// 项目名称
    /// </summary>
    public string packageName = "ELEJO";
    /// <summary>
    /// 项目名称
    /// </summary>
    public string productName = "ELEJO";
	/// <summary>
	/// Android版本号
	/// </summary>
	public string androidVersion = "1.0.0";
	/// <summary>
	/// iOS版本号
	/// </summary>
	public string iOSVersion = "1.0.0";
	/// <summary>
	/// 构建路径
	/// </summary>
	public string buildPath = "AssetBundle";
	/// <summary>
	/// 变体名
	/// </summary>
	public string variant = "bundle";
	/// <summary>
	/// 资源配置文件
	/// </summary>
	public string manifest = "manifest";
    /// <summary>
    /// 测试数据名称
    /// </summary>
    public string testDataName = "Txt_20230208";
	/// <summary>
	/// 资源信息文件
	/// </summary>
	public string config = "config.json";
	/// <summary>
	/// 构建选项
	/// </summary>
	public BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;
	/// <summary>
	/// 资源包构建链表
	/// </summary>
	public List<AssetBundleSetting> assetBundleSettings = new List<AssetBundleSetting>();

	/// <summary>
	/// 保存数据
	/// </summary>
	public void Save()
	{
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}
}