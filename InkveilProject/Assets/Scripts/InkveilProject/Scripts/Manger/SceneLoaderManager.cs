using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using Framework;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

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
    private AsyncOperationHandle<SceneInstance> _sceneLoadHandle;

    /// <summary>
    /// 切换到新场景（带加载界面）
    /// </summary>
    public void SwitchToScene(string sceneName)
    {
        if (isLoading) return;

        targetScene = sceneName;
        LoadSceneDirect();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 直接加载场景
    /// </summary>
    public void LoadSceneDirect()
    {
        StartCoroutine(WaitLoadSceneDirect());
   
    }

    private IEnumerator WaitLoadSceneDirect()
    {
        yield return new WaitForSeconds(2);
        if (!GuideDispositionManager.instance.isGuide)
        {
            LevelManager.instance.SetCurLevel(0);
            targetScene = LevelManager.instance.m_CurLevelInfo.sceneName;
        }
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(targetScene, true));
        }
    }

    private IEnumerator LoadWithLoadingScreen()
    {
        isLoading = true;

        // 触发加载开始事件
        OnLoadBegin?.Invoke();

        // 加载过渡场景
        yield return SceneManager.LoadSceneAsync(loadingSceneName);

        // 加载目标场景
        yield return StartCoroutine(LoadSceneAsync(targetScene, true));

        isLoading = false;
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool reportProgress)
    {
        float startTime = Time.realtimeSinceStartup;
        float minEndTime = startTime + minLoadTime;
        float progress = 0f;

        // 第一阶段：最小加载时间等待
        while (Time.realtimeSinceStartup < minEndTime)
        {
            progress = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / minLoadTime) * 0.3f;
            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // 第二阶段：加载场景（使用Addressables）
        _sceneLoadHandle = Addressables.LoadSceneAsync(sceneName,
            LoadSceneMode.Single, false); // 不自动激活场景

        // 监控加载进度
        while (!_sceneLoadHandle.IsDone)
        {
            float sceneProgress = Mathf.Clamp01(_sceneLoadHandle.PercentComplete / 0.9f);
            progress = 0.3f + sceneProgress * 0.7f;

            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // 确保最小加载时间
        while (Time.realtimeSinceStartup < minEndTime)
        {
            yield return null;
        }

        // 激活场景
        if (_sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var sceneInstance = _sceneLoadHandle.Result;
            yield return sceneInstance.ActivateAsync();
        }

        // 完成加载
        progress = 1f;
        if (reportProgress) OnLoadProgress?.Invoke(progress);

        OnLoadComplete?.Invoke();

        // 释放场景句柄（可选）
        // Addressables.Release(_sceneLoadHandle);
    }

    /// <summary>
    /// 卸载当前场景
    /// </summary>
    public void UnloadCurrentScene()
    {
        if (_sceneLoadHandle.IsValid())
        {
            Addressables.UnloadSceneAsync(_sceneLoadHandle);
        }
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        SwitchToScene(GetCurrentSceneName());
    }

    private void OnDestroy()
    {
        // 确保释放资源
        if (_sceneLoadHandle.IsValid())
        {
            Addressables.Release(_sceneLoadHandle);
        }
    }
}