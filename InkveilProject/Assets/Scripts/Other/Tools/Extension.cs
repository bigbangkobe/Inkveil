using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using XLua;

/// <summary>
/// 扩展类
/// </summary>
public static class Extension
{
    private const string TYPE_ARRAY = "[]";
    private const string FORMAT_NAME = "[{0}]";
    private const string FORMAT_DATA = "{0}, {1}";

    /// <summary>
    /// 数据进制
    /// </summary>
    private const int DATA_HEX = 1024;
    /// <summary>
    /// 数据单位
    /// </summary>
    private static readonly string[] s_DataUnits = new string[]
    {
        "B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB",
        "BB",
        "NB",
        "DB",
    };

    /// <summary>
    /// 类型表 [键:类型名 值:类型]
    /// </summary>
    private static readonly Dictionary<string, Type> s_TypeMap = new Dictionary<string, Type>()
    {
        { "boolean", typeof(bool) },
        { "char", typeof(char) },
        { "sbyte", typeof(sbyte) },
        { "byte", typeof(byte) },
        { "short", typeof(short) },
        { "ushort", typeof(ushort) },
        { "int", typeof(int) },
        { "uint", typeof(uint) },
        { "long", typeof(long) },
        { "ulong", typeof(ulong) },
        { "float", typeof(float) },
        { "double", typeof(double) },
        { "string", typeof(string) },
    };

    /// <summary>
    /// 转换字符串
    /// </summary>
    /// <param name="bytes">字节数组</param>
    /// <returns>返回字符串</returns>
    public static string ToString(byte[] bytes)
    {
        if (bytes.Length <= 0)
        {
            return string.Format(FORMAT_NAME, string.Empty);
        }

        string str = string.Empty;
        for (int i = 0; i < bytes.Length; ++i)
        {
            int value = bytes[i] > 128 ? (bytes[i] - 256) : bytes[i];
            str = i == 0 ? value.ToString() : string.Format(FORMAT_DATA, str, value);
        }

        return string.Format(FORMAT_NAME, str);
    }

    /// <summary>
    /// 获取数据单位
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="format">返回格式</param>
    /// <returns></returns>
    public static string GetDataUnit(ulong data, string format = "f1")
    {
        int index = 0;
        double temp = data;
        for (index = 0; index < s_DataUnits.Length; ++index)
        {
            if (temp < DATA_HEX)
            {
                break;
            }

            temp /= DATA_HEX;
        }

        return temp.ToString(format) + s_DataUnits[index];
    }

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="name">类型名字</param>
    /// <returns>返回类型</returns>
    public static Type GetType(string name)
    {
        if (IsArrayType(name))
        {
            Type elementType = GetType(GetElementName(name));

            return elementType != null ? elementType.MakeArrayType() : null;
        }

        Type type = null;
        s_TypeMap.TryGetValue(name, out type);

        return type;
    }

    /// <summary>
    /// 获取元素类型
    /// </summary>
    /// <param name="name">类型名字</param>
    /// <returns>返回元素类型</returns>
    public static string GetElementName(string name)
    {
        if (!IsArrayType(name))
        {
            return name;
        }

        return name.Remove(name.Length - TYPE_ARRAY.Length);
    }

    /// <summary>
    /// 判断是否数组类型
    /// </summary>
    /// <param name="name">类型名字</param>
    /// <returns>返回结果</returns>
    public static bool IsArrayType(string name)
    {
        return name.Contains(TYPE_ARRAY);
    }

    /// <summary>
    /// 字符串转MD5
    /// </summary>
    /// <param name="str">字符串对象</param>
    /// <returns>返回MD5</returns>
    public static string ToMD5(string str)
    {
        string md5 = string.Empty;
        byte[] bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str));
        for (int i = 0; i < bytes.Length; ++i)
        {
            md5 += bytes[i].ToString("x").PadLeft(2, '0');
        }

        return md5;
    }

    /// <summary>
    /// 转换字节数组
    /// </summary>
    /// <param name="luaTable">Lua表</param>
    /// <returns>返回字节数组</returns>
    //public static byte[] LuaTableToBytes(LuaTable luaTable)
    //{
    //    byte[] bytes = new byte[luaTable.Length];
    //    for (int i = 0; i < bytes.Length; ++i)
    //    {
    //        bytes[i] = luaTable.Get<int, byte>(i + 1);
    //    }

    //    return bytes;
    //}

    /// <summary>
    /// 手机震动
    /// </summary>
    public static void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IPHONE
		Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// 获取动画状态
    /// </summary>
    /// <param name="animation">动画对象</param>
    /// <param name="name">动画名</param>
    /// <returns>返回动画状态</returns>
    public static AnimationState GetAnimationState(this Animation animation, string name)
    {
        return animation[name];
    }

    /// <summary>
    /// 射线碰撞
    /// </summary>
    /// <param name="origin">开始</param>
    /// <param name="direction">方向</param>
    /// <param name="hitInfo">碰撞信息</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">图层蒙版</param>
    /// <returns>返回结果</returns>
    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
    {
        return Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
    }

    /// <summary>
    /// 添加事件触发器
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="callback">事件回调</param>
    /// <param name="childName">节点名</param>
    public static void AddTrigger(this EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = null;
        for (int i = 0; i < eventTrigger.triggers.Count; ++i)
        {
            EventTrigger.Entry trigger = eventTrigger.triggers[i];
            if (trigger.eventID == eventType)
            {
                entry = trigger;
                break;
            }
        }

        if (entry == null)
        {
            entry = new EventTrigger.Entry()
            {
                eventID = eventType,
            };
        }
        entry.callback.AddListener(callback);

        eventTrigger.triggers.Add(entry);
    }

    /// <summary>
    /// 移除事件触发器
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="callback">事件回调</param>
    /// <param name="childName">节点名</param>
    public static void RemoveTrigger(this EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        for (int i = 0; i < eventTrigger.triggers.Count; ++i)
        {
            EventTrigger.Entry entry = eventTrigger.triggers[i];
            if (entry.eventID == eventType)
            {
                entry.callback.RemoveListener(callback);
                return;
            }
        }
    }

    /// <summary>
    /// 移除所有事件触发器
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="callback">事件回调</param>
    /// <param name="childName">节点名</param>
    public static void RemoveAllTrigger(this EventTrigger eventTrigger, EventTriggerType eventType)
    {
        for (int i = 0; i < eventTrigger.triggers.Count; ++i)
        {
            EventTrigger.Entry entry = eventTrigger.triggers[i];
            if (entry.eventID == eventType)
            {
                entry.callback.RemoveAllListeners();
                eventTrigger.triggers.Remove(entry);
                return;
            }
        }
    }

    /// <summary>
    /// 转二维向量
    /// </summary>
    /// <param name="vector">向量</param>
    /// <returns>返回向量</returns>
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return vector;
    }

    /// <summary>
    /// 转三维向量
    /// </summary>
    /// <param name="vector">向量</param>
    /// <returns>返回向量</returns>
    public static Vector3 ToVector3(this Vector2 vector)
    {
        return vector;
    }

    public static void ResetDropDownOptions(Dropdown dropDown, string[] optionArray)
    {
        dropDown.ClearOptions();
        List<string> options = new List<string>(optionArray);
        dropDown.AddOptions(options);
    }
}