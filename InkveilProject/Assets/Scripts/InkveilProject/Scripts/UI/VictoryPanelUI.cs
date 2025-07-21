using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;

public class VictoryPanelUI : MonoBehaviour
{
    private Button m_Btn;
    public RewardCard m_RewardCard;

    private void Start()
    {
        m_Btn = GetComponent<Button>();
        m_Btn.onClick.AddListener(OnClickHandler);
        OnboardingGuidePanel.instance.StartGuide();
        InitinlRewards();
    }

    private void InitinlRewards()
    {
        if (!GuideDispositionManager.instance.isGuide)
        {
            PropertyInfo propertyInfo1 = PropertyDispositionManager.instance.GetPropertyById(1);
            propertyInfo1.number = 1000;

            PropertyInfo propertyInfo2 = PropertyDispositionManager.instance.GetPropertyById(3);
            propertyInfo2.number = 10;

            RewardCard reward1 = Instantiate(m_RewardCard, m_RewardCard.transform.parent);
            reward1.OnInit(propertyInfo1);
            BagManager.instance.AddItem(propertyInfo1);

            RewardCard reward2 = Instantiate(m_RewardCard, m_RewardCard.transform.parent);
            reward2.OnInit(propertyInfo2);
            BagManager.instance.AddItem(propertyInfo2);
        }
        else
        {
            int level = LevelManager.instance.m_CurLevel;
            List<StageRewardsInfo> stageRewardsInfo = StageRewardsDispositionManager.instance.GetStageRewardsInfoByStageRewards(level);
            List<PropertyInfo> propertyInfos = stageRewardsInfo[LevelManager.instance.curGrade].GetRewardsPropertyInfos();

            for (int i = 0; i < propertyInfos.Count; i++)
            {
                RewardCard reward = Instantiate(m_RewardCard, m_RewardCard.transform.parent);
                reward.OnInit(propertyInfos[i]);
                BagManager.instance.AddItem(propertyInfos[i]);
            }

            StageRewardsDispositionManager.instance.PassLevel(level - 1);
            stageRewardsInfo[LevelManager.instance.curGrade].grade = 1;
            if (PlayerController.instance.HitPoints >= 0.5f) stageRewardsInfo[LevelManager.instance.curGrade].grade++;
            if (PlayerController.instance.HitPoints >= 1f) stageRewardsInfo[LevelManager.instance.curGrade].grade++;
            stageRewardsInfo[LevelManager.instance.curGrade].isPass = true;

            StageRewardsDispositionManager.instance.Save();
        }
    }

    private void OnClickHandler()
    {
        GameManager.instance.OnClear();

        // 判断是否为 BuildSettings 场景
        if (Application.CanStreamedLevelBeLoaded("Main"))
        {
            SceneManager.LoadSceneAsync("Main");
        }
        else
        {
            StartCoroutine(LoadMainSceneWithAddressables());
        }
    }

    private IEnumerator LoadMainSceneWithAddressables()
    {
        string targetScene = "Main";

        // 可选：预加载依赖资源
        var downloadHandle = Addressables.DownloadDependenciesAsync(targetScene);
        yield return downloadHandle;

        if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("依赖资源下载失败: " + targetScene);
            yield break;
        }

        var handle = Addressables.LoadSceneAsync(targetScene, LoadSceneMode.Single, false);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Addressables 加载场景失败: " + targetScene);
            yield break;
        }

        // 激活场景
        yield return handle.Result.ActivateAsync();
    }
}
