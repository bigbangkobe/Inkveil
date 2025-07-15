using System;
using System.Collections.Generic;
using System.Reflection;

public class JenkinsBatData
{
    /// <summary>
    /// ����
    /// </summary>
    public static string Channel = "--Channel:";
    /// <summary>
    /// ƽ̨
    /// </summary>
    public static string Platform = "--Platform:";
    /// <summary>
    /// �Ƿ�������ģʽ
    /// </summary>
    public static string DebugMode = "--DebugMode:";
    /// <summary>
    /// ��־����
    /// </summary>
    public static string Log = "--Log:";
    /// <summary>
    /// �������
    /// </summary>
    public static string ApkType = "--ApkType:";
    /// <summary>
    /// ����
    /// </summary>
    public static string ProductName = "--ProductName:";
    /// <summary>
    /// �汾
    /// </summary>
    public static string Version = "--Version:";
    /// <summary>
    /// apk�����·��
    /// </summary>
    public static string ApkPath = "--ApkPath:";
    /// <summary>
    /// ��Դ·��
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
            string name = item.Name; //����
            object value = item.GetValue(t);  //ֵ

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
