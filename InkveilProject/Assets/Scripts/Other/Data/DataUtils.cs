using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

/// <summary>
/// 数据转化工具
/// </summary>
public sealed class DataUtils
{
    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="input">输入的字符串</param>
    /// <returns></returns>
    public static string FirstCharToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    /// <summary>
    /// 时间转化 yyyy-MM-dd'T'HH:mm
    /// </summary>
    /// <param name="dateStr">时间字符串</param>
    /// <returns>返回DateTime对象</returns>
    public static DateTime toHtmlDate(string dateStr)
    {
        if (dateStr == null)
        {
            return new DateTime();
        }
        if (dateStr.Length < 1)
        {
            return new DateTime();
        }

        try
        {
            //创建日期格式化对象
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd'T'HH:mm";
            //String转成对象
            return Convert.ToDateTime(dateStr, dtFormat);
        }
        catch (Exception e)
        {
            Debug.LogError("时间转化失败:" + e);
        }

        return new DateTime();
    }

    /// <summary>
    /// 时间转化 yyyy-MM-dd'T'HH:mm:ss
    /// </summary>
    /// <param name="dateStr">时间字符串</param>
    /// <returns>返回DateTime对象</returns>
    public static DateTime toHtmlDate0(string dateStr)
    {

        if (dateStr == null)
        {
            return new DateTime();
        }
        if (dateStr.Length < 1)
        {
            return new DateTime();
        }

        try
        {
            //创建日期格式化对象
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd'T'HH:mm:ss";
            //String转成对象
            return Convert.ToDateTime(dateStr, dtFormat);
        }
        catch (Exception e)
        {
            Debug.LogError("时间转化失败:" + e);
        }

        return new DateTime();
    }

    /// <summary>
    /// 转化为bool类型
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>返回bool类型</returns>
    public static bool toBoolean(string str)
    {
        try
        {
            // 空检测
            if (str == null || str.Length <= 0)
            {
                return false;
            }
            // 值检测
            if (str.Equals("true"))
            {
                return true;
            }
            if (str.Equals("True"))
            {
                return true;
            }
            if (str.Equals("TRUE"))
            {
                return true;
            }
            // 数值检测
            int v = toInt(str);
            if (v > 0)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("转化失败:" + e);
        }
        return false;
    }

    /// <summary>
    /// 将字符串转化成short
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>返回short，默认为0</returns>
    public static short toShort(string str)
    {
        try
        {
            return short.Parse(str);
        }
        catch (Exception e)
        {
            Debug.LogError("转化失败:" + e);
        }
        return 0;
    }

    /// <summary>
    /// 转化为Int
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <returns>返回Int，默认为0</returns>
    public static int toInt(string str)
    {
        try
        {
            return int.Parse(str);
        }
        catch (Exception e)
        {
            Debug.LogError("转化失败:" + e);
        }
        return 0;
    }

    /// <summary>
    /// 转化为float
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <returns>返回float,默认为0</returns>
    public static float toFloat(string str)
    {
        try
        {
            return float.Parse(str);
        }
        catch (Exception e)
        {
            Debug.LogError("转化失败:" + e);
        }
        return 0;
    }

    /// <summary>
    /// float转化为double(避免丢失精度)
    /// </summary>
    /// <param name="str">float对象</param>
    /// <returns>返回float,默认为0</returns>
    public static double toDouble(float f)
    {
        string fs = f.ToString();
        return double.Parse(fs);
    }

    /// <summary>
    /// double转化为float(避免丢失精度)
    /// </summary>
    /// <param name="str">float对象</param>
    /// <returns>返回float,默认为0</returns>
    public static float toFloat(double d)
    {
        string ds = d.ToString();
        return float.Parse(ds);
    }

    /// <summary>
    /// 转化为long
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <returns>返回long，默认为0L</returns>
    public static long toLong(String str)
    {
        try
        {
            return long.Parse(str);
        }
        catch (Exception e)
        {
            Debug.LogError("转化失败:" + e);
        }
        return 0L;
    }

    /// <summary>
    /// 裁剪字符串，将字符串转化为float[]
    /// </summary>
    /// <param name="str">裁剪的字符串</param>
    /// <param name="regex">裁剪符号</param>
    /// <returns>返回float[]</returns>
    public static float[] splitToFloat(string str, char regex)
    {
        // 空处理
        if (string.IsNullOrEmpty(str))
        {
            return new float[0];
        }
        try
        {
            // 裁剪
            string[] strs = str.Split(regex);
            float[] array = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                //trim()方法返回调用字符串对象的一个副本，但是所有起始和结尾的空格都被删除了
                array[i] = float.Parse(strs[i].Trim());
            }
            return array;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + str + ",regex:" + regex + e);
        }
        return null;
    }

    /// <summary>
    /// 2级拆分字符串，返回float[][]
    /// </summary>
    /// <param name="data">裁剪字符串</param>
    /// <param name="regexA">裁剪字符A</param>
    /// <param name="regexB">裁剪字符B</param>
    /// <returns>返回float[][]</returns>
    public static float[][] splitToFloat2(string data, char regexA, char regexB)
    {
        // 空处理
        if (string.IsNullOrEmpty(data))
        {
            return new float[0][];
        }
        try
        {
            string[] strs = data.Split(regexA);
            float[][] rets = new float[strs.Length][];
            for (int i = 0; i < strs.Length; i++)
            {
                rets[i] = splitToFloat(strs[i], regexB);
                if (rets[i] == null)
                {
                    return null; // 解析错误
                }
            }
            return rets;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + data + ",regex:" + regexA + ",regex2:"
                    + regexB + e);
        }
        return null;
    }

    /// <summary>
    /// 裁剪字符串
    /// </summary>
    /// <param name="str">裁剪对象</param>
    /// <param name="regex">裁剪符号</param>
    /// <returns>返回long[]</returns>
    public static long[] splitToLong(string str, char regex)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new long[0];
        }

        try
        {
            string[] strs = str.Split(regex);
            long[] array = new long[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                array[i] = long.Parse(strs[i].Trim());
            }
            return array;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + str + ",regex:" + regex + e);
        }
        return null;
    }

    /// <summary>
    /// 2级拆分字符串
    /// </summary>
    /// <param name="data">拆分的字符串</param>
    /// <param name="regexA">字符A</param>
    /// <param name="regexB">字符B</param>
    /// <returns>返回long[][]</returns>
    public static long[][] splitToLong2(string data, char regexA,
            char regexB)
    {
        // 空处理
        if (string.IsNullOrEmpty(data))
        {
            return new long[0][];
        }
        try
        {
            string[] strs = data.Split(regexA);
            long[][] rets = new long[strs.Length][];
            for (int i = 0; i < strs.Length; i++)
            {
                rets[0] = splitToLong(strs[i], regexB);
                if (rets[i] == null)
                {
                    return null; // 解析错误
                }
            }
            return rets;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + data + ",regex:" + regexA + ",regex2:"
                    + regexB + e);
        }
        return null;
    }

    /// <summary>
    /// 裁剪long字符串
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <returns>返回long列表</returns>
    public static List<long> splitToLong(string str)
    {
        // 空处理
        if (string.IsNullOrEmpty(str))
        {
            return new List<long>();
        }
        try
        {
            // 裁剪
            string[] strs = str.Split(',');
            List<long> longList = new List<long>(strs.Length);
            for (int i = 0; i < strs.Length; i++)
            {
                longList.Add(long.Parse(strs[i].Trim()));
            }
            return longList;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,str:" + str + e);
        }
        return null;
    }

    /// <summary>
    /// 裁剪int字符串
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <param name="regex">裁剪字符</param>
    /// <returns>返回int[]</returns>
    public static int[] splitToInt(string str, char regex)
    {
        // 空处理
        if (string.IsNullOrEmpty(str))
        {
            return new int[0];
        }
        try
        {
            // 裁剪
            string[] strs = str.Split(regex);
            int[] array = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                array[i] = int.Parse(strs[i].Trim());
            }
            return array;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,str=\"" + str + "\", regex=" + regex + e);
        }
        return null;
    }

    /// <summary>
    /// 2级拆分字符串，返回int[][]
    /// </summary>
    /// <param name="data">裁剪字符串</param>
    /// <param name="regexA">裁剪字符A</param>
    /// <param name="regexB">裁剪字符B</param>
    /// <returns>返回int[][]</returns>
    public static int[][] splitToInt2(string data, char regexA, char regexB)
    {
        // 空处理
        if (string.IsNullOrEmpty(data))
        {
            return new int[0][];
        }
        try
        {
            string[] strs = data.Split(regexA);
            int[][] rets = new int[strs.Length][];
            for (int i = 0; i < strs.Length; i++)
            {
                rets[i] = splitToInt(strs[i], regexB);
                if (rets[i] == null)
                {
                    return null; // 解析错误
                }
            }
            return rets;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + data + ",regex:" + regexA + ",regex2:"
                    + regexB + e);
        }
        return null;
    }

    /// <summary>
    /// 3级拆分字符串，返回int[][][]
    /// </summary>
    /// <param name="data">裁剪字符串</param>
    /// <param name="regexA">字符A</param>
    /// <param name="regexB">字符B</param>
    /// <param name="regexC">字符C</param>
    /// <returns>返回int[][][]</returns>
    public static int[][][] splitToInt3(string data, char regexA,
            char regexB, char regexC)
    {
        // 空处理
        if (string.IsNullOrEmpty(data))
        {
            return new int[0][][];
        }

        try
        {
            string[] strs = data.Split(regexA);
            int[][][] rets = new int[strs.Length][][];
            for (int i = 0; i < strs.Length; i++)
            {
                rets[i] = splitToInt2(strs[i], regexB, regexC);
                if (rets[i] == null)
                {
                    return null; // 解析错误
                }
            }
            return rets;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + data + ",regex:" + regexA + ",regex2:"
                    + regexB + e);
        }
        return null;
    }

    /// <summary>
    /// 裁剪字符串，返回double[]
    /// </summary>
    /// <param name="str">裁剪字符串</param>
    /// <param name="regex">裁剪字符</param>
    /// <returns></returns>
    public static double[] splitToDouble(string str, char regex)
    {
        // 空处理
        if (string.IsNullOrEmpty(str))
        {
            return new double[0];
        }
        try
        {
            // 裁剪
            string[] strs = str.Split(regex);
            double[] array = new double[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                array[i] = double.Parse(strs[i].Trim());
            }
            return array;
        }
        catch (Exception e)
        {
            Debug.LogError("拆分字符串异常,data:" + str + ",regex:" + regex + e);
        }
        return null;
    }

    /// <summary>
    /// 将long[]数组转化为字符串，用字符隔开
    /// </summary>
    /// <param name="array">数组对象</param>
    /// <param name="regex">隔开字符</param>
    /// <returns>返回字符串</returns>
    public static string toString(long[] array, string regex)
    {
        StringBuilder strBdr = new StringBuilder();
        int asize = (array != null) ? array.Length : 0;
        for (int i = 0; i < asize; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regex);
            }
            strBdr.Append(array[i]);
        }
        return strBdr.ToString();
    }

    /// <summary>
    /// 将int[]数组转化为字符串，用字符隔开
    /// </summary>
    /// <param name="array">数组对象</param>
    /// <param name="regex">隔开字符</param>
    /// <returns>返回字符串</returns>
    public static string toString(int[] array, string regex)
    {
        StringBuilder strBdr = new StringBuilder();
        int asize = (array != null) ? array.Length : 0;
        for (int i = 0; i < asize; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regex);
            }
            strBdr.Append(array[i]);
        }
        return strBdr.ToString();
    }

    /// <summary>
    /// 将int[][]二维数组转化为字符串，用字符隔开
    /// </summary>
    /// <param name="aryData">二维数组对象</param>
    /// <param name="regexA">隔开字符A 默认填","</param>
    /// <param name="regexB">隔开字符B "默认填"|"</param>
    /// <returns>返回字符串</returns>
    public static string toString(int[][] aryData, string regexA = ",",
            string regexB = "|")
    {
        StringBuilder strBdr = new StringBuilder();
        for (int i = 0, len = aryData.Length; i < len; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regexB);
            }
            strBdr.Append(toString(aryData[i], regexA));
        }
        return strBdr.ToString();
    }

    /// <summary>
    /// 将long[][]二维数组转化为字符串，用符号隔开
    /// </summary>
    /// <param name="aryData">long二维数组</param>
    /// <param name="regexA">隔开字符A 默认填","</param>
    /// <param name="regexB">隔开字符B "默认填"|"</param>
    /// <returns></returns>
    public static string toString(long[][] aryData, string regexA = ",",
            string regexB = "|")
    {
        StringBuilder strBdr = new StringBuilder();
        for (int i = 0, len = aryData.Length; i < len; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regexB);
            }
            strBdr.Append(toString(aryData[i], regexA));
        }
        return strBdr.ToString();
    }

    
    /// <summary>
    /// 将模板数组转化为字符串，用字符隔开
    /// </summary>
    /// <typeparam name="T">模板对象</typeparam>
    /// <param name="array">模板数组</param>
    /// <param name="regex">隔开字符</param>
    /// <returns>返回字符串</returns>
    public static string toString<T>(T[] array, string regex)
    {
        StringBuilder strBdr = new StringBuilder();
        int asize = (array != null) ? array.Length : 0;
        for (int i = 0; i < asize; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regex);
            }
            strBdr.Append(array[i]);
        }
        return strBdr.ToString();
    }

    /// <summary>
    /// 将对象数组列表转化为字符串，用字符隔开
    /// </summary>
    /// <typeparam name="T">模板数组列表的对象</typeparam>
    /// <param name="list">模板列表</param>
    /// <param name="regex">隔开字符</param>
    /// <returns>返回字符串</returns>
    public static string toString<T>(List<T> list, String regex)
    {
        StringBuilder strBdr = new StringBuilder();
        int asize = (list != null) ? list.Count : 0;
        for (int i = 0; i < asize; i++)
        {
            if (i > 0)
            {
                strBdr.Append(regex);
            }
            strBdr.Append(list[i]);
        }
        return strBdr.ToString();
    }

    /// <summary>
    /// 重设数组长度, 数组长度一样则不改变. (不够则增加到对应长度用0代替, 超过则删掉数据)
    /// </summary>
    /// <param name="array">重设的数组</param>
    /// <param name="size">数组长度</param>
    /// <returns>返回新的数组对象</returns>
    public static int[] resetArray(int[] array, int size)
    {
        int size0 = (array != null) ? array.Length : 0;
        if (size0 != size)
        {
            // 重新创建数据
            int[] news = new int[size];
            Array.Copy(array, 0, news, 0, size0);
            return news;
        }
        return array;
    }

    /// <summary>
    /// 重设数组长度, 数组长度一样则不改变. (不够则增加到对应长度用0代替, 超过则删掉数据)
    /// </summary>
    /// <param name="array">重设的数组</param>
    /// <param name="size">数组长度</param>
    /// <returns>返回新的数组对象</returns>
    public static long[] resetArray(long[] array, int size)
    {
        int size0 = (array != null) ? array.Length : 0;
        if (size0 != size)
        {
            // 重新创建数据
            long[] news = new long[size];
            Array.Copy(array, 0, news, 0, size0);
            return news;
        }
        return array;
    }

    /// <summary>
    /// 把数据添加到数组中(新增复制数组)
    /// </summary>
    /// <param name="aryData">复制数组对象</param>
    /// <param name="data">复制的长度</param>
    /// <returns>返回复制的新 数组</returns>
    public static int[] copyToNew(int[] aryData, int data)
    {
        int[] aryNewData = new int[aryData.Length + 1];
        Array.Copy(aryData, 0, aryNewData, 0, aryData.Length);
        aryNewData[aryNewData.Length - 1] = data;
        return aryNewData;
    }

    /// <summary>
    /// 判断数组中是否有相同的数字
    /// </summary>
    /// <param name="aryData">数组对象</param>
    /// <returns>返回是否相同</returns>
    public static bool isRepeated(int[] aryData)
    {
        for (int i = 0; i < aryData.Length - 1; i++)
        {
            for (int j = i + 1; j < aryData.Length; j++)
            {
                if (aryData[i] > 0 && aryData[i] == aryData[j])
                {
                    return true;
                }
            }
        }

        return false;
    }
}