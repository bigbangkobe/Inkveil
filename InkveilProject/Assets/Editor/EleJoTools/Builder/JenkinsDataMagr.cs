using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class JenkinsDataMagr : MonoBehaviour
{
    //private static string apkPath = @"D:\Unity\UnityApk";
    //private static string apkName = "UnityDome.apk";
    
    [MenuItem("ELEJO/Jenkins/集成打包")]
    public static void BuildApk()
    {

        Dictionary<string, string> JenkinsData = new Dictionary<string, string>();
        Dictionary<string, string> ElejoBuildData = new Dictionary<string, string>();
        //if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        //{
        //    BuildTarget buildTarget = BuildTarget.Android;
        //    // 切换到 Android 平台
        //    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, buildTarget);
        //}
        JenkinsData = JenkinsBatData.JenkinsDataDic();

        FileStream file = File.Open("D:/Unity/UnityApk/ELEJO.log.txt", FileMode.OpenOrCreate);
        ////foreach (var item in JenkinsData)
        ////{
        ////    //file.Write(Encoding.UTF8.GetBytes(item.Key), 0, Encoding.UTF8.GetBytes(item.Key).Length);
        ////    Debug.Log(item.Key + item.Value);
        ////    //file.Write(Encoding.UTF8.GetBytes(":"), 0, Encoding.UTF8.GetBytes(":").Length);
        ////    //file.Write(Encoding.UTF8.GetBytes(item.Value), 0, Encoding.UTF8.GetBytes(item.Value).Length);
        ////    //file.Write(Encoding.UTF8.GetBytes("\n"), 0, Encoding.UTF8.GetBytes("\n").Length);

        ////}

        // 解析命令行参数
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (var s in args)
        {
            foreach (var item in JenkinsData)
            {
                if (s.Contains(item.Value))
                {
                    string data = s.Remove(0, item.Value.Length);
                    if (string.IsNullOrEmpty(data))
                        continue;
                    ElejoBuildData.Add(item.Value, data);
                    file.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
                    //if (data.Equals(""))
                    //{
                    //    file.Write(Encoding.UTF8.GetBytes("数据为空"), 0, Encoding.UTF8.GetBytes("数据为空").Length);
                    //}
                    //else
                    //{
                    //    file.Write(Encoding.UTF8.GetBytes("开始写入数据"), 0, Encoding.UTF8.GetBytes("开始写入数据").Length);
                    //}
                    

                    file.Write(Encoding.UTF8.GetBytes("\n"), 0, Encoding.UTF8.GetBytes("\n").Length);
                }

            }
        }
        file.Write(Encoding.UTF8.GetBytes("开始打包"), 0, Encoding.UTF8.GetBytes("开始打包").Length);

        file.Close();
        ///切换平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        
        PlayerBuilder.BuildPlayer(ElejoBuildData);


        
        //设置apk存放地址

        // 执行打包
        //BuildPlayerOptions opt = new BuildPlayerOptions();
        //string[] m_ScenePaths = new string[EditorBuildSettings.scenes.Length];

        //for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        //{
        //    m_ScenePaths[i] = EditorBuildSettings.scenes[i].path;
        //}

        //opt.scenes = m_ScenePaths;

        //opt.locationPathName = string.Format(@"{0}\{1}", apkPath, apkName);


        //opt.target = BuildTarget.Android;
        //opt.options = BuildOptions.None;

        //BuildPipeline.BuildPlayer(opt);

        //Debug.Log("Build App Done!");

    }


}
