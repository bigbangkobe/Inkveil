using System.Collections;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SettingPanel : BaseUI
{
    [SerializeField] Button mBackBtn;
    [SerializeField] Slider m_BGSlider;
    [SerializeField] Slider m_SoundSlider;

    /// <summary>
    /// 初始化函数
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        mBackBtn.onClick.AddListener(OnBackButtonClick);
        m_BGSlider.onValueChanged.AddListener(OnBGVolumeChanged);
        m_SoundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
    }

    protected override void OnShow(object param)
    {
        base.OnShow(param);
        GameManager.instance.GameStateEnum = GameConfig.GameState.State.Pause;
    }

    protected void OnBGVolumeChanged(float value)
    {
        SoundSystem.instance.SetBgmSound(value);
    }

    protected void OnSoundVolumeChanged(float value)
    {
        SoundSystem.instance.SetSfxSound(value);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 显示界面函数
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }

    /// <summary>
    /// 返回按钮点击事件（兼容 Addressables 和 BuildSettings 场景加载）
    /// </summary>
    private void OnBackButtonClick()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Equals("Main"))
        {
            gameObject.SetActive(false);
            GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play;
        }
        else
        {
            // 兼容两种加载方式
            if (Application.CanStreamedLevelBeLoaded("Main"))
            {
                GameManager.instance.OnClear();
                SceneManager.LoadSceneAsync("Main");
            }
            else
            {
                GameManager.instance.OnClear();
                StartCoroutine(LoadMainSceneWithAddressables());
            }
        }
    }

    /// <summary>
    /// 使用 Addressables 加载 Main 场景（异步）
    /// </summary>
    private IEnumerator LoadMainSceneWithAddressables()
    {
        string targetScene = "Main";

        // 可选：预加载依赖资源（如果远程资源或分包部署）
        var downloadHandle = Addressables.DownloadDependenciesAsync(targetScene);
        yield return downloadHandle;

        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("依赖资源下载失败！");
            yield break;
        }

        // 加载场景（不激活）
        var loadHandle = Addressables.LoadSceneAsync(targetScene, LoadSceneMode.Single, false);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Addressables 场景加载失败: {targetScene}");
            yield break;
        }

        // 激活场景
        yield return loadHandle.Result.ActivateAsync();
    }
}
