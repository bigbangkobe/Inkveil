
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Excel读取数据进行编辑保存
/// </summary>
public class ExcelEditorWindow : EditorWindow
{
    /// <summary>
    /// 单选框选项
    /// </summary>
    List<bool> toggleValues = new List<bool>();

    /// <summary>
    /// 表数据对应类的反射（注意：FieldInfo和PropertyInfo）
    /// </summary>
    FieldInfo[] itemFieldInfoArray;

    /// <summary>
    /// 当前选择的某行数据下标
    /// </summary>
    int selectedIndex = -1;

    /// <summary>
    /// 选择的某行数据，可修改
    /// </summary>
    List<string> temperDataValues = new List<string>();

    /// <summary>
    /// 纪录ScrollView滚动的位置
    /// </summary>
    Vector2 ScrollViewContentOffset = Vector2.zero;

    /// <summary>
    /// 资源路径
    /// </summary>
    private static string assetPath;

    private const string EXT_XLSX = "*.xlsx";

    /// <summary>
    /// 配置表数据
    /// </summary>
    private ExcelWorksheet excelSheet;

    [MenuItem("ELEJO/ExcelTool/ExcelDataEditor")]
    public static void GetWindow()
    {
        //支持多选
        string[] guids = Selection.assetGUIDs;//获取当前选中的asset的GUID
        if (guids.Length > 1)
        {
            EditorUtility.DisplayDialog("错误", "不支持多选配偶表进行编辑", "OK");
            return;
        }
        Rect rect = new Rect(0, 0, 1000, 500);
        var window = GetWindowWithRect(typeof(ExcelEditorWindow), rect, true, "Excel表编辑");
        window.Show();
        for (int i = 0; i < guids.Length; i++)
        {
            assetPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + AssetDatabase.GUIDToAssetPath(guids[i]);//通过GUID获取路径
            Debug.Log(assetPath);
        }
    }

    void OnGUI()
    {
        DrawTableDatas();

        GUILayout.Space(30);

        DrawEditArea();
    }

    //显示当前xlsx导出的asset文件的内容，并添加复选框用于选择需要修改的某行数据
    void DrawTableDatas()
    {
        EditorGUILayout.BeginHorizontal();

        ScrollViewContentOffset = EditorGUILayout.BeginScrollView(ScrollViewContentOffset, GUILayout.Width(1000), GUILayout.Height(300));

        FileInfo fileInfo = new FileInfo(assetPath);
        ExcelPackage excelPackage = new ExcelPackage(fileInfo);
        ExcelWorksheets excelSheets = excelPackage.Workbook.Worksheets;
        foreach (ExcelWorksheet excelSheet0 in excelSheets)
        {
            excelSheet = excelSheet0;
            for (int i = 1,index = 0; i <= excelSheet.Dimension.Rows; ++i, index++)
            {
                EditorGUILayout.BeginHorizontal();
                toggleValues.Add(false);
                if (toggleValues[index] = EditorGUILayout.Toggle(toggleValues[index], GUILayout.Width(100)))
                {
                    if (selectedIndex != index)
                    {
                        ChangeSelect(index);
                    }
                }

                for (int j = 1; j <= excelSheet.Dimension.Columns; ++j)
                {
                    string value = excelSheet.GetValue<string>(i, j);
                    if (string.IsNullOrEmpty(value))
                    {
                        value = " ";
                    }
                    EditorGUILayout.LabelField(value, GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
        }          

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    void DrawEditArea()
    {
        if (selectedIndex != -1)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("编辑数据:"));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            //显示title
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i <= excelSheet.Dimension.Columns; i++)
            {
                EditorGUILayout.LabelField(excelSheet.GetValue<string>(1, i), GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            //显示数据
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < temperDataValues.Count; i++)
            {
                temperDataValues[i] = EditorGUILayout.TextField(temperDataValues[i], GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            //编辑按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存数据到Excel"))
            {
                WriteToExcel();
            }
            if (GUILayout.Button("保存测试数据"))
            {
                SaveToAsset();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    //读取Asset数据
    void LoadExcelData()
    {
        //itemFieldInfoArray = typeof(Item).GetFields();
    }

    //选择某行数据
    void ChangeSelect(int index)
    {
        if (selectedIndex != index)
        {
            if (selectedIndex != -1)
            {
                toggleValues[selectedIndex] = false;
            }
            toggleValues[index] = true;
        }
        selectedIndex = index;
        GetSelectDataStringValues();
    }

    //获取选择行的数据的string value
    void GetSelectDataStringValues()
    {
        temperDataValues.Clear();

        for (int i = 1; i <= excelSheet.Dimension.Columns; i++)
        {
            var value = excelSheet.GetValue<string>(selectedIndex, i);
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }
            temperDataValues.Add(value);
        }
    }

    //写入xlsx文件，注意excel文件的后缀必须为.xlsx，若为.xls则无法读取到Workbook.Worksheets
    public void WriteToExcel()
    {
        SaveToAsset();

        FileInfo xlsxFile = new FileInfo(assetPath);

        if (xlsxFile.Exists)
        {
            //通过ExcelPackage打开文件
            using (ExcelPackage package = new ExcelPackage(xlsxFile))
            {
                //修改excel的第一个sheet，下标从1开始
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                for (int i = 0; i < temperDataValues.Count; i++)
                {
                    //修复selectedIndex + 2行，i + 1列数据，下标从1开始，第一行因为是描述信息所以是selectedIndex + 2
                    worksheet.Cells[selectedIndex, i + 1].Value = temperDataValues[i];
                }

                package.Save();
                Debug.Log("WriteToExcel Success");
            }
        }
    }

    //修改的数据保存到Asset文件中
    void SaveToAsset()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SaveToAsset Success");
    }


    //类型转换
    object ConvertObject(object obj, Type type)
    {
        if (type == null)
        {
            return obj;
        }
        if (obj == null)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        Type underlyingType = Nullable.GetUnderlyingType(type);
        if (type.IsAssignableFrom(obj.GetType()))
        {
            // 如果待转换对象的类型与目标类型兼容，则无需转换
            return obj;
        }
        else if ((underlyingType ?? type).IsEnum)
        {
            // 如果待转换的对象的基类型为枚举
            if (underlyingType != null && string.IsNullOrEmpty(obj.ToString()))
            {
                // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                return null;
            }
            else
            {
                return Enum.Parse(underlyingType ?? type, obj.ToString());
            }
        }
        else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
        {
            // 如果目标类型的基类型实现了IConvertible，则直接转换
            try
            {
                return Convert.ChangeType(obj, underlyingType ?? type, null);
            }
            catch
            {
                return underlyingType == null ? Activator.CreateInstance(type) : null;
            }
        }
        else if (type.IsAssignableFrom(typeof(Vector3)))
        {
            //按照自己自定义的格式解析，例如：
            //string v = obj.ToString();
            //v = v.Replace("(", "").Replace(")", "");
            //var strValues = v.Split(',');
            //return new Vector3(Convert.ToSingle(strValues[0]), Convert.ToSingle(strValues[1]), Convert.ToSingle(strValues[2]));
        }
        else
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(obj.GetType()))
            {
                return converter.ConvertFrom(obj);
            }
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                object o = constructor.Invoke(null);
                System.Reflection.PropertyInfo[] propertys = type.GetProperties();
                Type oldType = obj.GetType();
                foreach (System.Reflection.PropertyInfo property in propertys)
                {
                    System.Reflection.PropertyInfo p = oldType.GetProperty(property.Name);
                    if (property.CanWrite && p != null && p.CanRead)
                    {
                        property.SetValue(o, ConvertObject(p.GetValue(obj, null), property.PropertyType), null);
                    }
                }
                return o;
            }
        }
        return obj;
    }

}
