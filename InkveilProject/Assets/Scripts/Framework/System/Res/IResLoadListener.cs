using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 资源加载回调接口
    /// </summary>
    public interface IResLoadListener
    {
        /// <summary>
        /// 加载完成
        /// </summary>
        /// <param name="asset">资源</param>
        /// <param name="name">名称</param>
        void Finish(object asset,string name);

        /// <summary>
        /// 加载失败
        /// </summary>
        void Failure();

        ///// <summary>
        ///// 加载成功
        ///// </summary>
        ///// <param name="obj"></param>
        //void OnLoad(Object obj);
    }
}
