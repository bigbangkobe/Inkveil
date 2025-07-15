using System.Collections.Generic;
using Framework;
using LitJson;
using System.Threading.Tasks;
using UnityEngine;

public class GuideDispositionManager : Singleton<GuideDispositionManager>
{
    private List<Guide> onboardingGuideList = new List<Guide>();
    private List<Guide> guideList = new List<Guide>();

    private bool m_IsInitialized = false;
    public bool isGuide;
    private int curonboardingGuideIndex = 0;
    public int CuronboardingGuideIndex => curonboardingGuideIndex;

    /// <summary>
    /// 异步初始化引导配置系统
    /// </summary>
    public async Task OnInitAsync()
    {
        if (m_IsInitialized) return;

        // 判断是否已完成新手引导
        isGuide = PlayerPrefs.GetInt("Guide", 0) == 1;

        // 异步加载 JSON 配置
        var guideHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.guide_info);
        var onboardingHandle = ResourceService.LoadAsync<TextAsset>(ConfigDefine.onboarding_guide_info);

        TextAsset guideAsset = await guideHandle.Task;
        TextAsset onboardingAsset = await onboardingHandle.Task;

        if (guideAsset == null)
        {
            Debug.LogError($"引导配置加载失败: {ConfigDefine.guide_info}");
            return;
        }
        if (onboardingAsset == null)
        {
            Debug.LogError($"首次引导配置加载失败: {ConfigDefine.onboarding_guide_info}");
            return;
        }

        // 解析列表
        guideList = JsonMapper.ToObject<List<Guide>>(guideAsset.text) ?? new List<Guide>();
        onboardingGuideList = JsonMapper.ToObject<List<Guide>>(onboardingAsset.text) ?? new List<Guide>();

        m_IsInitialized = true;
        Debug.Log($"引导配置加载完成: 共{guideList.Count}条通用引导, {onboardingGuideList.Count}条首次引导");
    }

    /// <summary>
    /// 标记已通过新手引导
    /// </summary>
    public void PassGuide()
    {
        isGuide = true;
        PlayerPrefs.SetInt("Guide", 1);
    }

    /// <summary>
    /// 开始新手引导
    /// </summary>
    public Guide StartOnboardingGuide()
    {
        if (!m_IsInitialized || isGuide || onboardingGuideList.Count == 0)
            return null;

        var guide = onboardingGuideList[curonboardingGuideIndex++];
        if (curonboardingGuideIndex >= onboardingGuideList.Count)
            PassGuide();
        return guide;
    }

    /// <summary>
    /// 根据类型获取通用引导
    /// </summary>
    public List<Guide> GetGuidesByType(int type)
    {
        if (!m_IsInitialized) return new List<Guide>();
        return guideList.FindAll(x => x.guideType == type);
    }

    /// <summary>
    /// 根据ID获取通用引导
    /// </summary>
    public Guide GetGuideById(int id)
    {
        if (!m_IsInitialized) return null;
        return guideList.Find(x => x.id == id);
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Clear()
    {
        // 不持有 TextAsset 引用，若有则释放
        PlayerPrefs.Save();
        onboardingGuideList.Clear();
        guideList.Clear();
        m_IsInitialized = false;
    }
}
