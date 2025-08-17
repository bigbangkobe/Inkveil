using Framework;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class GuidePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_huli;
    [SerializeField] private Text m_Text;
    
    public event Action onStopEvent;

    private void Start()
    {
        m_huli.SetActive(false);
    }

    internal async Task<SoundObject> SetGuide(Guide guide)
    {
        if (guide.isText == 1)
        {
            m_huli.gameObject.SetActive(true);
            m_Text.text = guide.guideText;
            SoundObject sound = await SoundSystem.instance.Play(guide.sound);
            sound.onStopEvent += OnSoundEndHandler;
            return sound;
        }
        else
        {
            SoundObject sound = await SoundSystem.instance.Play(guide.sound);
            sound.onStopEvent += OnSoundEndHandler;
            return sound;
        }
    }

    public void OnSoundEndHandler(SoundObject sound)
    {
        onStopEvent?.Invoke();
        this.Invoke(nameof(OnEndHandler), 1);
    }

    public void OnEndHandler()
    {
        m_huli.gameObject.SetActive(false);
    }
}