using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 构建设置
/// </summary>
public sealed class BuildSettings : EditorWindow
{
    public const int BUTTON_SIZE = 60;

    private const string COMPLIE_LOCAL_AB = ";LOCAL_AB";
    private const string GUI_HELP_BOX = "HelpBox";
    private const string GUI_LOCAL_AB = "Local AB";
    private const string GUI_AUTO_RUN_PLAYER = "Auto Run Player";
    private const string GUI_USE_TEST_DATA = "Use Test Data";
    private const string COMPLIE_USE_TEST_DATA = ";USE_TEST_DATA";
    private const string GUI_DEBUG_LOG = "Debug Log";
    private const string COMPLIE_DEBUG_LOG = ";DEBUG_LOG";
    private const string GUI_DEBUG_TEXT = "Debug Text";
    private const string COMPLIE_DEBUG_TEXT = ";DEBUG_TEXT";
    private const string GUI_PACKAGE_SVN = "Package SVN";
    private const string GUI_PACKAGE_NAME = "Package Name";
    private const string GUI_PRODUCT_NAME = "Product Name";
    private const string GUI_ANDROID_VERSION = "Android Version";
    private const string GUI_IOS_VERSION = "iOS Version";
    private const string GUI_BUILD_PATH = "Build Path";
    private const string GUI_BUILDVARIANT = "Build Variant";
    private const string GUI_BUILD_MANIFEST = "Build Manifest";
    private const string GUI_BUILD_CONFIG = "Build Config";
    private const string GUI_BUILD_OPTIONS = "Build Options";
    private const string GUI_SELECT = "Select";
    private const string GUI_SELECT_FOLDER = "Select Folder";
    private const string GUI_BUILD_ASSETBUNDLE = "Build AssetBundle";
    private const string GUI_REBUILD_ASSETBUNDLE = "Rebuild AssetBundle";
    private const string GUI_BUILD_DEBUG = "Debug";
    private const string GUI_BUILD_RELEASE = "Release";
    private const string GUI_ADD_SETTINGS = "Add Settings";
    private const string GUI_SAVE_SETTINGS = "Save Settings";
    private const string GUI_DELETE = "Delete";
    private const string GUI_MOVE_UP = "Move Up";
    private const string GUI_MOVE_DOWN = "Move Down";
    private const string GUI_CLEAR_TITLE = "Clear Asset Bundle [{0}/{1}]";
    private const string GUI_CLEAR_INFO = "Clearing...{0}";

    private Vector2 m_ScrollPostion;

#if UNITY_ANDROID
    private static BuildTargetGroup s_TargetGroup = BuildTargetGroup.Android;
#elif UNITY_IPHONE
    private static BuildTargetGroup s_TargetGroup = BuildTargetGroup.iOS;
#else
    private static BuildTargetGroup s_TargetGroup = BuildTargetGroup.Standalone;
#endif

    public static bool isLocalAB
    {
        get => PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup).Contains(COMPLIE_LOCAL_AB);
        set
        {
            string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup);
            if (value != define.Contains(COMPLIE_LOCAL_AB))
            {
                define = define.Replace(COMPLIE_LOCAL_AB, string.Empty);
                if (value) define += COMPLIE_LOCAL_AB;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(s_TargetGroup, define);
            }
        }
    }

    public static bool isUseTestData
    {
        get => PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup).Contains(COMPLIE_USE_TEST_DATA);
        set
        {
            string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup);
            if (value != define.Contains(COMPLIE_USE_TEST_DATA))
            {
                define = define.Replace(COMPLIE_USE_TEST_DATA, string.Empty);
                if (value) define += COMPLIE_USE_TEST_DATA;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(s_TargetGroup, define);
            }
        }
    }

    public static bool isDebugLog
    {
        get => PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup).Contains(COMPLIE_DEBUG_LOG);
        set
        {
            string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup);
            if (value != define.Contains(COMPLIE_DEBUG_LOG))
            {
                define = define.Replace(COMPLIE_DEBUG_LOG, string.Empty);
                if (value) define += COMPLIE_DEBUG_LOG;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(s_TargetGroup, define);
            }
        }
    }

    public static bool isDebugText
    {
        get => PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup).Contains(COMPLIE_DEBUG_TEXT);
        set
        {
            string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(s_TargetGroup);
            if (value != define.Contains(COMPLIE_DEBUG_TEXT))
            {
                define = define.Replace(COMPLIE_DEBUG_TEXT, string.Empty);
                if (value) define += COMPLIE_DEBUG_TEXT;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(s_TargetGroup, define);
            }
        }
    }

    [MenuItem("ELEJO/Build/Build Settings", false, 0)]
    public static void Open() => GetWindow<BuildSettings>();

    [MenuItem("ELEJO/Build/Build Debug", false, 100)]
    public static void BuildDebug()
    {
        BuildOptions buildOptions = BuildOptions.Development;
        if (EditorSettings.autoRunPlayer) buildOptions |= BuildOptions.AutoRunPlayer;
        PlayerBuilder.BuildPlayer(buildOptions, true);
    }

    [MenuItem("ELEJO/Build/Build Release", false, 101)]
    public static void BuildRelease()
    {
        BuildOptions buildOptions = BuildOptions.None;
        if (EditorSettings.autoRunPlayer) buildOptions |= BuildOptions.AutoRunPlayer;
        PlayerBuilder.BuildPlayer(buildOptions, false);
    }

#if LOCAL_AB
    [MenuItem("ELEJO/Build/Disabled LocalAB", false, 102)]
#else
    [MenuItem("ELEJO/Build/Enabled LocalAB", false, 102)]
#endif
    public static void SwitchLocalAB() => isLocalAB = !isLocalAB;

    [MenuItem("ELEJO/Build/Build Asset Bundle", false, 200)]
    public static void BuildAssetBundle()
    {
        if (string.IsNullOrEmpty(BuildSettingData.instance.buildPath))
        {
            if (!SetBuildPath()) return;
        }
        AssetBundleBuilder.Build(false);
    }

    [MenuItem("ELEJO/Build/Rebuild Asset Bundle", false, 201)]
    public static void RebuildAssetBundle()
    {
        if (string.IsNullOrEmpty(BuildSettingData.instance.buildPath))
        {
            if (!SetBuildPath()) return;
        }
        AssetBundleBuilder.Build(true);
    }

    [MenuItem("ELEJO/Build/Clear Asset Bundle Names", false, 202)]
    public static void ClearAssetBundleNames()
    {
        FileInfo[] fileInfos = new DirectoryInfo(Application.dataPath).GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileInfo fileInfo = fileInfos[i];
            string path = fileInfo.FullName.Substring(Directory.GetCurrentDirectory().Length + 1);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null || string.IsNullOrEmpty(assetImporter.assetBundleName)) continue;

            string title = string.Format(GUI_CLEAR_TITLE, i + 1, fileInfos.Length);
            string info = string.Format(GUI_CLEAR_INFO, path);
            if (EditorUtility.DisplayCancelableProgressBar(title, info, (float)(i + 1) / fileInfos.Length)) break;

            assetImporter.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("ELEJO/Build/Update Config", false, 203)]
    public static void UpdateConfig()
    {
        foreach (var setting in BuildSettingData.instance.assetBundleSettings)
        {
            if (setting.processorType == typeof(ConfigProcessor).Name)
            {
                (setting.processor as ConfigProcessor).UpdateConfig();
                break;
            }
        }
    }

    [MenuItem("ELEJO/Build/Update Protocol", false, 204)]
    public static void UpdateProtocol()
    {
        foreach (var setting in BuildSettingData.instance.assetBundleSettings)
        {
            if (setting.processorType == typeof(ProtocolProcessor).Name)
            {
                (setting.processor as ProtocolProcessor).UpdateProtocol();
                break;
            }
        }
    }

    [MenuItem("ELEJO/Assets/SVN Update")]
    public static void Update() =>
        SVNTool.Update(Selection.activeObject == null ? null : AssetDatabase.GetAssetPath(Selection.activeObject));

    public static bool SetBuildPath()
    {
        string path = EditorUtility.SaveFolderPanel(GUI_SELECT_FOLDER, BuildSettingData.instance.buildPath, string.Empty);
        if (string.IsNullOrEmpty(path)) return false;
        BuildSettingData.instance.buildPath = path.Substring(Directory.GetCurrentDirectory().Length + 1);
        return true;
    }

    private void OnGUI()
    {
        OnDrawBuildPlayer();
        OnDrawBuildAssetBundle();

        m_ScrollPostion = GUILayout.BeginScrollView(m_ScrollPostion);
        foreach (var setting in new List<AssetBundleSetting>(BuildSettingData.instance.assetBundleSettings))
        {
            if (OnDrawAssetBundleBuild(setting)) break;
        }
        GUILayout.EndScrollView();
    }

    private void OnDrawBuildPlayer()
    {
        GUILayout.BeginVertical(GUI_HELP_BOX);
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        isLocalAB = EditorGUILayout.Toggle(GUI_LOCAL_AB, isLocalAB);
        isUseTestData = EditorGUILayout.Toggle(GUI_USE_TEST_DATA, isUseTestData);
        isDebugLog = EditorGUILayout.Toggle(GUI_DEBUG_LOG, isDebugLog);
        isDebugText = EditorGUILayout.Toggle(GUI_DEBUG_TEXT, isDebugText);
        EditorSettings.autoRunPlayer = EditorGUILayout.Toggle(GUI_AUTO_RUN_PLAYER, EditorSettings.autoRunPlayer);

        if (GUILayout.Button(GUI_BUILD_DEBUG, GUILayout.Width(BUTTON_SIZE)))
        {
            BuildDebug();
            GUIUtility.ExitGUI();
        }
        else if (GUILayout.Button(GUI_BUILD_RELEASE, GUILayout.Width(BUTTON_SIZE)))
        {
            BuildRelease();
            GUIUtility.ExitGUI();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void OnDrawBuildAssetBundle()
    {
        GUILayout.BeginVertical(GUI_HELP_BOX);

        BuildSettingData.instance.packageSVN = EditorGUILayout.TextField(GUI_PACKAGE_SVN, BuildSettingData.instance.packageSVN);
        BuildSettingData.instance.packageName = EditorGUILayout.TextField(GUI_PACKAGE_NAME, BuildSettingData.instance.packageName);
        BuildSettingData.instance.productName = EditorGUILayout.TextField(GUI_PRODUCT_NAME, BuildSettingData.instance.productName);
        BuildSettingData.instance.androidVersion = EditorGUILayout.TextField(GUI_ANDROID_VERSION, BuildSettingData.instance.androidVersion);
        BuildSettingData.instance.iOSVersion = EditorGUILayout.TextField(GUI_IOS_VERSION, BuildSettingData.instance.iOSVersion);

        GUILayout.BeginHorizontal();
        EditorGUILayout.TextField(GUI_BUILD_PATH, BuildSettingData.instance.buildPath);
        if (GUILayout.Button(GUI_SELECT, GUILayout.Width(BUTTON_SIZE)))
        {
            SetBuildPath();
        }
        GUILayout.EndHorizontal();

        BuildSettingData.instance.variant = EditorGUILayout.TextField(GUI_BUILDVARIANT, BuildSettingData.instance.variant);
        BuildSettingData.instance.manifest = EditorGUILayout.TextField(GUI_BUILD_MANIFEST, BuildSettingData.instance.manifest);
        BuildSettingData.instance.config = EditorGUILayout.TextField(GUI_BUILD_CONFIG, BuildSettingData.instance.config);
        BuildSettingData.instance.buildOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(GUI_BUILD_OPTIONS, BuildSettingData.instance.buildOptions);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(GUI_ADD_SETTINGS))
        {
            BuildSettingData.instance.assetBundleSettings.Add(new AssetBundleSetting());
        }
        else if (GUILayout.Button(GUI_SAVE_SETTINGS))
        {
            BuildSettingData.instance.Save();
        }
        else if (GUILayout.Button(GUI_BUILD_ASSETBUNDLE))
        {
            BuildAssetBundle();
            GUIUtility.ExitGUI();
        }
        else if (GUILayout.Button(GUI_REBUILD_ASSETBUNDLE))
        {
            RebuildAssetBundle();
            GUIUtility.ExitGUI();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private bool OnDrawAssetBundleBuild(AssetBundleSetting setting)
    {
        bool shouldBreak = false;

        GUILayout.BeginVertical(GUI_HELP_BOX);

        setting.OnGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(GUI_MOVE_UP))
        {
            int index = BuildSettingData.instance.assetBundleSettings.IndexOf(setting);
            if (index > 0)
            {
                var temp = BuildSettingData.instance.assetBundleSettings[index - 1];
                BuildSettingData.instance.assetBundleSettings[index - 1] = setting;
                BuildSettingData.instance.assetBundleSettings[index] = temp;
            }
        }
        else if (GUILayout.Button(GUI_MOVE_DOWN))
        {
            int index = BuildSettingData.instance.assetBundleSettings.IndexOf(setting);
            if (index < BuildSettingData.instance.assetBundleSettings.Count - 1)
            {
                var temp = BuildSettingData.instance.assetBundleSettings[index + 1];
                BuildSettingData.instance.assetBundleSettings[index + 1] = setting;
                BuildSettingData.instance.assetBundleSettings[index] = temp;
            }
        }
        else if (GUILayout.Button(GUI_DELETE))
        {
            BuildSettingData.instance.assetBundleSettings.Remove(setting);
            shouldBreak = true;
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        return shouldBreak;
    }
}
