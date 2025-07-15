using UnityEditor;
using UnityEngine;

public class SwitchPlatform : MonoBehaviour
{
    static void PlatFormCheck()
    {
        if (SystemInfo.operatingSystem.StartsWith("Windows"))
        {
            Debug.Log("Windows系统");



#if UNITY_ANDROID
            Debug.Log("安卓平台");
#else
                Debug.Log("其它平台");
                if (EditorUtility.DisplayDialog("切换平台", "检测到当前平台不是Android \n是否切换到Android平台", "yes", "no"))
                {
                    //切换平台到Android
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
                    Debug.Log("切换成功");
                }
#endif
        }
    }
}
