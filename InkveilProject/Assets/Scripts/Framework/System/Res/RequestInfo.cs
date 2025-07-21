using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 资源加载信息
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// 模拟的资源反馈信息（为了兼容旧逻辑，不再访问其 .asset）
        /// </summary>
        public ResourceRequest request;

        /// <summary>
        /// 是否常驻内存
        /// </summary>
        public bool isKeepInMemory;

        /// <summary>
        /// 加载完成之后的回调
        /// </summary>
        public List<IResLoadListener> linsteners;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string assetName;

        /// <summary>
        /// 资源类型
        /// </summary>
        public Type type;

        /// <summary>
        /// 是否加载完成（用于替代 request.isDone）
        /// </summary>
        private bool _isDone = false;

        /// <summary>
        /// 加载完成的资源对象
        /// </summary>
        private UnityEngine.Object _loadedAsset;

        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool IsDone => _isDone;

        /// <summary>
        /// 加载到的资源对象（对外访问）
        /// </summary>
        public object Asset => _loadedAsset;

        /// <summary>
        /// 添加加载监听者
        /// </summary>
        public void AddListener(IResLoadListener listener)
        {
            if (linsteners == null)
            {
                linsteners = new List<IResLoadListener> { listener };
            }
            else if (!linsteners.Contains(listener))
            {
                linsteners.Add(listener);
            }
        }

        /// <summary>
        /// 获取资源路径（异步）
        /// </summary>
        public async Task<string> GetAssetFullNameAsync()
        {
            return await ResMgr.Instance.GetFileFullName(assetName);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public async void LoadAsync()
        {
            try
            {
                if (type == null)
                {
                    Debug.LogWarning($"[RequestInfo] type 为 null，默认使用 GameObject 类型: {assetName}");
                    type = typeof(GameObject);
                }

                string fullPath = await GetAssetFullNameAsync();
                var loadedAsset = await ResourceService.LoadAsync(fullPath, type);

                _loadedAsset = loadedAsset;

                // 构造一个假的 ResourceRequest（兼容旧逻辑，但不再访问其 .asset）
                var fakeRequest = new FakeResourceRequest();
                fakeRequest.SetAsset(loadedAsset);
                request = fakeRequest;

                _isDone = true;

                //// 通知监听者
                //if (linsteners != null)
                //{
                //    foreach (var listener in linsteners)
                //    {
                //        listener?.OnLoaded(loadedAsset);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载资源失败: {assetName}，错误: {ex.Message}");
            }
        }
    }
}
