using Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// 手机Profiler信息提取到本地上
/// </summary>
public class ProfilerUtils : MonoSingleton<ProfilerUtils>
{

    public void BeginRecord()
    {
        if (!instance.GetComponent<InternalBehaviour>())
        {
            instance.gameObject.AddComponent<InternalBehaviour>();
        }
    }

    class InternalBehaviour : MonoBehaviour
    {
        private string m_DebugInfo = String.Empty;

        private void OnGUI()
        {
            GUILayout.Label(String.Format("<size=50>{0}</size>", m_DebugInfo));
        }

        IEnumerator Start()
        {

            for (int i = 5; i > 0; i--)
            {
                m_DebugInfo = string.Format("<color=blue>{0}s后开始保存Profiler日志</color>", i);
                yield return new WaitForSeconds(1);
            }
            string file = Application.persistentDataPath + "/profiler_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".log";
            Debug.Log(file);
            Profiler.logFile = file;
            Profiler.enabled = true;
            Profiler.enableBinaryLog = true;

            for (int i = 5; i > 0; i--)
            {
                m_DebugInfo = string.Format("<color=red>{0}s后结束保存Profiler日志</color>", i);
                yield return new WaitForSeconds(1);
            }

            Profiler.enableBinaryLog = false;
            m_DebugInfo = string.Format("保存完毕:{0}", file);
            yield return new WaitForSeconds(10);

            Destroy(this);
        }
    }

}