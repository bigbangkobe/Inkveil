using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;

public static class ResourceService
{
    #region 内部数据结构
    private class ResourceHandle
    {
        public AsyncOperationHandle Handle;
        public int RefCount;
        public Type ResourceType;
    }

    private static readonly Dictionary<string, ResourceHandle> _resourceHandles =
        new Dictionary<string, ResourceHandle>();

    private static readonly Dictionary<UnityEngine.Object, string> _instantiatedObjects =
        new Dictionary<UnityEngine.Object, string>();
    #endregion

    #region 公共接口

    /// <summary>
    /// 卸载指定资源（Addressables 1.22.3版本使用Release实现）
    /// </summary>
    public static void UnloadAsset(UnityEngine.Object asset)
    {
        if (asset == null) return;

        // 查找资源路径
        string pathToRemove = null;
        foreach (var kvp in _resourceHandles)
        {
            if (kvp.Value.Handle.Result == asset)
            {
                pathToRemove = kvp.Key;
                break;
            }
        }

        if (pathToRemove != null)
        {
            Release(pathToRemove);
        }
        else
        {
            // 如果不是通过ResourceService加载的，直接调用Addressables释放
            Addressables.Release(asset);
        }
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("资源路径不能为空");
            return null;
        }

        // 检查缓存
        if (_resourceHandles.TryGetValue(path, out var handle) &&
            handle.ResourceType == typeof(T))
        {
            handle.RefCount++;
            return (T)handle.Handle.Result;
        }

        // 新加载（同步阻塞）
        var operation = Addressables.LoadAssetAsync<T>(path);
        operation.WaitForCompletion();

        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            _resourceHandles[path] = new ResourceHandle
            {
                Handle = operation,
                RefCount = 1,
                ResourceType = typeof(T)
            };
            return operation.Result;
        }

        Debug.LogError($"资源加载失败: {path}");
        return null;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public static AsyncOperationHandle<T> LoadAsync<T>(string path, Action<T> onComplete = null) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("资源路径不能为空");
            onComplete?.Invoke(null);
            return default;
        }

        var operation = Addressables.LoadAssetAsync<T>(path);

        operation.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                CacheResource(path, op, typeof(T));
                onComplete?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"加载失败: {path}");
                onComplete?.Invoke(null);
            }
        };

        return operation;
    }

    /// <summary>
    /// 通过Type异步加载
    /// </summary>
    public static AsyncOperationHandle LoadAsync(string path, Type type, Action<UnityEngine.Object> onComplete = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("资源路径不能为空");
            onComplete?.Invoke(null);
            return default;
        }

        if (type == null || !typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            Debug.LogError($"无效的资源类型: {type}");
            onComplete?.Invoke(null);
            return default;
        }

        // 使用反射调用泛型方法
        var method = typeof(Addressables).GetMethod("LoadAssetAsync").MakeGenericMethod(type);
        var operation = (AsyncOperationHandle)method.Invoke(null, new object[] { path });

        operation.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                CacheResource(path, op, type);
                onComplete?.Invoke(op.Result as UnityEngine.Object);
            }
            else
            {
                Debug.LogError($"加载失败: {path}");
                onComplete?.Invoke(null);
            }
        };

        return operation;
    }
    #endregion

    #region 实例化管理
    /// <summary>
    /// 实例化游戏对象
    /// </summary>
    public static GameObject Instantiate(string path,
        Vector3 position = default,
        Quaternion rotation = default,
        Transform parent = null)
    {
        var prefab = Load<GameObject>(path);
        if (prefab == null) return null;

        var instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
        TrackInstance(instance, path);
        return instance;
    }

    /// <summary>
    /// 异步实例化游戏对象
    /// </summary>
    public static void InstantiateAsync(string path,
        Action<GameObject> onComplete = null,
        Vector3 position = default,
        Quaternion rotation = default,
        Transform parent = null)
    {
        LoadAsync<GameObject>(path, prefab =>
        {
            if (prefab != null)
            {
                var instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
                TrackInstance(instance, path);
                onComplete?.Invoke(instance);
            }
            else
            {
                onComplete?.Invoke(null);
            }
        });
    }

    /// <summary>
    /// 销毁实例并释放资源
    /// </summary>
    public static void Destroy(GameObject instance)
    {
        if (instance == null) return;

        if (_instantiatedObjects.TryGetValue(instance, out var path))
        {
            _instantiatedObjects.Remove(instance);
            UnityEngine.Object.Destroy(instance);
            Release(path);
        }
        else
        {
            UnityEngine.Object.Destroy(instance);
        }
    }
    #endregion

    #region 资源管理
    /// <summary>
    /// 释放资源
    /// </summary>
    public static void Release(string path)
    {
        if (_resourceHandles.TryGetValue(path, out var handle))
        {
            handle.RefCount--;

            if (handle.RefCount <= 0)
            {
                // 检查是否有实例还在使用这个资源
                bool isInUse = false;
                foreach (var kvp in _instantiatedObjects)
                {
                    if (kvp.Value == path && kvp.Key != null)
                    {
                        isInUse = true;
                        break;
                    }
                }

                if (!isInUse)
                {
                    Addressables.Release(handle.Handle);
                    _resourceHandles.Remove(path);
                }
            }
        }
    }

    /// <summary>
    /// 卸载未使用的资源
    /// </summary>
    public static void UnloadUnusedAssets()
    {
        List<string> toRemove = new List<string>();

        foreach (var kvp in _resourceHandles)
        {
            bool isInUse = false;

            // 检查是否有实例引用
            foreach (var instance in _instantiatedObjects)
            {
                if (instance.Value == kvp.Key && instance.Key != null)
                {
                    isInUse = true;
                    break;
                }
            }

            if (!isInUse && kvp.Value.RefCount <= 0)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            Addressables.Release(_resourceHandles[key].Handle);
            _resourceHandles.Remove(key);
        }

        // 调用Unity原生资源清理
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public static void ClearAll()
    {
        foreach (var handle in _resourceHandles.Values)
        {
            Addressables.Release(handle.Handle);
        }

        _resourceHandles.Clear();
        _instantiatedObjects.Clear();
    }
    #endregion

    #region 内部方法
    private static void CacheResource(string path, AsyncOperationHandle handle, Type type)
    {
        if (_resourceHandles.TryGetValue(path, out var existingHandle))
        {
            existingHandle.RefCount++;
        }
        else
        {
            _resourceHandles[path] = new ResourceHandle
            {
                Handle = handle,
                RefCount = 1,
                ResourceType = type
            };
        }
    }

    private static void TrackInstance(GameObject instance, string path)
    {
        _instantiatedObjects[instance] = path;

        // 添加自动释放组件
        var tracker = instance.AddComponent<ResourceTracker>();
        tracker.Path = path;
    }

    private class ResourceTracker : MonoBehaviour
    {
        public string Path { get; set; }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                _instantiatedObjects.Remove(gameObject);
                Release(Path);
            }
        }
    }
    #endregion
}