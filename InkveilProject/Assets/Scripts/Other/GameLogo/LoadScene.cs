using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private AsyncOperationHandle<SceneInstance> handle;
    private bool isReadyToActivate = false;

    void Start()
    {
        // 替换为 Addressables 场景名称（请替换为你实际的名称）
        string sceneName = "Main"; // e.g. "Main"

        // 开始异步加载，但不激活
        handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);
        handle.Completed += OnSceneLoadCompleted;
    }

    private void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> result)
    {
        if (result.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene loaded but not activated.");
            if (isReadyToActivate)
            {
                // 如果用户已调用过允许激活，则激活
                result.Result.ActivateAsync();
            }
        }
        else
        {
            Debug.LogError("Scene load failed: " + result.OperationException);
        }
    }

    public void AllowSceneActivation()
    {
        isReadyToActivate = true;
        if (handle.IsDone)
        {
            handle.Result.ActivateAsync();
        }
    }
}
