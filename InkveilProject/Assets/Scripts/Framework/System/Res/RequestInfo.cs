using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 资源加载信息（适配Addressables系统）
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// Addressables异步操作句柄
        /// </summary>
        public AsyncOperationHandle Handle { get; private set; }

        /// <summary>
        /// 是否常驻内存
        /// </summary>
        public bool isKeepInMemory;

        /// <summary>
        /// 加载完成之后的回调
        /// </summary>
        public List<IResLoadListener> listeners;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string assetName;

        /// <summary>
        /// 资源完整路径
        /// </summary>
        public string assetFullName => ResMgr.Instance.GetFileFullName(assetName);

        /// <summary>
        /// 资源类型
        /// </summary>
        public Type type;

        /// <summary>
        /// 资源是否加载完成
        /// </summary>
        public bool IsDone => Handle.IsValid() && Handle.IsDone;

        /// <summary>
        /// 加载到的资源
        /// </summary>
        public object Asset => Handle.IsValid() && Handle.Status == AsyncOperationStatus.Succeeded
                                ? Handle.Result
                                : null;

        /// <summary>
        /// 添加事件到事件列表中
        /// </summary>
        public void AddListener(IResLoadListener listener)
        {
            listeners ??= new List<IResLoadListener>();
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAsync()
        {
            if (type == null || !typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                Debug.LogError($"无效的资源类型: {type}");
                return;
            }

            // 修复：使用正确的参数类型获取方法
            MethodInfo method = GetLoadAssetAsyncMethod(type);
            if (method == null)
            {
                Debug.LogError($"找不到匹配的LoadAssetAsync方法: {type}");
                return;
            }
            Debug.Log($"LoadAssetAsync assetFullName: {assetFullName}");

            // 1) 调用泛型方法并获得返回值
            object rawHandle = method.Invoke(null, new object[] { assetFullName });
            // 2) 用 dynamic 触发 AsyncOperationHandle<T> -> AsyncOperationHandle 的隐式转换
            dynamic dynHandle = rawHandle;
            Handle = dynHandle;

            // 添加完成回调
            Handle.Completed += OnLoadComplete;
        }

        /// <summary>
        /// 获取正确的LoadAssetAsync方法（解决歧义问题）
        /// </summary>
        private MethodInfo GetLoadAssetAsyncMethod(Type resourceType)
        {
            // 尝试获取参数类型为 object 的泛型方法
            MethodInfo method = typeof(Addressables).GetMethod(
                "LoadAssetAsync",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(object) },
                null
            );
            if (method != null)
                return method.MakeGenericMethod(resourceType);

            // 备选方案：扫描所有泛型方法定义
            foreach (var m in typeof(Addressables).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name == "LoadAssetAsync"
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(object))
                {
                    return m.MakeGenericMethod(resourceType);
                }
            }

            return null;
        }

        private void OnLoadComplete(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"资源加载成功: {assetFullName}");
                NotifyListeners(true);
            }
            else
            {
                Debug.LogError($"资源加载失败: {assetFullName}");
                NotifyListeners(false);
            }
        }

        private void NotifyListeners(bool success)
        {
            if (listeners == null) return;

            foreach (var listener in listeners)
            {
                try
                {
                    if (success)
                        listener.Finish(Handle.Result, assetName);
                    else
                        listener.Failure();
                }
                catch (Exception e)
                {
                    Debug.LogError($"通知监听器时发生错误: {e}");
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            if (Handle.IsValid())
                Addressables.Release(Handle);
        }
    }
}
