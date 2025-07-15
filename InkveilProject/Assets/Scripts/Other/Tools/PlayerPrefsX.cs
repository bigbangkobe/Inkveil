using UnityEngine;
using System;
using System.Collections.Generic;

public static class PlayerPrefsX
{
    /// <summary>
    /// 保存字符串数组存档
    /// </summary>
    /// <param name="key"></param>
    /// <param name="stringArray"></param>
    /// <returns></returns>
    public static bool SetStringArray(string key,List<string> stringArray)
    {
        if (stringArray.Count == 0) return false;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < stringArray.Count - 1; i++)
            sb.Append(stringArray[i]).Append("|");
        sb.Append(stringArray[stringArray.Count - 1]);

        try { PlayerPrefs.SetString(key, sb.ToString()); }
        catch { return false; }
        return true;
    }

    /// <summary>
    /// 返回字符串数组
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultInt"></param>
    /// <returns></returns>
    public static List<string> GetStringArray(string key,List<string> defaultInt = null)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
            List<string> targetArray = new List<string>();
            for (int i = 0; i < stringArray.Length; i++)
                targetArray.Add(Convert.ToString(stringArray[i]));
            return targetArray;
        }
        else if (defaultInt == null)
            return new List<string>();
        else
            return defaultInt;
    }

    /// <summary>
    /// 保存整型数组存档
    /// </summary>
    public static bool SetIntArray(string key, List<int> intArray)
    {
        if (intArray.Count == 0) return false;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < intArray.Count - 1; i++)
            sb.Append(intArray[i]).Append("|");
        sb.Append(intArray[intArray.Count - 1]);

        try { PlayerPrefs.SetString(key, sb.ToString()); }
        catch{ return false; }
        return true;
    }

    /// <summary>
    /// 返回整型数组存档
    /// </summary>
    public static List<int> GetIntArray(string key,List<int> defaultInt = null)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
            List<int> targetArray = new List<int>();
            for (int i = 0; i < stringArray.Length; i++)
                targetArray.Add(Convert.ToInt32(stringArray[i]));
            return targetArray;
        }
        else if (defaultInt == null)
            return new List<int>();
        else
            return defaultInt;
    }

    /// <summary>
    /// 保存浮点数组存档
    /// </summary>
    public static bool SetFloatArray(string key, List<float> intArray)
    {
        if (intArray.Count == 0) return false;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < intArray.Count - 1; i++)
            sb.Append(intArray[i]).Append("|");
        sb.Append(intArray[intArray.Count - 1]);

        try { PlayerPrefs.SetString(key, sb.ToString()); }
        catch{ return false; }
        return true;
    }

    /// <summary>
    /// 返回浮点数组存档
    /// </summary>
    public static List<float> GetFloatArray(string key, List<float> defaultInt = null)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
            List<float> targetArray = new List<float>();
            for (int i = 0; i < stringArray.Length; i++)
                targetArray.Add(Convert.ToSingle(stringArray[i]));
            return targetArray;
        }
        else if (defaultInt == null)
            return new List<float>();
        else
            return defaultInt;
    }
}