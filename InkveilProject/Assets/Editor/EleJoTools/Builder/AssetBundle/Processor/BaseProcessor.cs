using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 基础处理器
/// </summary>
public class BaseProcessor
{
    private const string NAME_ASSETS = "Assets";
	private const string FORMAT_FILE = "{0}.{1}";
	private const string FORMAT_PATH = "{0}/{1}";
    private const string EXT_MANIFEST = ".manifest";
    private const string EXT_SHADER = ".shader";
	private const string EXT_ALL = "*";

	/// <summary>
	/// 构建配置
	/// </summary>
	public AssetBundleSetting setting { get; private set; }

	private List<AssetBundleConfig> m_ConfigList;

	/// <summary>
	/// 获取处理器
	/// </summary>
	/// <param name="setting">构建配置</param>
	/// <returns>返回处理器</returns>
	public static BaseProcessor GetProcessor(AssetBundleSetting setting)
	{
		Type type = Type.GetType(setting.processorType);
		BaseProcessor processor = Activator.CreateInstance(type) as BaseProcessor;
		processor.setting = setting;

		return processor;
	}

	/// <summary>
	/// 绘制回调
	/// </summary>
	public virtual void OnGUI()
	{
	}

	/// <summary>
	/// 构建前处理
	/// </summary>
	/// <param name="configMap">配置表</param>
	public virtual void OnPreProcess(Dictionary<string, AssetBundleConfig> configMap)
	{
		m_ConfigList = new List<AssetBundleConfig>();
		
		if (setting.buildType == AssetBundleSetting.BuildType.Packages)
		{
			BuildPackages(configMap);
		}
		else if (setting.buildType == AssetBundleSetting.BuildType.Directorys)
		{
			BuilDirectorys(configMap);
		}
		else if (setting.buildType == AssetBundleSetting.BuildType.Files)
		{
			BuildFiles(configMap);
		}
	}

	/// <summary>
	/// 构建后处理
	/// </summary>
	/// <param name="path">文件路径</param>
	/// <param name="buildPath">构建路径</param>
	public virtual void OnPostProcess(string path, string buildPath)
	{
		for (int i = 0; i < m_ConfigList.Count; ++i)
		{
			AssetBundleConfig config = m_ConfigList[i];

			string filePath = string.Format(FORMAT_PATH, path, config.name);
			FileInfo fileInfo = new FileInfo(string.Format(FORMAT_PATH, buildPath, config.name));
			if (!fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}

			File.Copy(filePath, fileInfo.FullName, true);

			BuildPipeline.GetCRCForAssetBundle(filePath, out config.crc);
			config.size = fileInfo.Length;
		}
	}

	private void BuildPackages(Dictionary<string, AssetBundleConfig> configMap)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(setting.path);
		FileInfo[] fileInfos = directoryInfo.GetFiles(setting.searchPattern, setting.searchOption);
		AssetBundleConfig config = null;
		string name = setting.name.ToLower();
		for (int i = 0; i < fileInfos.Length; ++i)
		{
            FileInfo fileInfo = fileInfos[i];
			string filePath = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(NAME_ASSETS)).Replace('\\', '/');
			AssetImporter assetImporter = AssetImporter.GetAtPath(filePath);
			if (assetImporter == null)
			{
				continue;
			}

			string fileName = string.Format(FORMAT_FILE, name, BuildSettingData.instance.variant);
			if (i == 0)
			{
				config = new AssetBundleConfig();
				config.name = fileName;
				configMap[filePath] = config;
				m_ConfigList.Add(config);
			}
			else
            {
				configMap[filePath] = config;
            }

            assetImporter.SetAssetBundleNameAndVariant(name, BuildSettingData.instance.variant);
		}
	}

	private void BuilDirectorys(Dictionary<string, AssetBundleConfig> configMap)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(setting.path);
		DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories(EXT_ALL, SearchOption.AllDirectories);
		AssetBundleConfig config = null;
		for (int i = 0; i < directoryInfos.Length; ++i)
		{
			DirectoryInfo childDirectoryInfo = directoryInfos[i];
			FileInfo[] fileInfos = childDirectoryInfo.GetFiles(setting.searchPattern, SearchOption.AllDirectories);
			for (int j = 0; j < fileInfos.Length; ++j)
			{
				FileInfo fileInfo = fileInfos[j];
				string filePath = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(NAME_ASSETS)).Replace('\\', '/');
				AssetImporter assetImporter = AssetImporter.GetAtPath(filePath);
				if (assetImporter == null)
				{
					continue;
				}

				string name = childDirectoryInfo.FullName.Substring(childDirectoryInfo.FullName.LastIndexOf(NAME_ASSETS));
				name = name.Substring(NAME_ASSETS.Length + 1).Replace('\\', '/').ToLower();
				string fileName = string.Format(FORMAT_FILE, name, BuildSettingData.instance.variant);
				if (j == 0)
				{
					config = new AssetBundleConfig();
					config.name = fileName;
					configMap[filePath] = config;
					m_ConfigList.Add(config);
				}
				else
                {
					configMap[filePath] = config;
				}

				assetImporter.SetAssetBundleNameAndVariant(name, BuildSettingData.instance.variant);
			}
		}
	}

	private void BuildFiles(Dictionary<string, AssetBundleConfig> configMap)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(setting.path);
		FileInfo[] fileInfos = directoryInfo.GetFiles(setting.searchPattern, setting.searchOption);
		AssetBundleConfig config = null;
		for (int i = 0; i < fileInfos.Length; ++i)
		{
			FileInfo fileInfo = fileInfos[i];
			string filePath = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(NAME_ASSETS)).Replace('\\', '/');
			string[] dependencies = AssetDatabase.GetDependencies(filePath);
			for (int j = 0; j < dependencies.Length; ++j)
			{
				string dependence = dependencies[j];
                int lastIndexOfNameAssets = dependence.LastIndexOf(NAME_ASSETS);
                if (lastIndexOfNameAssets < 0)
                {
					continue;
                }
                dependence = dependence.Substring(lastIndexOfNameAssets);
                AssetImporter assetImporter = AssetImporter.GetAtPath(dependence);
				if (assetImporter == null
					|| assetImporter is MonoImporter
					|| assetImporter is PluginImporter)
				{
					continue;
                }

                if (dependence.Contains(EXT_SHADER))
                {
                    assetImporter.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
                    continue;
                }

				string name = dependence.Substring(NAME_ASSETS.Length + 1);
				name = name.Remove(name.LastIndexOf('.')).ToLower();
				string fileName = string.Format(FORMAT_FILE, name, BuildSettingData.instance.variant);
				if (configMap.ContainsKey(dependence))
				{
					continue;
				}

				config = new AssetBundleConfig();
				config.name = fileName;
				configMap[dependence] = config;
				m_ConfigList.Add(config);

				assetImporter.SetAssetBundleNameAndVariant(name, BuildSettingData.instance.variant);
			}
		}
	}
}