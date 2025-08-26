using Framework;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GuideDispositionManager : Singleton<GuideDispositionManager>
{
    private List<Guide> onboardingGuideList = new List<Guide>();
    private List<Guide> guideList = new List<Guide>();


    private bool m_IsInitialized = false;

    public bool isGuide;
    private int curonboardingGuideIndex = 0;

    public int CuronboardingGuideIndex { get => curonboardingGuideIndex;private set => curonboardingGuideIndex = value; }

    /// <summary>
    /// 初始化引导配置系统
    /// </summary>
    public async void OnInit()
    {
        if (m_IsInitialized) return;
        isGuide = PlayerPrefs.GetInt("Guide") == 1;
        string guide_info = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.guide_info)).text;
        string onboarding_guide_info = (await ResourceService.LoadAsync<TextAsset>(ConfigDefine.onboarding_guide_info)).text;
        guideList = JsonHelper.ToObject<List<Guide>>(guide_info);
        onboardingGuideList = JsonHelper.ToObject<List<Guide>>(onboarding_guide_info);
    }
    
    public void PassGuide() 
    {
        isGuide = true;
        PlayerPrefs.SetInt("Guide",1);
    }

    public Guide StartonboardingGuide() 
    {
        if (isGuide) return null;
        Guide guide = onboardingGuideList[CuronboardingGuideIndex++];
        if (CuronboardingGuideIndex == onboardingGuideList.Count) PassGuide();
        return guide;
    }

    internal List<Guide> GetGuidesByType(int type)
    {
        return guideList.FindAll(x => x.guideType == type);
    }

    internal Guide GetGuidesID(int id)
    {
        return guideList.Find(x => x.id == id);
    }
}