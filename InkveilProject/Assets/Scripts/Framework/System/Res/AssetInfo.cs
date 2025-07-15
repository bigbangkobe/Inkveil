namespace Framework
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// 资源
        /// </summary>
        public object asset;

        /// <summary>
        /// 是否常驻内存
        /// </summary>
        public bool isKeepInMemory;

        /// <summary>
        /// 资源堆栈数量
        /// </summary>
        public int stackCount = 0;
    }
}

