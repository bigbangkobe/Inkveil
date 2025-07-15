using Framework;
using System.Collections;
using UnityEngine;

public class GetReadyPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject[] go321; // 顺序：[3, 2, 1, GO]

    private void Start()
    {
        // 初始隐藏所有元素
        foreach (GameObject obj in go321)
        {
            obj.SetActive(false);
        }
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // 显示3
        go321[0].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[0].SetActive(false);

        // 显示2
        go321[1].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[1].SetActive(false);

        // 显示1
        go321[2].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[2].SetActive(false);

        // 显示GO并开始游戏
        go321[3].SetActive(true);
       

        // 隐藏整个面板（可选）
        yield return new WaitForSeconds(0.5f);
        SoundSystem.instance.Play("CombatBG", 1, true, true);
        if (!GuideDispositionManager.instance.isGuide)
        {
            OnboardingGuidePanel.instance.StartGuide();
        }
        else
        {
            StrtGame();

        }
        gameObject.SetActive(false);
    }

    public void StrtGame() 
    {
        GameManager.instance.StartGame(); // 调用你的游戏开始方法
    }
}