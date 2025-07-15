using Framework;
using System.Collections;
using UnityEngine;

public class GetReadyPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject[] go321; // ˳��[3, 2, 1, GO]

    private void Start()
    {
        // ��ʼ��������Ԫ��
        foreach (GameObject obj in go321)
        {
            obj.SetActive(false);
        }
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // ��ʾ3
        go321[0].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[0].SetActive(false);

        // ��ʾ2
        go321[1].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[1].SetActive(false);

        // ��ʾ1
        go321[2].SetActive(true);
        yield return new WaitForSeconds(1f);
        go321[2].SetActive(false);

        // ��ʾGO����ʼ��Ϸ
        go321[3].SetActive(true);
       

        // ����������壨��ѡ��
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
        GameManager.instance.StartGame(); // ���������Ϸ��ʼ����
    }
}