using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Framework.UIManager;

/// <summary>
/// 主UI面板
/// </summary>
public class MainPanel : BaseUI, ILoadUIListener
{
    [SerializeField]
    private TBButton m_GGTB;

    [SerializeField]
    private TBButton m_MHTB;

    [SerializeField]
    private TBButton m_XYTB;

    [SerializeField]
    private TBButton m_TLTB;

    [SerializeField]
    private Button m_SettingBtn;

    [SerializeField]
    private Button m_SCBtn;

    [SerializeField]
    private Button m_MRFLBtn;

    [SerializeField]
    private Button m_PHBtn;

    [SerializeField]
    private Button m_FXBtn;

    [SerializeField]
    private Toggle m_JSBtn;

    [SerializeField]
    private Toggle m_CJBtn;

    [SerializeField]
    private Toggle m_ZDBtn;

    [SerializeField]
    private Toggle m_BGBtn;

    [SerializeField]
    private Toggle m_QSBtn;

    [SerializeField]
    private GameObject m_LevelUI;


    public void Failure()
    {
        
    }

    public void FiniSh(BaseUI ui)
    {
        
    }

    // 初始化函数
    void Start()
    {
        LevelManager.instance.OnInit();
        //UIManager.Instance.CreateUI("GamePanel", typeof(BaseUI), this);
        //UIManager.Instance.ShowUI("GamePanel", typeof(BaseUI), this);
        SoundSystem.instance.Play("MainBG", 1, true, true);
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    protected override void OnInit()
    {
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        m_SettingBtn.onClick.AddListener(OnSettingButtonClick);
        m_SettingBtn.onClick.AddListener(OnSettingButtonClick);
        m_SCBtn.onClick.AddListener(OnSCButtonClick);
        m_MRFLBtn.onClick.AddListener(OnMRFLButtonClick);
        m_PHBtn.onClick.AddListener(OnPHButtonClick);
        m_FXBtn.onClick.AddListener(OnFXButtonClick);
        m_ZDBtn.onValueChanged.AddListener(OnZDChanagedHnadler);
    }

    private void OnZDChanagedHnadler(bool isOn)
    {
        if (isOn) 
        {
            m_LevelUI.gameObject.SetActive(true);

        }
    }

    /// <summary>
    /// 更新函数
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        m_SettingBtn.onClick.RemoveListener(OnSettingButtonClick);
        m_SCBtn.onClick.RemoveListener(OnSCButtonClick);
        m_MRFLBtn.onClick.RemoveListener(OnMRFLButtonClick);
        m_PHBtn.onClick.RemoveListener(OnPHButtonClick);
        m_FXBtn.onClick.RemoveListener(OnFXButtonClick);

    }

    /// <summary>
    /// 点击设置按钮事件
    /// </summary>
    private void OnSettingButtonClick()
    {
        print("点击设置按钮事件");
    }

    /// <summary>
    /// 点击商城按钮事件
    /// </summary>
    private void OnSCButtonClick()
    {
        print("点击商城按钮事件");
    }

    /// <summary>
    /// 点击每日福利按钮事件
    /// </summary>
    private void OnMRFLButtonClick()
    {
        print("点击每日福利按钮事件");
    }

    /// <summary>
    /// 点击排行 按钮事件
    /// </summary>
    private void OnPHButtonClick()
    {
        print("点击排行按钮事件");
    }

    /// <summary>
    /// 点击分享按钮事件
    /// </summary>
    private void OnFXButtonClick()
    {
        print("点击分享按钮事件");
    }

    [ContextMenu("清除")]
    public void Clear() 
    {
        PlayerPrefs.DeleteAll();
    }
}
