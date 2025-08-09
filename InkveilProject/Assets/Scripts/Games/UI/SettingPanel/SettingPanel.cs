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
    /// ��ʼ������
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
    /// ��ʾ���溯��
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }

    /// <summary>
    /// ���ذ�ť����¼������� Addressables �� BuildSettings �������أ�
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
            // �������ּ��ط�ʽ
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
    /// ʹ�� Addressables ���� Main �������첽��
    /// </summary>
    private IEnumerator LoadMainSceneWithAddressables()
    {
        string targetScene = "Main";

        // ��ѡ��Ԥ����������Դ�����Զ����Դ��ְ�����
        var downloadHandle = Addressables.DownloadDependenciesAsync(targetScene);
        yield return downloadHandle;

        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("������Դ����ʧ�ܣ�");
            yield break;
        }

        // ���س����������
        var loadHandle = Addressables.LoadSceneAsync(targetScene, LoadSceneMode.Single, false);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Addressables ��������ʧ��: {targetScene}");
            yield break;
        }

        // �����
        yield return loadHandle.Result.ActivateAsync();
    }
}
