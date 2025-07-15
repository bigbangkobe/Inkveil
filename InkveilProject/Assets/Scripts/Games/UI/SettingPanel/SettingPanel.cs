using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingPanel : BaseUI
{
    [SerializeField] Button mBackBtn;
    [SerializeField] Slider m_BGSlider;
    [SerializeField] Slider m_SoundSlider;

    /// <summary>
    /// 初始化函数
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        mBackBtn.onClick.AddListener(OnBackButtonClick);
        m_BGSlider.onValueChanged.AddListener(OnBGVolumeChanged);
        m_SoundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
    }

    protected void OnBGVolumeChanged(float value) 
    {
        SoundSystem.instance.SetBgmVolume(value);
    }

    protected void OnSoundVolumeChanged(float value)
    {
        SoundSystem.instance.SetSfxVolume(value);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 显示界面函数
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
       
    }

    /// <summary>
    /// 隐藏界面函数
    /// </summary>
    protected override void OnHideDisable()
    {
      
        base.OnHideDisable();
    }

    private void OnBackButtonClick()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("Main"))
        {
            gameObject.SetActive(false);
        }
        else
        {
            SceneManager.LoadSceneAsync("Main");
        }
      
    }
}
