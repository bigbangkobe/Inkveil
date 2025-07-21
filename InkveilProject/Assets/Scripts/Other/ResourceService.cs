using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;

public static class ResourceService
{
    /// <summary>
    /// 泛型异步加载资源（支持 TextAsset, GameObject 等）
    /// </summary>
    public static async Task<T> LoadAsync<T>(string path) where T : UnityEngine.Object
    {
#if UNITY_WEBGL || ADDRESSABLES
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        Debug.LogError($"Failed to load addressable asset: {path}");
        return null;
#else
        ResourceRequest request = Resources.LoadAsync<T>(path);
        await AwaitResourceRequest(request);
        return request.asset as T;
#endif
    }

    /// <summary>
    /// 非泛型异步加载资源（兼容原 Resources.LoadAsync(string, Type)）
    /// </summary>
    public static async Task<UnityEngine.Object> LoadAsync(string path, Type type)
    {
#if UNITY_WEBGL || ADDRESSABLES
        AsyncOperationHandle<UnityEngine.Object> handle = Addressables.LoadAssetAsync<UnityEngine.Object>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        Debug.LogError($"Failed to load addressable asset: {path}");
        return null;
#else
        ResourceRequest request = Resources.LoadAsync(path, type);
        await AwaitResourceRequest(request);
        return request.asset;
#endif
    }

    /// <summary>
    /// 等待 ResourceRequest 的异步包装器
    /// </summary>
    private static async Task AwaitResourceRequest(ResourceRequest request)
    {
        while (!request.isDone)
            await Task.Yield();
    }
}
