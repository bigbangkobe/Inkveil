using UnityEngine;
using UnityEngine.UI;
using Framework;
using UnityEngine.Events;
using System;
using DG.Tweening;
using System.Threading.Tasks;

public class OnboardingGuidePanel : MonoBehaviour
{
    [Serializable]
    public class GuideEvent 
    {
        public UnityEvent onStart;
        public UnityEvent onEnd;
    }
    public GameObject m_target;
    public GameObject m_huli;
    public Button m_huliBtn;
    public Text m_Text;

    private int index = 0;

    public GuideEvent[] onEvent;

    public static OnboardingGuidePanel instance;

    public Image m_Image;
    public bool isLinding = false;

    async void Start()
    {
        m_Image = GetComponent<Image>();

         instance = this;
        if (!GuideDispositionManager.instance.isGuide)
        {
            if (isLinding)
            {
                await TimerSystem.Start((x) => {
                    m_Image.DOColor(Color.black * 0, 2).OnComplete(async () => {
                        StartGuide();
                        m_Image.enabled = false;
                    });
                },false,1);
              
            }
            else
            {
                StartGuide();
            }
          
        }
        else
        {
            m_Image.enabled = false;
            m_target.gameObject.SetActive(true);
        }
    }

    public void StartGuide(int index) 
    {
        if (this.index== index)
        {
            StartGuide();
        }
    }

    public async void StartGuide()
    {
        if (GuideDispositionManager.instance.isGuide) return;
        Guide guide = GuideDispositionManager.instance.StartonboardingGuide();

        if (guide == null) return;
        if (onEvent != null && index < onEvent.Length)
        {
            onEvent[index].onStart?.Invoke();
        }

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

    public void WaitNexHandler(float timer) 
    {
        Invoke(nameof(OnNextHandler), timer);
    }

    private void OnNextHandler()
    {
        StartGuide();
    }

    public void OnSoundEndHandler(SoundObject sound)
    {
        this.Invoke(nameof(OnEndHandler), 0.5f);
    }

    public void OnEndHandler()
    {
        if (onEvent != null && index < onEvent.Length)
        {
            onEvent[index].onEnd?.Invoke();
        }

        m_huliBtn.interactable = true;
        index++;
    }
}