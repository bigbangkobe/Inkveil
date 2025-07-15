using Framework;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoSingleton<GuideManager>
{
    GuidePanel guidePanel;

    protected override void Awake()
    {
        base.Awake();

        guidePanel = ResourceService.Load<GameObject>("UI/GuidePanel").GetComponent<GuidePanel>();
        Instantiate(guidePanel.gameObject, transform);
    }

    public void OnInit() { }

    public void OnPlayRandomGuideByType(int type)
    {
        List<Guide> guides = GuideDispositionManager.instance.GetGuidesByType(type);
        int index = Random.Range(0, guides.Count);
        Guide guide = guides[index];

        guidePanel.SetGuide(guide);
    }

    public void OnPlayRandomGuideByID(int id)
    {
        Guide guide = GuideDispositionManager.instance.GetGuideById(id);

        guidePanel.SetGuide(guide);
    }
}