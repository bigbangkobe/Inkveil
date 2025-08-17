using Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanelUI : BaseUI
{
    [SerializeField] private Text m_LevelName;
    [SerializeField] private Image m_LevelIcon;
    [SerializeField] private Button m_RightBtn;
    [SerializeField] private Button m_LeftBtn;
    [SerializeField] private Toggle m_Level1;
    [SerializeField] private Toggle m_Level2;
    [SerializeField] private Toggle m_Level3;
    [SerializeField] private Button m_Awrod1;
    [SerializeField] private Button m_Awrod2;
    [SerializeField] private Button m_Awrod3;
    [SerializeField] private Button m_StartBtn;
    [SerializeField] private Button m_BackBtn;
    [SerializeField] private GameObject[] m_Awrods;
    [SerializeField] private Image m_Fill;

    private List<StageRewardsInfo> stageRewardsInfo;
    private int curLevel;
    private int curGrade = 0;

    private void Awake()
    {
        m_RightBtn.onClick.AddListener(OnRightHandler);
        m_LeftBtn.onClick.AddListener(OnLeftHandler);
        m_Level1.onValueChanged.AddListener((isOn) => { OnLevelHandler(isOn, 0); });
        m_Level2.onValueChanged.AddListener((isOn) => { OnLevelHandler(isOn, 1); });
        m_Level3.onValueChanged.AddListener((isOn) => { OnLevelHandler(isOn, 2); });
        m_Awrod1.onClick.AddListener(() => { OnAwrodHandler(0); });
        m_Awrod2.onClick.AddListener(() => { OnAwrodHandler(1); });
        m_Awrod3.onClick.AddListener(() => { OnAwrodHandler(2); });
        m_StartBtn.onClick.AddListener(OnStartHandler);
        m_BackBtn.onClick.AddListener(OnBackHandler);
    }

    private void OnBackHandler()
    {
        gameObject.SetActive(false);
    }

    private void OnStartHandler()
    {
        if (PlayerDispositionManager.instance.HasEnoughStaminaInit(5)) 
        {
            LevelManager.instance.SetCurLevel(curLevel + 1);
            LevelManager.instance.curGrade = curGrade;
            SceneLoaderManager.instance.SwitchToScene(LevelManager.instance.m_CurLevelInfo.sceneName);
        }
    }

    private void OnAwrodHandler(int index)
    {
        if (stageRewardsInfo[curGrade].receive[index] == 0)
        {
          
            List<PropertyInfo> propertyInfos = stageRewardsInfo[curGrade].GetSpecialAwardsPropertyInfos();
            BagManager.instance.AddItem(propertyInfos[index]);
            StageRewardsDispositionManager.instance.Save();
            stageRewardsInfo[curGrade].receive[index] = 1;
            InitialLevel();
            Debug.Log("ÁìÈ¡½±Àø");
        }
    }

    private void OnLevelHandler(bool arg0, int index)
    {
        if (arg0)
        {
            curGrade = index;
            InitialLevel();
        }
    }

    private void OnLeftHandler()
    {
        StageRewardsDispositionManager.instance.CurOpenLevel--;
        curGrade = 0;
        m_Level1.isOn = true;
        InitialLevel();
    }

    private void OnRightHandler()
    {
        StageRewardsDispositionManager.instance.CurOpenLevel++;
        InitialLevel();
    }

    private void OnEnable()
    {
        InitialLevel();
    }

    private void InitialLevel()
    {
        stageRewardsInfo = StageRewardsDispositionManager.instance.CurStageRewardsInfo;
        curLevel = StageRewardsDispositionManager.instance.CurOpenLevel;
        m_LeftBtn.interactable = curLevel > 0;
        m_RightBtn.interactable = curLevel < StageRewardsDispositionManager.instance.SumLevel - 1;
        m_StartBtn.interactable = true;// curLevel <= StageRewardsDispositionManager.instance.UnlockLevel;
      
        m_LevelName.text = stageRewardsInfo[curGrade].stageName;
        m_Level2.interactable = stageRewardsInfo[0].isPass;
        m_Level3.interactable = stageRewardsInfo[1].isPass;

        m_Awrod1.interactable = stageRewardsInfo[curGrade].grade > 0;
        m_Awrod2.interactable = stageRewardsInfo[curGrade].grade > 1;
        m_Awrod3.interactable = stageRewardsInfo[curGrade].grade > 2;

        if (stageRewardsInfo[curGrade].grade > 2)
        {
            m_Fill.fillAmount = 1;
        }
        else if (stageRewardsInfo[curGrade].grade > 1)
        {
            m_Fill.fillAmount = 0.5f;
        }
        else
        {
            m_Fill.fillAmount = 0;
        }
       
        for (int i = 0; i < m_Awrods.Length; i++)
        {
            m_Awrods[i].SetActive(stageRewardsInfo[curGrade].receive[i] == 1);
        }
    }
}
