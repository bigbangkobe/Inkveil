using OfficeOpenXml;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Numerics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excel工具编辑类
/// </summary>
public class ExcelDataTool : EditorWindow
{
    /// <summary>
    /// 配置表数据生成C#的路径
    /// </summary>
    public string csPath
    {
        get { return EditorPrefs.GetString(PREFS_PREFIX + "_CSPath", string.Empty); }
        set { EditorPrefs.SetString(PREFS_PREFIX + "_CSPath", value); }
    }

    /// <summary>
    /// 类的名称
    /// </summary>
    public string className = string.Empty;

    private const string EXT_XLSX = "*.xls";
    private const string FORMAT_PATH = "{0}/{1}";
    private const string GUI_HELP_BOX = "HelpBox";
    private const string GUI_CONFIG_PATH = "Config Path";
    private const string GUI_SELECT_FOLDER = "Select Folder";
    private const string GUI_SELECT = "Select";
    private const string GUI_UPDATE = "Generate";
    private const string GUI_TITLE = "Update Config [{0}/{1}]";
    private const string GUI_INFO = "Updating...{0}";
    private const string TYPE_INT = "int";
    private const string TYPE_FLOAT = "float";
    private const string TYPE_NULL = "null";
    private const string TYPE_EMPTY = "\"\"";

    private const string GUI_NAME = "Name";
    private const string GUI_PATH = "Path";
    private const string GUI_SELECT_SAVE = "Select";
    private const string GUI_SELECT_FOLDER_SAVE = "Select Folder";
    private const string GUI_SEARCH_PATTERN = "Search Pattern";

    /// <summary>
    /// 按钮大小
    /// </summary>
    public const int BUTTON_SIZE = 120;

    /// <summary>
    /// GUI位置点
    /// </summary>
    private UnityEngine.Vector2 m_ScrollPostion;

    /// <summary>
    /// 搜索选项
    /// </summary>
    public SearchOption searchOption = SearchOption.AllDirectories;

    /// <summary>
    /// 搜索参数
    /// </summary>
    //public string searchPattern = string.Empty;

    private static readonly string PREFS_PREFIX = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;

    /// <summary>
    /// 保存编辑器数据的标签
    /// </summary>
    private static readonly string PREFS_CONFIG_PATH = PREFS_PREFIX + "_ExcelDataPath";

    /// <summary>
    /// 数据路径
    /// </summary>
    public static string configPath
    {
        get { return EditorPrefs.GetString(PREFS_CONFIG_PATH, string.Empty); }
        set { EditorPrefs.SetString(PREFS_CONFIG_PATH, value); }
    }

    [MenuItem("ELEJO/ExcelTool/ExcelToCS")]
    public static void ExcelToCS()
    {
        GetWindow<ExcelDataTool>();
    }

    /// <summary>
    /// 将配置表数据生成C#类
    /// </summary>
    public void GenerateExcelToCsharp()
    {
        if (string.IsNullOrEmpty(configPath))
        {
            if (!SetConfigPath())
            {
                return;
            }
        }
        DateTime tick = DateTime.Now;
        DirectoryInfo directoryInfo = new DirectoryInfo(configPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles(EXT_XLSX, SearchOption.AllDirectories);
        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileInfo fileInfo = fileInfos[i];
            ExcelPackage excelPackage = new ExcelPackage(fileInfo);
            ExcelWorksheets excelSheets = excelPackage.Workbook.Worksheets;

            foreach (ExcelWorksheet excelSheet in excelSheets)
            {
                string className = DataUtils.FirstCharToUpper(excelSheet.Name) + "Info";

                CodeTypeDeclaration myClass = new CodeTypeDeclaration(className); //生成类
                myClass.IsClass = true;
                myClass.TypeAttributes = TypeAttributes.Public;
                myClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Serializable"))); //添加序列化的特性

                string title = string.Format(GUI_TITLE, i + 1, fileInfos.Length);
                string info = string.Format(GUI_INFO, excelSheet.Name);
                EditorUtility.DisplayProgressBar(title, info, (float)(i + 1) / fileInfos.Length);

                //string path1 = fileInfo.FullName.Substring(directoryInfo.FullName.Length).Replace('\\', '/');
                //path1 = path + path1;
                //path1 = path1.Remove(path1.LastIndexOf('/'));
                //path1 = string.Format(FORMAT_PATH, path1, excelSheet.Name) + ext;

                //FileInfo newFileInfo = new FileInfo(path1);
                //if (!newFileInfo.Directory.Exists)
                //{
                //    newFileInfo.Directory.Create();
                //}
                for (int j = 1; j <= excelSheet.Dimension.Columns; ++j)
                {
                    string type = excelSheet.GetValue<string>(2, j); //这边我的Excel第三行填的是数据类型，这里把他读出来
                    string filed = excelSheet.GetValue<string>(1, j);//第一行字段名
                    string desc = excelSheet.GetValue<string>(3, j); //第二行是描述
                    CodeMemberField member = new CodeMemberField(GetTheType(type), filed); //生成字段
                    member.Attributes = MemberAttributes.Public;
                    CodeCommentStatementCollection collection = new CodeCommentStatementCollection();
                    member.Comments.Add(new CodeCommentStatement(desc));
                    myClass.Members.Add(member); //把生成的字段加入到生成的类中
                }


                //生成代码
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();    //代码生成风格
                options.BracingStyle = "C";
                options.BlankLinesBetweenMembers = true;

                string outputPath = csPath + @"\" + className + ".cs";     //指定文件的输出路径

                using (StreamWriter sw = new StreamWriter(outputPath))
                {
                    provider.GenerateCodeFromType(myClass, sw, options); //生成文件
                }
                //string content = ToJson(excelSheet);
                //File.WriteAllText(path1, content);
            }
        }
        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
        TimeSpan timeSpan = DateTime.Now - tick;
        EditorUtility.DisplayDialog("生成代码成功", string.Format("Build Complete", timeSpan), "OK");
    }

    /// <summary>
    /// GUI函数
    /// </summary>
    public void OnGUI()
    {
        m_ScrollPostion = GUILayout.BeginScrollView(m_ScrollPostion);


        EditorGUILayout.TextField(GUI_PATH, csPath);
        if (GUILayout.Button(GUI_SELECT, GUILayout.Width(60)))
        {
            SetBuildPath();
        }

        EditorGUILayout.TextField(GUI_CONFIG_PATH, configPath);
        if (GUILayout.Button(GUI_SELECT, GUILayout.Width(BUTTON_SIZE)))
        {
            SetConfigPath();
        }
        else if (GUILayout.Button(GUI_UPDATE, GUILayout.Width(BUTTON_SIZE)))
        {
            GenerateExcelToCsharp();
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 设置构建路径
    /// </summary>
    public void SetBuildPath()
    {
        string temp = EditorUtility.SaveFolderPanel(GUI_SELECT_FOLDER_SAVE, csPath, null);
        if (!string.IsNullOrEmpty(temp))
        {
            csPath = temp.Substring(Directory.GetCurrentDirectory().Length + 1);
        }
    }

    /// <summary>
    /// 设置构建目录
    /// </summary>
    /// <returns>返回设置结果</returns>
    private bool SetConfigPath()
    {
        string path = EditorUtility.OpenFolderPanel(GUI_SELECT_FOLDER, configPath, null);
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        configPath = path;

        return true;
    }

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <returns></returns>
    private static Type GetTheType(string type)
    {
        switch (type)
        {
            case "string":
                return typeof(String);
            case "int":
                return typeof(Int32);
            case "float":
                return typeof(Single);
            case "long":
                return typeof(Int64);
            case "double":
                return typeof(Double);
            case "bigInteger":
                return typeof(BigInteger);
            default:
                return typeof(String);
        }
    }
}
