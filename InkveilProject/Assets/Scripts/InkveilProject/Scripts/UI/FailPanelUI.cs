using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class FailPanelUI : MonoBehaviour
{
    private Button m_Btn;

    private void Start()
    {
        m_Btn = GetComponent<Button>();
        m_Btn.onClick.AddListener(OnClickHandler);

        GuideManager.instance.OnPlayRandomGuideByType(2);
    }

    private void OnClickHandler()
    {
        GameManager.instance.OnClear();

        if (Application.CanStreamedLevelBeLoaded("Main"))
        {
            // ✅ Build Settings 中存在 UI 场景时走此流程
            SceneManager.LoadSceneAsync("Main");
        }
        else
        {
            // ✅ Addressables 加载场景
            StartCoroutine(LoadUISceneFromAddressables());
        }
    }

    private IEnumerator LoadUISceneFromAddressables()
    {
        string targetScene = "Main";

        // Step 1: 预加载依赖（如果有远程依赖资源）
        var downloadHandle = Addressables.DownloadDependenciesAsync(targetScene);
        yield return downloadHandle;

        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("依赖资源下载失败: " + targetScene);
            yield break;
        }

        // Step 2: 加载场景（不激活）
        var handle = Addressables.LoadSceneAsync(targetScene, LoadSceneMode.Single, false);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Addressables 加载场景失败: " + targetScene);
            yield break;
        }

        // Step 3: 激活场景
        yield return handle.Result.ActivateAsync();
    }
}
