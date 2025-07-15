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
    [SerializeField] private float minLoadTime = 2f; // ��С����ʱ�䣨ȷ�����ػ���ɼ���
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("Events")]
    public Action OnLoadBegin;
    public Action<float> OnLoadProgress;
    public Action OnLoadComplete;

    private string targetScene = "Main";
    private bool isLoading;
    private AsyncOperationHandle<SceneInstance> _sceneLoadHandle;

    /// <summary>
    /// �л����³����������ؽ��棩
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
    /// ֱ�Ӽ��س���
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

        // �������ؿ�ʼ�¼�
        OnLoadBegin?.Invoke();

        // ���ع��ɳ���
        yield return SceneManager.LoadSceneAsync(loadingSceneName);

        // ����Ŀ�곡��
        yield return StartCoroutine(LoadSceneAsync(targetScene, true));

        isLoading = false;
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool reportProgress)
    {
        float startTime = Time.realtimeSinceStartup;
        float minEndTime = startTime + minLoadTime;
        float progress = 0f;

        // ��һ�׶Σ���С����ʱ��ȴ�
        while (Time.realtimeSinceStartup < minEndTime)
        {
            progress = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / minLoadTime) * 0.3f;
            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // �ڶ��׶Σ����س�����ʹ��Addressables��
        _sceneLoadHandle = Addressables.LoadSceneAsync(sceneName,
            LoadSceneMode.Single, false); // ���Զ������

        // ��ؼ��ؽ���
        while (!_sceneLoadHandle.IsDone)
        {
            float sceneProgress = Mathf.Clamp01(_sceneLoadHandle.PercentComplete / 0.9f);
            progress = 0.3f + sceneProgress * 0.7f;

            if (reportProgress) OnLoadProgress?.Invoke(progress);
            yield return null;
        }

        // ȷ����С����ʱ��
        while (Time.realtimeSinceStartup < minEndTime)
        {
            yield return null;
        }

        // �����
        if (_sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var sceneInstance = _sceneLoadHandle.Result;
            yield return sceneInstance.ActivateAsync();
        }

        // ��ɼ���
        progress = 1f;
        if (reportProgress) OnLoadProgress?.Invoke(progress);

        OnLoadComplete?.Invoke();

        // �ͷų����������ѡ��
        // Addressables.Release(_sceneLoadHandle);
    }

    /// <summary>
    /// ж�ص�ǰ����
    /// </summary>
    public void UnloadCurrentScene()
    {
        if (_sceneLoadHandle.IsValid())
        {
            Addressables.UnloadSceneAsync(_sceneLoadHandle);
        }
    }

    /// <summary>
    /// ��ȡ��ǰ��������
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// ���¼��ص�ǰ����
    /// </summary>
    public void ReloadCurrentScene()
    {
        SwitchToScene(GetCurrentSceneName());
    }

    private void OnDestroy()
    {
        // ȷ���ͷ���Դ
        if (_sceneLoadHandle.IsValid())
        {
            Addressables.Release(_sceneLoadHandle);
        }
    }
}