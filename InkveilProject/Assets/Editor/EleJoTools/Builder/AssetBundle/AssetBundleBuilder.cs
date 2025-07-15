using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 资源包构建器
/// </summary>
public static class AssetBundleBuilder
{
	private const string FORMAT_PATH = "{0}/{1}";
	private const string PATH_BUILD = "Build/AssetBundle";
	private const string NAME_OK = "OK";
	private const string GUI_PRE_PROCESS_TITLE = "Build Preprocess [{0}/{1}]";
	private const string GUI_PRE_PROCESS_INFO = "Preprocessing...{0}";
	private const string GUI_POST_PROCESS_TITLE = "Build Postprocess [{0}/{1}]";
	private const string GUI_POST_PROCESS_INFO = "Postprocessing...{0}";
	private const string NAME_BUILD_COMPLETE = "Build Complete";
	private const string NAME_BUILD_TIME = "Build cost time:{0}.";

	/// <summary>
	/// 构建资源包
	/// </summary>
	/// <param name="buildPath">构建目录</param>
	/// <param name="rebuild">重新构建</param>
	public static void Build(bool rebuild = false)
	{
		DateTime tick = DateTime.Now;

		Dictionary<string, AssetBundleConfig> configMap = new Dictionary<string, AssetBundleConfig>();
		string path = null;

		OnPreBuild(configMap, ref path, rebuild);

        BuildAssetBundleOptions options = BuildSettingData.instance.buildOptions;
        if (rebuild)
        {
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
        }

		BuildPipeline.BuildAssetBundles(path, options, EditorUserBuildSettings.activeBuildTarget);

		OnPostBuild(configMap, ref path, BuildSettingData.instance.buildPath);

		TimeSpan timeSpan = DateTime.Now - tick;
		EditorUtility.DisplayDialog(NAME_BUILD_COMPLETE, string.Format(NAME_BUILD_TIME, timeSpan), NAME_OK);
	}

	private static void OnPreBuild(Dictionary<string, AssetBundleConfig> configMap, ref string path, bool rebuild)
	{
		for (int i = 0; i < BuildSettingData.instance.assetBundleSettings.Count; ++i)
		{
			AssetBundleSetting setting = BuildSettingData.instance.assetBundleSettings[i];

			string title = string.Format(GUI_PRE_PROCESS_TITLE, i + 1, BuildSettingData.instance.assetBundleSettings.Count);
			string info = string.Format(GUI_PRE_PROCESS_INFO, setting.name);
			EditorUtility.DisplayProgressBar(title, info, (float)(i + 1) / BuildSettingData.instance.assetBundleSettings.Count);
			setting.processor.OnPreProcess(configMap);
		}
		EditorUtility.ClearProgressBar();

		path = string.Format(FORMAT_PATH, Directory.GetCurrentDirectory(), PATH_BUILD);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		else if (rebuild)
		{
			Directory.Delete(path, true);
			Directory.CreateDirectory(path);
		}
	}

	private static void OnPostBuild(Dictionary<string, AssetBundleConfig> configMap, ref string path, string buildPath)
	{
		buildPath = string.Format(FORMAT_PATH, buildPath, Config.PLATFORM);
		if (Directory.Exists(buildPath))
		{
			Directory.Delete(buildPath, true);
		}
		Directory.CreateDirectory(buildPath);

		string filePath = string.Format(FORMAT_PATH, path, path.Substring(path.LastIndexOf('/') + 1));
		File.Copy(filePath, string.Format(FORMAT_PATH, buildPath, BuildSettingData.instance.manifest));

		for (int i = 0; i < BuildSettingData.instance.assetBundleSettings.Count; ++i)
		{
			AssetBundleSetting setting = BuildSettingData.instance.assetBundleSettings[i];

			string title = string.Format(GUI_POST_PROCESS_TITLE, i + 1, BuildSettingData.instance.assetBundleSettings.Count);
			string info = string.Format(GUI_POST_PROCESS_INFO, setting.name);
			EditorUtility.DisplayProgressBar(title, info, (float)(i + 1) / BuildSettingData.instance.assetBundleSettings.Count);

			setting.processor.OnPostProcess(path, buildPath);
		}
		EditorUtility.ClearProgressBar();

		List<AssetBundleConfig> configs = new List<AssetBundleConfig>();
		foreach (AssetBundleConfig config in configMap.Values)
		{
			if (configs.Contains(config))
			{
				continue;
			}

			configs.Add(config);
		}

		AssetBundleConfig manifestConfig = new AssetBundleConfig();
		manifestConfig.name = BuildSettingData.instance.manifest;
		BuildPipeline.GetCRCForAssetBundle(filePath, out manifestConfig.crc);
		manifestConfig.size = new FileInfo(filePath).Length;
		configs.Insert(0, manifestConfig);

		string json = JsonMapper.ToJson(configs);
		File.WriteAllText(string.Format(FORMAT_PATH, buildPath, BuildSettingData.instance.config), json);
	}
}
