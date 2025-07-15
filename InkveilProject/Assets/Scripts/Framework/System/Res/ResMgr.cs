using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using LitJson;

public class ResMgr : EventNode, IEventListener
{
    #region 内部数据结构
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// 资源对象
        /// </summary>
        public object asset;

        /// <summary>
        /// 是否常驻内存
        /// </summary>
        public bool isKeepInMemory;

        /// <summary>
        /// 栈计数
        /// </summary>
        public int stackCount;
    }

    /// <summary>
    /// 资源配置信息
    /// </summary>
    [Serializable]
    public class ResourceInfo
    {
        public string Name;
        public string Path;
    }
    #endregion

    #region 单例和初始化
    private static ResMgr mInstance;
    public static ResMgr Instance => mInstance;

    /// <summary>
    /// 资源路径字典[key:资源名称,value:资源路径]
    /// </summary>
    private Dictionary<string, string> mAssetPathDic = new Dictionary<string, string>();

    /// <summary>
    /// 所有资源字典
    /// </summary>
    private Dictionary<string, AssetInfo> mDicAsset = new Dictionary<string, AssetInfo>();

    /// <summary>
    /// CPU 个数
    /// </summary>
    private int mProcessorCount = 0;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Awake()
    {
        mInstance = this;
        AttachEventListener(EventDefine.ResLoadFinish, this);
        mProcessorCount = SystemInfo.processorCount > 0 && SystemInfo.processorCount <= 8 ?
                          SystemInfo.processorCount : 1;

        LoadResourceConfig();
    }

    /// <summary>
    /// 加载资源配置
    /// </summary>
    private void LoadResourceConfig()
    {
        // 使用Addressables加载资源配置
        var handle = Addressables.LoadAssetAsync<TextAsset>("Config/resourcesConfig");
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                List<ResourceInfo> resourceInfos = JsonMapper.ToObject<List<ResourceInfo>>(op.Result.text);
                foreach (var resourceInfo in resourceInfos)
                {
                    mAssetPathDic[resourceInfo.Name] = resourceInfo.Path;
                }
          
            }
            else
            {
                Debug.LogError("加载资源配置文件失败");
            }
        };
    }

    /// <summary>
    /// 摧毁函数
    /// </summary>
    public void OnDestroy()
    {
        DetachEventListener(EventDefine.ResLoadFinish, this);
        ClearAllResources();
    }
    #endregion

    #region 资源路径管理
    /// <summary>
    /// 通过资源名称获得整个资源路径
    /// </summary>
    public string GetFileFullName(string assetName)
    {
        if (mAssetPathDic.TryGetValue(assetName, out var path))
        {
            return path;
        }

        // 如果未找到配置，返回原名称
        return assetName;
    }
    #endregion

    #region 加载队列管理
    /// <summary>
    /// 正在加载的列表
    /// </summary>
    public List<RequestInfo> mInLoads = new List<RequestInfo>();

    /// <summary>
    /// 等待加载的列表
    /// </summary>
    public Queue<RequestInfo> mWaitting = new Queue<RequestInfo>();

    /// <summary>
    /// 更新函数 - 处理加载队列
    /// </summary>
    void Update()
    {
        // 处理已完成加载
        for (int i = mInLoads.Count - 1; i >= 0; i--)
        {
            if (mInLoads[i] == null)
            {
                mInLoads.RemoveAt(i);
                continue;
            }

            if (mInLoads[i].IsDone)
            {
                var info = mInLoads[i];
                SendEvent(EventDefine.ResLoadFinish, info);
                mInLoads.RemoveAt(i);
            }
        }

        // 开始新加载
        while (mInLoads.Count < mProcessorCount && mWaitting.Count > 0)
        {
            var info = mWaitting.Dequeue();
            if (info == null) continue;

            try
            {
                mInLoads.Add(info);
                info.LoadAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"加载资源失败: {info.assetName}\n{e}");
                mInLoads.Remove(info);
            }
        }
    }
    #endregion

    #region 资源加载接口
    /// <summary>
    /// 加载资源
    /// </summary>
    public void Load(string assetName, IResLoadListener listener, Type type = null,
                    bool isKeepInMemory = false, bool isAsync = true)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            Debug.LogError("资源名称不能为空");
            listener.Failure();
            return;
        }

        // 检查缓存
        if (mDicAsset.TryGetValue(assetName, out var assetInfo))
        {
            listener.Finish(assetInfo.asset, assetName);
            return;
        }

        if (isAsync)
        {
            LoadAsync(assetName, listener, isKeepInMemory, type);
        }
        else
        {
            // 同步加载（不推荐）
            try
            {
                var asset = LoadSync(assetName, type ?? typeof(UnityEngine.Object));
                listener.Finish(asset, assetName);
            }
            catch (Exception e)
            {
                Debug.LogError($"同步加载失败: {assetName}\n{e}");
                listener.Failure();
            }
        }
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    private object LoadSync(string assetName, Type type)
    {
        string path = GetFileFullName(assetName);

        // 使用反射调用Addressables同步加载
        var method = typeof(Addressables).GetMethod("LoadAssetAsync");
        if (method == null)
        {
            throw new MissingMethodException("找不到LoadAssetAsync方法");
        }

        var genericMethod = method.MakeGenericMethod(type);
        var handle = (AsyncOperationHandle)genericMethod.Invoke(null, new object[] { path });

        // 等待加载完成
        return handle.WaitForCompletion();
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    private void LoadAsync(string assetName, IResLoadListener listener, bool isKeepInMemory, Type type)
    {
        // 检查是否已经在加载中
        foreach (var request in mInLoads)
        {
            if (request.assetName == assetName)
            {
                request.AddListener(listener);
                return;
            }
        }

        // 检查是否在等待队列中
        foreach (var request in mWaitting)
        {
            if (request.assetName == assetName)
            {
                request.AddListener(listener);
                return;
            }
        }

        // 创建新的加载请求
        var requestInfo = new RequestInfo
        {
            assetName = assetName,
            isKeepInMemory = isKeepInMemory,
            type = type ?? typeof(UnityEngine.Object)
        };
        requestInfo.AddListener(listener);
        mWaitting.Enqueue(requestInfo);
    }
    #endregion

    #region 资源栈管理
    /// <summary>
    /// 资源加载堆栈
    /// </summary>
    public Stack<List<string>> mAssetStack = new Stack<List<string>>();

    /// <summary>
    /// 把资源压入顶层栈内
    /// </summary>
    public void AddAssetToName(string assetName)
    {
        if (mAssetStack.Count == 0)
        {
            mAssetStack.Push(new List<string>());
        }

        List<string> list = mAssetStack.Peek();
        list.Add(assetName);
    }

    /// <summary>
    /// 开始让资源入栈
    /// </summary>
    public void PushAssetStack()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, AssetInfo> info in mDicAsset)
        {
            info.Value.stackCount++;
            list.Add(info.Key);
        }

        mAssetStack.Push(list);
    }

    /// <summary>
    /// 释放栈内资源
    /// </summary>
    public void PopAssetStack()
    {
        if (mAssetStack.Count == 0) return;

        List<string> list = mAssetStack.Pop();
        List<string> removeList = new List<string>();
        AssetInfo info = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (mDicAsset.TryGetValue(list[i], out info))
            {
                info.stackCount--;
                if (info.stackCount < 1 && !info.isKeepInMemory)
                {
                    removeList.Add(list[i]);
                }
            }
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            if (mDicAsset.ContainsKey(removeList[i]))
                mDicAsset.Remove(removeList[i]);
        }

        // 执行垃圾回收
        GC();
    }
    #endregion

    #region 资源管理
    /// <summary>
    /// 从资源字典中取得一个资源
    /// </summary>
    public AssetInfo GetAsset(string assetName)
    {
        mDicAsset.TryGetValue(assetName, out var info);
        return info;
    }

    /// <summary>
    /// 释放一个资源
    /// </summary>
    public void ReleaseAsset(string assetName)
    {
        if (mDicAsset.TryGetValue(assetName, out var info) && !info.isKeepInMemory)
        {
            mDicAsset.Remove(assetName);
        }
    }

    /// <summary>
    /// 修改资源是否常驻内存
    /// </summary>
    public void IsKeepInMemory(string assetName, bool IsKeepInMemory)
    {
        if (mDicAsset.TryGetValue(assetName, out var info))
        {
            info.isKeepInMemory = IsKeepInMemory;
        }
    }
    #endregion

    #region 内存管理
    /// <summary>
    /// 垃圾回收
    /// </summary>
    public void GC()
    {
        // 1. 清理Addressables缓存
        Addressables.CleanBundleCache();

        // 2. 释放未使用的资源
        ReleaseUnusedAssets();

        // 3. 调用Unity内存回收
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    /// <summary>
    /// 释放未使用的资源
    /// </summary>
    private void ReleaseUnusedAssets()
    {
        List<string> toRemove = new List<string>();

        foreach (var kvp in mDicAsset)
        {
            var assetInfo = kvp.Value;

            // 满足释放条件：非常驻内存且引用计数为0
            if (!assetInfo.isKeepInMemory && assetInfo.stackCount <= 0)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            mDicAsset.Remove(key);
        }
    }

    /// <summary>
    /// 清空所有资源
    /// </summary>
    private void ClearAllResources()
    {
        // 释放所有正在加载的资源
        foreach (var request in mInLoads)
        {
            request?.Release();
        }
        mInLoads.Clear();

        // 释放所有等待加载的资源
        mWaitting.Clear();

        // 释放所有已加载的资源
        foreach (var kvp in mDicAsset)
        {
            if (kvp.Value.asset is UnityEngine.Object obj)
            {
                Addressables.Release(obj);
            }
        }
        mDicAsset.Clear();

        mAssetStack.Clear();
    }
    #endregion

    #region 事件处理
    /// <summary>
    /// 处理资源加载完成事件
    /// </summary>
    public bool HandleEvent(int id, object param1, object param2)
    {
        switch (id)
        {
            case EventDefine.ResLoadFinish:
                RequestInfo info = param1 as RequestInfo;
                if (info != null)
                {
                    if (info.Asset != null)
                    {
                        // 添加到资源字典
                        var assetInfo = new AssetInfo
                        {
                            asset = info.Asset,
                            isKeepInMemory = info.isKeepInMemory
                        };

                        if (!mDicAsset.ContainsKey(info.assetName))
                        {
                            mDicAsset.Add(info.assetName, assetInfo);
                        }

                        // 添加到资源栈
                        AddAssetToName(info.assetName);
                    }
                }
                //print("加载所有资源完成");
           
                return false;
        }
        return false;
    }

    public int EventPriority() => 0;
    #endregion
}