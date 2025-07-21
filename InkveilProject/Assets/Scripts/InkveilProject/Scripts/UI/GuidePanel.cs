using Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
public class GuidePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_huli;
    [SerializeField] private Text m_Text;

    internal async void SetGuide(Guide guide)
    {
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
        m_huli.gameObject.SetActive(false);
    }
}