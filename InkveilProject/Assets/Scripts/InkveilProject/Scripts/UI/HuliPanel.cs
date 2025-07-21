using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HuliPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_huli;
    [SerializeField] private Text m_Text;
    [SerializeField] private Button m_huliBtn;
    private List<Guide> guides = new List<Guide>();

    private void Awake()
    {
        guides = GuideDispositionManager.instance.GetGuidesByType(1);
        m_huliBtn.onClick.AddListener(PlayGuide);
    }


    internal async void PlayGuide()
    {
        int index = Random.Range(0, guides.Count);
        Guide guide = guides[index];
        m_huliBtn.interactable = false;
        if (guide.isText == 1)
        {
            m_huli.gameObject.SetActive(true);
            m_Text.text = guide.guideText;
            SoundObject sound = await SoundSystem.instance.Play(guide.sound);
            sound.onStopEvent += OnSoundEndHandler;
        }
        else
        {
            SoundObject sound = await SoundSystem.instance.Play(guide.sound);
            sound.onStopEvent += OnSoundEndHandler;
        }
    }

    public void OnSoundEndHandler(SoundObject sound)
    {
        this.Invoke(nameof(OnEndHandler), 1);
    }

    public void OnEndHandler()
    {
        m_huliBtn.interactable = true;
        m_huli.gameObject.SetActive(false);
    }
}
