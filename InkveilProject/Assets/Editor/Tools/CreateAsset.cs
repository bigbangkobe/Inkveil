using UnityEngine;

using System.Collections;

using UnityEditor;

using System.IO;

public class CreateAsset : Editor
{

    public class PathConfig
    {
        /// <summary>
        /// 存放asset文件的文件夹路径
        /// </summary>
        public static readonly string assetPath = "Assets/Resources";
    }

}
