using System.Collections;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanelUI : BaseUI
{
    [SerializeField] private Image m_Fill;      // 进度条Image组件
    [SerializeField] private Text m_Percentage; // 百分比文本
    private void Start()
    {
        // 开始加载main场景
        // 延迟加载主场景
        StartCoroutine(DelayedLoadMainScene(2f));
        // 订阅场景加载事件
        SceneLoaderManager.instance.OnLoadBegin += HandleLoadBegin;
        SceneLoaderManager.instance.OnLoadProgress += UpdateProgress;
        SceneLoaderManager.instance.OnLoadComplete += HandleLoadComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // 取消订阅防止内存泄漏
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
        // 确保加载器已初始化
        if (SceneLoaderManager.instance == null)
        {
            Debug.LogError("场景加载器未初始化！");
            return;
        }

        // 触发场景加载
        SceneLoaderManager.instance.LoadSceneDirect();
    }

    private void HandleLoadBegin()
    {
        // 加载开始时的初始化
        m_Fill.fillAmount = 0f;
        m_Percentage.text = "游戏加载中0%";

        // 可以在这里添加动画效果
        Debug.Log("场景加载开始...");
    }

    private void UpdateProgress(float progress)
    {
        // 更新百分比文本 (0~100)
        int percentage = Mathf.RoundToInt(progress * 100);

        // 可选：添加平滑动画
        m_Fill.fillAmount = Mathf.Lerp(m_Fill.fillAmount, progress, Time.deltaTime * 5f);
        m_Percentage.text = $"游戏加载中{(int)(m_Fill.fillAmount * 100)}%";
    }

    private void HandleLoadComplete()
    {
        // 加载完成后的处理
        m_Fill.fillAmount = 1f;
        m_Percentage.text = $"游戏加载中100%";

        Debug.Log("场景加载完成！");
    }

    private IEnumerator DelayedClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        Close();
    }

    // 关闭界面方法（根据BaseUI实现调整）
    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}