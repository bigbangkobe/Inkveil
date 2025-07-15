using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 安装包构建器
/// </summary>
public static class PlayerBuilder
{
	private const string CMD_BUILD_OPTION = "buildOption";
	private const string CMD_ADD_VERSION = "addVersion";
	private const string CMD_UPDATE_SVN = "updateSVN";
	private const string CMD_COMMIT_SVN = "commitSVN";
	private const string CMD_BUILD_AB = "buildAB";
	private const string CMD_REBUILD_AB = "rebuildAB";
	private const string CMD_BUILD_DEBUG = "buildDebug";
	private const string CMD_BUILD_RELEASE = "buildRelease";
	private const string CMD_LOCAL_AB = "localAB";
	private const string FORMAT_VERSION = "{0}.{1}.{2}";
	private const string FORMAT_PATH = "{0}/{1}";
	private const string EXT_ARG = "-";
	private const string EXT_META = "*.meta";
#if UNITY_IPHONE
	private const string PATH_BUILD = "Build/Xcode";
	private const string CMD_XCBUILD = "xcodebuild";
	private const string CMD_XCBUILD_ARCHIVE = "-scheme Unity-iPhone -archivePath build/Unity-iPhone.xcarchive archive";
	private const string CMD_XCBUILD_EXORT = "xcodebuild -exportArchive -exportOptionsPlist info.plist -archivePath build/Unity-iPhone.xcarchive -exportPath build/";
	private const string PATH_PACKAGE = "Build/Xcode/build/Unity-iPhone.ipa";
	private const string NAME_PACKAGE = "{0}_{1}_{2}_{3}.ipa";
#elif UNITY_ANDROID
	private const string PATH_BUILD = "Build/Packages/";
	private const string NAME_PACKAGE = "{0}_{1}_{2}_{3}.apk";
#else
	private const string PATH_BUILD = "Build/Packages/Unity-PC.exe";
	private const string NAME_PACKAGE = "{0}_{1}_{2}_{3}.exe";
#endif

	/// <summary>
	/// 构建选项
	/// </summary>
	private static BuildOptions s_BuildOptions;

	/// <summary>
	/// 命令构建
	/// </summary>
	public static void CommandBuild()
	{
		//设定命令
		Dictionary<string, string> argMap = GetCommandArgs();
		string value = null;

		bool addVersion = false;
		bool commitSVN = false;
		bool buildAB = true;
		bool rebuildAB = false;
		bool buildDebug = true;
		bool buildRelease = false;
		if (argMap.TryGetValue(CMD_BUILD_OPTION, out value))
		{
			//更新SVN
			if (value.Contains(CMD_UPDATE_SVN))
			{
				SVNTool.Clean();
				SVNTool.Revert();
				SVNTool.Update();
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}

			addVersion = value.Contains(CMD_ADD_VERSION);
			commitSVN = value.Contains(CMD_COMMIT_SVN);
			buildAB = value.Contains(CMD_BUILD_AB);
			rebuildAB = value.Contains(CMD_REBUILD_AB);
			buildDebug = value.Contains(CMD_BUILD_DEBUG);
			buildRelease = value.Contains(CMD_BUILD_RELEASE);
			BuildSettings.isLocalAB = value.Contains(CMD_LOCAL_AB);
		}

		//构建AssetBundle
		if (rebuildAB)
		{
			AssetBundleBuilder.Build(rebuildAB);
		}
		else if (buildAB)
		{
			AssetBundleBuilder.Build();
		}

		//处理版本号
		if (addVersion)
		{
#if UNITY_IPHONE
			string[] splits = BuildSettingData.instance.iOSVersion.Split('.');
			int version = int.Parse(splits[splits.Length - 1]) + 1;
			BuildSettingData.instance.iOSVersion = string.Format(FORMAT_VERSION, splits[0], splits[1], version);
#else
			string[] splits = BuildSettingData.instance.androidVersion.Split('.');
			int version = int.Parse(splits[splits.Length - 1]) + 1;
			BuildSettingData.instance.androidVersion = string.Format(FORMAT_VERSION, splits[0], splits[1], version);
#endif
		}

		//构建Debug安装包
		if (buildDebug)
		{
			BuildPlayer(BuildOptions.Development, true);

#if UNITY_IPHONE
			string buildCode = PlayerSettings.iOS.buildNumber;
			string xcodePath = string.Format(FORMAT_PATH, Directory.GetCurrentDirectory(), PATH_BUILD);
			string path = PATH_PACKAGE;
			CMDTool.Excute(CMD_XCBUILD, CMD_XCBUILD_ARCHIVE, xcodePath);
			CMDTool.Excute(CMD_XCBUILD, CMD_XCBUILD_EXORT, xcodePath);
#else
            string buildCode = PlayerSettings.Android.bundleVersionCode.ToString();
            string path = PATH_BUILD;
#endif
            string name = string.Format(NAME_PACKAGE, Application.identifier, PlayerSettings.bundleVersion, buildCode, "debug");
			string svn = string.Format(FORMAT_PATH, Config.PLATFORM, name);
			svn = string.Format(FORMAT_PATH, BuildSettingData.instance.packageSVN, svn).ToLower();
			SVNTool.Import(path, svn, "Auto Build Debug Package.");
		}

		//构建Release安装包
		if (buildRelease)
		{
			BuildPlayer(BuildOptions.None, false);

#if UNITY_IPHONE
			string buildCode = PlayerSettings.iOS.buildNumber;
			string xcodePath = string.Format(FORMAT_PATH, Directory.GetCurrentDirectory(), PATH_BUILD);
			string path = PATH_PACKAGE;
			CMDTool.Excute(CMD_XCBUILD, CMD_XCBUILD_ARCHIVE, xcodePath);
			CMDTool.Excute(CMD_XCBUILD, CMD_XCBUILD_EXORT, xcodePath);
			buildCode = PlayerSettings.iOS.buildNumber;
#else
			string buildCode = PlayerSettings.Android.bundleVersionCode.ToString();
			string path = PATH_BUILD;
#endif
			string name = string.Format(NAME_PACKAGE, Application.identifier, PlayerSettings.bundleVersion, buildCode, "release");
			string svn = string.Format(FORMAT_PATH, Config.PLATFORM, name);
			svn = string.Format(FORMAT_PATH, BuildSettingData.instance.packageSVN, svn).ToLower();
			SVNTool.Import(path, svn, "Auto Build Release Package.");
		}

		//提交SVN
		if (commitSVN)
		{
			BuildSettingData.instance.Save();
			SVNTool.Commit(BuildSettingData.PATH, "Update Build Setting Data.");
			SVNTool.Commit(BuildSettingData.instance.buildPath, "Update Resources.");
		}
	}

	/// <summary>
	/// 获取命令参数表
	/// </summary>
	/// <returns>返回参数表</returns>
	private static Dictionary<string, string> GetCommandArgs()
	{
		string[] args = Environment.GetCommandLineArgs();
		Dictionary<string, string> argMap = new Dictionary<string, string>();
		for (int i = 0; i < args.Length - 1; ++i)
		{
			string name = args[i];
			string arg = args[i + 1];
			if (!name.Contains(EXT_ARG))
			{
				continue;
			}

			argMap[name.Substring(1)] = arg;
			++i;
		}

		return argMap;
	}

	/// <summary>
	/// 构建安装包
	/// </summary>
	/// <param name="buildOptions">构建选项</param>
	public static void BuildPlayer(BuildOptions buildOptions,bool isDebug)
	{
		s_BuildOptions = buildOptions;
		string assetPath = string.Format(FORMAT_PATH, Application.streamingAssetsPath, BuildSettingData.instance.buildPath);
		assetPath = string.Format(FORMAT_PATH, assetPath, Config.PLATFORM);

		OnPreBuild(assetPath);
        string pathBuild = PATH_BUILD + BuildSettingData.instance.packageName;
        if (isDebug)
        {
            pathBuild += "_Debug";
        }
#if UNITY_IPHONE
	pathBuild += ".ipa";
#elif UNITY_ANDROID
    pathBuild += ".apk";
#else
	pathBuild += ".exe";
#endif
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
            pathBuild,
			EditorUserBuildSettings.activeBuildTarget,
			buildOptions);

		OnPostBuild(assetPath);
	}
	/// <summary>
	/// 构建安装包
	/// </summary>
	/// <param name="buildOptions">构建选项</param>
	public static void BuildPlayer(Dictionary<string, string> ElejoBuildData)
	{

		BuildOptions buildOptions = BuildOptions.None;
		if (EditorSettings.autoRunPlayer)
		{
			buildOptions |= BuildOptions.AutoRunPlayer;
		}

		OnPreBuild(ElejoBuildData);

		string pathBuild = string.Format(@"{0}\{1}-{2}", ElejoBuildData[JenkinsBatData.ApkPath], ElejoBuildData[JenkinsBatData.ProductName], ElejoBuildData[JenkinsBatData.Version]);

		if (ElejoBuildData[JenkinsBatData.ApkType] == "Debug")
		{
			pathBuild += "_Debug";
		}
		else
		{
			buildOptions = BuildOptions.Development;
		}

		s_BuildOptions = buildOptions;
		///直接jenkins传输过来包类型
		pathBuild += ".apk";

		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
						pathBuild,
						EditorUserBuildSettings.activeBuildTarget,
						buildOptions);

		OnPostBuild(ElejoBuildData[JenkinsBatData.AssetPath]);
	}
	/// <summary>
	/// jenkins传递参数构建预处理
	/// </summary>
	/// <param name="assetPath">资源路径</param>
	private static void OnPreBuild(Dictionary<string, string> ElejoBuildData)
	{
		//EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
		//|| 
		//if (BuildSettings.isLocalAB
		//        || (s_BuildOptions & BuildOptions.Development) == BuildOptions.Development)
		//{
		//        DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
		//        if (directoryInfo.Exists)
		//        {
		//                directoryInfo.Delete(true);
		//        }
		//        else if (!directoryInfo.Parent.Exists)
		//        {
		//                directoryInfo.Parent.Create();
		//        }
		//        Directory.Move(Config.PATH_ASSET_BUNDLE, assetPath);
		//}

		//处理版本号
		int buildCode = (DateTime.Now.Year % 100) * 10000000 + DateTime.Now.DayOfYear * 10000 + DateTime.Now.Hour * 100 + DateTime.Now.Minute;
		PlayerSettings.productName = string.Format("{0}-{1}",ElejoBuildData[JenkinsBatData.ProductName],ElejoBuildData[JenkinsBatData.Version]);

		PlayerSettings.bundleVersion = BuildSettingData.instance.androidVersion;
		PlayerSettings.Android.bundleVersionCode = buildCode;

		if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
		{
			PlayerSettings.Android.keystoreName = Directory.GetCurrentDirectory().Replace('\\', '/') + "/" + Config.KEYSTORE_NAME;
		}
		else
		{
			PlayerSettings.Android.keystoreName = Config.KEYSTORE_NAME;
		}
		PlayerSettings.Android.keystorePass = Config.KEYSTORE_PASS;
		PlayerSettings.Android.keyaliasName = Config.KEYSTORE_PROJECTNAME;
		PlayerSettings.Android.keyaliasPass = Config.KEYSTORE_PASS;
		FileInfo fileInfo = new FileInfo(PATH_BUILD);
		if (!fileInfo.Directory.Exists)
		{
			fileInfo.Directory.Create();
		}

	}


	/// <summary>
	/// 构建预处理
	/// </summary>
	/// <param name="assetPath">资源路径</param>
	private static void OnPreBuild(string assetPath)
	{
        //EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
			//|| 
		//if (BuildSettings.isLocalAB
		//	|| (s_BuildOptions & BuildOptions.Development) == BuildOptions.Development)
		//{
		//	DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
		//	if (directoryInfo.Exists)
		//	{
		//		directoryInfo.Delete(true);
		//	}
		//	else if (!directoryInfo.Parent.Exists)
		//	{
		//		directoryInfo.Parent.Create();
		//	}
		//	Directory.Move(Config.PATH_ASSET_BUNDLE, assetPath);
		//}

		//处理版本号
        int buildCode = (DateTime.Now.Year % 100) * 10000000 + DateTime.Now.DayOfYear * 10000 + DateTime.Now.Hour * 100 + DateTime.Now.Minute;
        PlayerSettings.productName = BuildSettingData.instance.productName;
#if UNITY_IPHONE
		PlayerSettings.bundleVersion = BuildSettingData.instance.iOSVersion;
		PlayerSettings.iOS.buildNumber = buildCode.ToString();

		if (!Directory.Exists(PATH_BUILD))
		{
			Directory.CreateDirectory(PATH_BUILD);
		}
#elif UNITY_ANDROID
        PlayerSettings.bundleVersion = BuildSettingData.instance.androidVersion;
		PlayerSettings.Android.bundleVersionCode = buildCode;

		if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
		{
			PlayerSettings.Android.keystoreName = Directory.GetCurrentDirectory().Replace('\\', '/') + "/" + Config.KEYSTORE_NAME;
		}
		else
		{
			PlayerSettings.Android.keystoreName = Config.KEYSTORE_NAME;
		}
		PlayerSettings.Android.keystorePass = Config.KEYSTORE_PASS;
		PlayerSettings.Android.keyaliasName = Config.KEYSTORE_PROJECTNAME;
		PlayerSettings.Android.keyaliasPass = Config.KEYSTORE_PASS;
        FileInfo fileInfo = new FileInfo(PATH_BUILD);
		if (!fileInfo.Directory.Exists)
		{
			fileInfo.Directory.Create();
		}
#endif
	}

	/// <summary>
	/// 构建后处理
	/// </summary>
	/// <param name="assetPath">资源路径</param>
	private static void OnPostBuild(string assetPath)
	{
        //EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
			//|| 
		//if (BuildSettings.isLocalAB
		//	|| (s_BuildOptions & BuildOptions.Development) == BuildOptions.Development)
		//{
		//	Directory.Move(assetPath, Config.PATH_ASSET_BUNDLE);
		//	DirectoryInfo directoryInfo = new DirectoryInfo(Config.PATH_ASSET_BUNDLE);
		//	FileInfo[] fileInfos = directoryInfo.GetFiles(EXT_META, SearchOption.AllDirectories);
		//	for (int i = 0; i < fileInfos.Length; ++i)
		//	{
		//		FileInfo fileInfo = fileInfos[i];
		//		File.Delete(fileInfo.FullName);
		//	}
		//}
	}

	[PostProcessBuild(0)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		//iOS处理
#if UNITY_IPHONE
		string projPath = PBXProject.GetPBXProjectPath(path);
		PBXProject pbxProject = new PBXProject();
		pbxProject.ReadFromString(File.ReadAllText(projPath));
		string targetGuid = pbxProject.TargetGuidByName("Unity-iPhone");

		//添加framework
		pbxProject.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
		pbxProject.AddFrameworkToProject(targetGuid, "CoreAudio.framework", false);
        pbxProject.AddFrameworkToProject(targetGuid, "Security.framework", false);
        pbxProject.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);

		//添加dylib
		pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
		pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
		pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
		pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile("usr/lib/libstdc++.6.0.9.tbd", "Frameworks/libstdc++.6.0.9.tbd", PBXSourceTree.Sdk));

		//添加property
		pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
		pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

		//应用修改
		File.WriteAllText(projPath, pbxProject.WriteToString());

		//处理Plist
		string plistPath = path + "/Info.plist";
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);
		PlistElementDict rootDict = plist.root;

		PlistElementArray array = rootDict.CreateArray("LSApplicationQueriesSchemes");
		array.AddString("weixin");

		array = rootDict.CreateArray("CFBundleURLTypes");

        PlistElementDict dict = array.AddDict();
        dict.SetString("CFBundleTypeRole", "Editor");
        dict.SetString("CFBundleURLName", "");
        PlistElementArray elementArray = dict.CreateArray("CFBundleURLSchemes");
        elementArray.AddString("wxb1ab806104882755");

        dict = array.AddDict();
        dict.SetString("CFBundleTypeRole", "Editor");
        dict.SetString("CFBundleURLName", "");
        elementArray = dict.CreateArray("CFBundleURLSchemes");
        elementArray.AddString("jubafang");

		rootDict.SetString("NSLocationWhenInUseUsageDescription", "需要获取您的位置用于麻将游戏“好友位置”模块记录您的位置信息，若不允许，将无法显示您与您朋友的位置情况。");
		rootDict.SetString("NSMicrophoneUsageDescription", "可以用下你的麦克风吗？");

		if ((s_BuildOptions & BuildOptions.Development) == BuildOptions.Development)
		{
			rootDict.SetBoolean("UIFileSharingEnabled", true);
		}
		plist.WriteToFile(plistPath);
#endif
	}
}