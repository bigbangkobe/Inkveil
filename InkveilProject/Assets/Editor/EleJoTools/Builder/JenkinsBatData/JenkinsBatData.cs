using System;
using System.Collections.Generic;
using System.Reflection;

public class JenkinsBatData
{
    /// <summary>
    /// 渠道
    /// </summary>
    public static string Channel = "--Channel:";
    /// <summary>
    /// 平台
    /// </summary>
    public static string Platform = "--Platform:";
    /// <summary>
    /// 是否开启调试模式
    /// </summary>
    public static string DebugMode = "--DebugMode:";
    /// <summary>
    /// 日志开关
    /// </summary>
    public static string Log = "--Log:";
    /// <summary>
    /// 打包类型
    /// </summary>
    public static string ApkType = "--ApkType:";
    /// <summary>
    /// 包名
    /// </summary>
    public static string ProductName = "--ProductName:";
    /// <summary>
    /// 版本
    /// </summary>
    public static string Version = "--Version:";
    /// <summary>
    /// apk包存放路径
    /// </summary>
    public static string ApkPath = "--ApkPath:";
    /// <summary>
    /// 资源路径
    /// </summary>
    public static string AssetPath = "--AssetPath:";



    public static Dictionary<string, string> JenkinsDataDic()
    {

        Type type = typeof(JenkinsBatData);
        FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        Dictionary<string, string> JenkinsData = new Dictionary<string, string>();
        foreach (var fieldsData in fields)
        {
            JenkinsData.Add(fieldsData.Name, fieldsData.GetValue(fieldsData.GetType()).ToString());
        }
        return JenkinsData;
    }




    public List<string> jenkinsData()
    {
        return GetFields(this.GetType());
    }


    public List<string> GetFields<T>(T t)
    {
        List<string> ListStr = new List<string>();
        if (t == null)
        {
            return ListStr;
        }
        System.Reflection.FieldInfo[] fields = t.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (fields.Length <= 0)
        {
            return ListStr;
        }
        foreach (System.Reflection.FieldInfo item in fields)
        {
            string name = item.Name; //名称
            object value = item.GetValue(t);  //值

            if (item.FieldType.IsValueType || item.FieldType.Name.StartsWith("String"))
            {
                ListStr.Add(name);
            }
            else
            {
                GetFields(value);
            }
        }
        return ListStr;
    }
}
