using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    [Serializable] private class Wrapper<T> { public T[] Items = Array.Empty<T>(); }
    [Serializable] private class WrapperLower<T> { public T[] items = Array.Empty<T>(); }

    // ========= 完全替代 JsonMapper.ToObject<T>(json) =========
    public static T ToObject<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return default;
        string s = TrimBom(json).TrimStart();

        var t = typeof(T);

        // 目标类型是 List<U>
        if (IsGenericList(t, out var elem))
        {
            IList list = FromJsonListInternal(s, elem!);
            return (T)list; // List<U>
        }

        // 目标类型是 U[]
        if (t.IsArray)
        {
            var elemType = t.GetElementType()!;
            IList list = FromJsonListInternal(s, elemType);
            Array arr = Array.CreateInstance(elemType, list.Count);
            for (int i = 0; i < list.Count; i++) arr.SetValue(list[i], i);
            return (T)(object)arr;
        }

        // 普通对象
        try
        {
            return JsonUtility.FromJson<T>(s);
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonHelper.ToObject<{t.Name}> 解析失败: {e}");
            return default;
        }
    }

    // 方便：直接接受 TextAsset
    public static T ToObject<T>(TextAsset asset)
        => asset == null ? default : ToObject<T>(asset.text);

    // ========= 完全替代 JsonMapper.ToJson(obj, prettyPrint=false) =========
    public static string ToJson(object obj, bool prettyPrint = false)
    {
        if (obj == null) return "null";

        // IList（List<T> / T[]）→ 输出“纯数组 JSON”
        if (obj is IList ilist)
            return ToJsonArray(ilist, prettyPrint);

        // 普通对象
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    // ======== 内部工具 ========

    private static IList FromJsonListInternal(string s, Type elemType)
    {
        try
        {
            // 1) 裸数组：[ ... ] → 包一层 {"Items":[...]}
            if (s.StartsWith("["))
            {
                s = $"{{\"Items\":{s}}}";
                return DeserializeUpperWrapper(s, elemType);
            }

            // 2) 已包装（兼容大小写）
            if (s.IndexOf("\"Items\"", StringComparison.Ordinal) >= 0)
                return DeserializeUpperWrapper(s, elemType);

            if (s.IndexOf("\"items\"", StringComparison.Ordinal) >= 0)
                return DeserializeLowerWrapper(s, elemType);

            Debug.LogError("JsonHelper: 期望数组 JSON（如 [ {...}, {...} ]）或包装 JSON（{\"Items\":[...]})");
            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
        }
        catch (Exception e)
        {
            Debug.LogError($"JsonHelper.FromJsonListInternal<{elemType.Name}> 失败: {e}");
            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
        }
    }

    private static IList DeserializeUpperWrapper(string wrapped, Type elemType)
    {
        Type wrapperType = typeof(Wrapper<>).MakeGenericType(elemType);
        object wrapper = JsonUtility.FromJson(wrapped, wrapperType);
        var itemsField = wrapperType.GetField(nameof(Wrapper<int>.Items));
        Array arr = (Array)itemsField.GetValue(wrapper);

        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
        if (arr != null) foreach (var it in arr) list.Add(it);
        return list;
    }

    private static IList DeserializeLowerWrapper(string wrapped, Type elemType)
    {
        Type wrapperType = typeof(WrapperLower<>).MakeGenericType(elemType);
        object wrapper = JsonUtility.FromJson(wrapped, wrapperType);
        var itemsField = wrapperType.GetField(nameof(WrapperLower<int>.items));
        Array arr = (Array)itemsField.GetValue(wrapper);

        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
        if (arr != null) foreach (var it in arr) list.Add(it);
        return list;
    }

    private static string ToJsonArray(IList list, bool prettyPrint)
    {
        // 用 Wrapper<T> 生成 {"Items":[...]}，再剥壳成纯数组
        Type elemType = GetIListElementType(list);
        Type wrapperType = typeof(Wrapper<>).MakeGenericType(elemType);
        object wrapper = Activator.CreateInstance(wrapperType);

        Array arr = Array.CreateInstance(elemType, list.Count);
        for (int i = 0; i < list.Count; i++) arr.SetValue(list[i], i);

        var itemsField = wrapperType.GetField(nameof(Wrapper<int>.Items));
        itemsField.SetValue(wrapper, arr);

        string withKey = JsonUtility.ToJson(wrapper, prettyPrint);

        const string key = "\"Items\":";
        int idx = withKey.IndexOf(key, StringComparison.Ordinal);
        if (idx >= 0)
        {
            int lb = withKey.IndexOf('[', idx);
            int rb = withKey.LastIndexOf(']');
            if (lb >= 0 && rb > lb) return withKey.Substring(lb, rb - lb + 1);
        }
        return "[]"; // 兜底
    }

    private static bool IsGenericList(Type t, out Type elem)
    {
        elem = null;
        if (!t.IsGenericType) return false;
        if (t.GetGenericTypeDefinition() != typeof(List<>)) return false;
        elem = t.GetGenericArguments()[0];
        return true;
    }

    private static Type GetIListElementType(IList list)
    {
        var t = list.GetType();
        if (t.IsArray) return t.GetElementType();
        if (t.IsGenericType) return t.GetGenericArguments()[0];
        foreach (var it in list) return it?.GetType() ?? typeof(object);
        return typeof(object);
    }

    private static string TrimBom(string s)
    {
        // 处理 UTF-8 BOM
        if (!string.IsNullOrEmpty(s) && s.Length > 0 && s[0] == '\uFEFF') return s.TrimStart('\uFEFF');
        return s;
    }
}
