using System.Collections;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanelUI : BaseUI
{
    [SerializeField] private Image m_Fill;      // ������Image���
    [SerializeField] private Text m_Percentage; // �ٷֱ��ı�
    private void Start()
    {
        // ��ʼ����main����
        // �ӳټ���������
        StartCoroutine(DelayedLoadMainScene(2f));
        // ���ĳ��������¼�
        SceneLoaderManager.instance.OnLoadBegin += HandleLoadBegin;
        SceneLoaderManager.instance.OnLoadProgress += UpdateProgress;
        SceneLoaderManager.instance.OnLoadComplete += HandleLoadComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // ȡ�����ķ�ֹ�ڴ�й©
        SceneLoaderManager.instance.OnLoadBegin -= HandleLoadBegin;
        SceneLoaderManager.instance.OnLoadProgress -= UpdateProgress;
        SceneLoaderManager.instance.OnLoadComplete -= HandleLoadComplete;
    }


    private IEnumerator DelayedLoadMainScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadMainScene();
    }

    public void LoadMainScene()
    {
        // ȷ���������ѳ�ʼ��
        if (SceneLoaderManager.instance == null)
        {
            Debug.LogError("����������δ��ʼ����");
            return;
        }

        // ������������
        SceneLoaderManager.instance.LoadSceneDirect();
    }

    private void HandleLoadBegin()
    {
        // ���ؿ�ʼʱ�ĳ�ʼ��
        m_Fill.fillAmount = 0f;
        m_Percentage.text = "��Ϸ������0%";

        // ������������Ӷ���Ч��
        Debug.Log("�������ؿ�ʼ...");
    }

    private void UpdateProgress(float progress)
    {
        // ���°ٷֱ��ı� (0~100)
        int percentage = Mathf.RoundToInt(progress * 100);

        // ��ѡ�����ƽ������
        m_Fill.fillAmount = Mathf.Lerp(m_Fill.fillAmount, progress, Time.deltaTime * 5f);
        m_Percentage.text = $"��Ϸ������{(int)(m_Fill.fillAmount * 100)}%";
    }

    private void HandleLoadComplete()
    {
        // ������ɺ�Ĵ���
        m_Fill.fillAmount = 1f;
        m_Percentage.text = $"��Ϸ������100%";

        Debug.Log("����������ɣ�");
    }

    private IEnumerator DelayedClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        Close();
    }

    // �رս��淽��������BaseUIʵ�ֵ�����
    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}