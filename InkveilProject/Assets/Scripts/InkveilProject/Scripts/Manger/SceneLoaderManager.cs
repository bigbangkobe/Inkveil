using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Framework;
using UnityEngine.AddressableAssets;

public class SceneLoaderManager : MonoSingleton<SceneLoaderManager>
{
   [Header("Settings")]
    [SerializeField] private float minLoadTime = 2f; // 最小加载时间（确保加载画面可见）
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("Events")]
    public Action OnLoadBegin;
    public Action<float> OnLoadProgress;
    public Action OnLoadComplete;

    private string targetScene = "Main";
    private bool isLoading;

    /// <summary>
    /// 切换到新场景（带加载界面）
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void SwitchToScene(string sceneName)
    {
        if (isLoading) return;

        targetScene = sceneName;
        LoadSceneDirect();
        //StartCoroutine(LoadWithLoadingScreen());
    }

    protected override void Awake()
    {
        base.Awake();

    }

    /// <summary>
    /// 直接加载场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void LoadSceneDirect()
    {
        if (isLoading) return;
        if (!GuideDispositionManager.instance.isGuide)
        {
            LevelManager.instance.SetCurLevel(0);
            targetScene = LevelManager.instance.m_CurLevelInfo.sceneName;
        }
        StartCoroutine(LoadSceneAsync(targetScene, true));
    }

    private IEnumerator LoadWithLoadingScreen()
    {
        isLoading = true;

        // 触发加载开始事件
        OnLoadBegin?.Invoke();

        // 加载过渡场景
        //yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(loadingSceneName);

        // 卸载当前场景（保留Loading场景）
        //Scene previousScene = SceneManager.GetActiveScene();
        //yield return SceneManager.UnloadSceneAsync(previousScene);

        // 加载目标场景
        //yield return StartCoroutine(LoadSceneAsync(targetScene, true));

        // 卸载加载场景
        //yield return SceneManager.UnloadSceneAsync(loadingSceneName);

        isLoading = false;
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool reportProgress)
    {
        float startTime = Time.realtimeSinceStartup;
        float minEndTime = startTime + minLoadTime;
        float progress = 0;

        while (Time.realtimeSinceStartup < minEndTime)
        {
            progress = (Time.realtimeSinceStartup - startTime) / minLoadTime * 0.2f;
            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        int mInLoadssum = 100;
        while (ResMgr.Instance.mWaitting.Count > 0)
        {
            progress = 0.2f + ((1 - ResMgr.Instance.mWaitting.Count / (float)mInLoadssum) * 0.3f);
            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // ✅ Addressables 加载场景
        var handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);
        while (!handle.IsDone)
        {
            progress = 0.5f + Mathf.Clamp01(handle.PercentComplete) * 0.5f;
            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // 手动激活场景
        yield return handle.Result.ActivateAsync();

        if (reportProgress) OnLoadProgress?.Invoke(1f);
        OnLoadComplete?.Invoke();
    }

}