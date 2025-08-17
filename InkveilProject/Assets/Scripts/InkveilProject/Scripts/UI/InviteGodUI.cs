using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InviteGodUI : MonoBehaviour
{
    public Image m_Fill;                // 充能/持续时间进度条
    public Button m_InviteGodBtn;       // 召唤按钮
    public Button m_SummonBtn;       // 召唤按钮
    public Image m_SummonImage;       // 召唤按钮
    public Sprite[] summonSprites;
    public Transform m_SummonList;       // 召唤按钮
    public Button m_SummonBtn1;       // 召唤按钮
    public Button m_SummonBtn2;       // 召唤按钮
    public Button m_SummonBtn3;       // 召唤按钮
    public Button m_SummonBtn4;       // 召唤按钮

    [SerializeField] private Button m_DaZhao;

    private GodInfo godInfo;            // 当前神明信息
    private bool isGodActive = false;   // 神明是否在场
    private float chargeProgress = 0f;  // 充能进度(0-1)
    private float durationProgress = 1f; // 持续时间进度(1-0)
    private float chargeSpeed = 0f;
    private float durationSpeed = 0f;
    private GodBase curGodBase;
    public GuideHintPanelUI guideHintPanelUI;


    private void Awake()
    {
        godInfo = GodDispositionManager.instance.curGod;
        m_InviteGodBtn.onClick.AddListener(() => { OnInviteGodClickHandler(); });
        m_SummonBtn.onClick.AddListener(() => { m_SummonList.gameObject.SetActive(true); });
        m_SummonBtn1.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("杨戬", PlayerPrefs.GetInt("杨戬", 1)); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn2.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("关羽", PlayerPrefs.GetInt("关羽", 1)); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn3.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("悟空", PlayerPrefs.GetInt("悟空", 1)); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn4.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("哪吒", PlayerPrefs.GetInt("哪吒", 1)); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_DaZhao.onClick.AddListener(OnDaZhaoClickHandler);

        // 初始状态
        UpdateUI();
        m_InviteGodBtn.interactable = false;
    }

    private void OnEnable()
    {
        InitialSummon();
    }

    private void InitialSummon()
    {
        if (!GuideDispositionManager.instance.isGuide)
        {
            m_SummonBtn1.interactable = false;       // 召唤按钮
            m_SummonBtn2.interactable = false;      // 召唤按钮
            m_SummonBtn3.interactable = false;      // 召唤按钮
            m_SummonBtn4.interactable = false;

            return;
        }
        else
        {
            m_SummonBtn1.interactable = true;       // 召唤按钮
            m_SummonBtn2.interactable = true;      // 召唤按钮
            m_SummonBtn3.interactable = true;      // 召唤按钮
            m_SummonBtn4.interactable = true;
            switch (GodDispositionManager.instance.curGod.godName)
            {
                case "关羽":
                    m_SummonImage.sprite = summonSprites[1];
                    break;
                case "悟空":
                    m_SummonImage.sprite = summonSprites[2];
                    break;
                case "杨戬":
                    m_SummonImage.sprite = summonSprites[0];
                    break;
                case "哪吒":
                    m_SummonImage.sprite = summonSprites[3];
                    break;
                default:
                    m_SummonImage.sprite = summonSprites[3];
                    break;
            }
            godInfo = GodDispositionManager.instance.curGod;
            //return;
        }
        if (BagManager.instance.GetItem((int)PropertyIDType.sunWuKongHeroShard) != null)
        {
            //悟空碎片
            m_SummonBtn3.interactable = true;
        }
        else
        {
            m_SummonBtn3.interactable = false;
        }
        if (BagManager.instance.GetItem((int)PropertyIDType.neZhaHeroShard) != null)
        {
            //哪吒碎片
            m_SummonBtn4.interactable = true;
        }
        else
        {
            m_SummonBtn4.interactable = false;
        }
        if (BagManager.instance.GetItem((int)PropertyIDType.yangJianHeroShard) != null)
        {
            //杨戬碎片
            m_SummonBtn1.interactable = true;
        }
        else
        {
            m_SummonBtn1.interactable = false;
        }
        if (BagManager.instance.GetItem((int)PropertyIDType.guanYuHeroShard) != null)
        {
            //关羽碎片
            m_SummonBtn2.interactable = true;
        }
        else
        {
            m_SummonBtn2.interactable = false;
        }

        switch (GodDispositionManager.instance.curGod.godName)
        {
            case "关羽":
                m_SummonImage.sprite = summonSprites[1];
                break;
            case "悟空":
                m_SummonImage.sprite = summonSprites[2];
                break;
            case "杨戬":
                m_SummonImage.sprite = summonSprites[0];
                break;
            case "哪吒":
                m_SummonImage.sprite = summonSprites[3];
                break;
            default:
                m_SummonImage.sprite = summonSprites[3];
                break;
        }
        godInfo = GodDispositionManager.instance.curGod;
    }

    private void Update()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) return;
        if (!isGodActive)
        {
            // 充能阶段
            if (chargeProgress < 1f)
            {
                if (GuideDispositionManager.instance.isGuide) 
                {
                    chargeSpeed += Time.deltaTime;
                    chargeProgress = Mathf.Min(1f, chargeSpeed / godInfo.baseCooldown);
                }


                UpdateUI();

                // 充能完成时激活按钮
                if (chargeProgress >= 1f)
                {
                    m_InviteGodBtn.interactable = true;
                }
            }
        }
        else
        {
            // 神明在场时间递减
            if (durationProgress > 0f)
            {
                if (GuideDispositionManager.instance.isGuide) 
                {
                    durationSpeed += Time.deltaTime;
                    durationProgress = Mathf.Max(0f, 1 - durationSpeed / godInfo.baseDuration);
                } 
                UpdateUI();
            }
            else
            {
                // 时间结束，神明退场
                OnGodExit();
            }
        }
    }

    private void OnDaZhaoClickHandler()
    {
        if (!GuideDispositionManager.instance.isGuide)
        {
            guideHintPanelUI.gameObject.SetActive(false);
            m_DaZhao.gameObject.SetActive(false);
            TimerSystem.Start((x) => { OnGodExit(); }, false, 3);
        }
        curGodBase.UseSkill();

    }
    private async Task OnInviteGodClickHandler()
    {
        if (chargeProgress >= 1f && !isGodActive)
        {
            isGodActive = true;
            durationProgress = 1f;
            chargeProgress = 0f;
            m_InviteGodBtn.interactable = false;

            EffectObject effect = await EffectSystem.instance.GetEffect("Temporary explosion");
            effect.transform.position = GodManager.instance.GetInitPoint().position;
            effect.Play();
            await TimerSystem.Start(async (x) =>
            {
                // 召唤神明
                GameManager.instance.GameStateEnum = GameConfig.GameState.State.Pause;

                // 这里调用实际召唤逻辑
                curGodBase = await GodManager.instance.GetGodByName(godInfo.godName);
                curGodBase.Activate();
                SoundObject sound = null;
                switch (godInfo.godName)
                {
                    case "哪吒":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(7);
                        break;
                    case "杨戬":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(8);
                        break;
                    case "关羽":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(9);
                        break;
                    case "悟空":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(10);
                        break;
                    default:
                        break;
                }
                await Task.Delay(3000);
                GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play;

                curGodBase.OnIsEnergy += OnIsEnergyHandler;
                //if (sound != null) sound.onStopEvent += (sound) => { GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play; };
                UpdateUI();
            }, false, 1.5f);
        }
    }

    private void OnIsEnergyHandler(bool obj)
    {
        m_DaZhao.gameObject.SetActive(obj);
        if (!GuideDispositionManager.instance.isGuide && obj)
        {
            guideHintPanelUI.SetPoint(m_DaZhao.transform);
            GameManager.instance.GameStateEnum = GameConfig.GameState.State.Pause;
        }
    }

    private void OnGodExit()
    {
        isGodActive = false;
        durationProgress = 1f;
        chargeProgress = 0f;
        chargeSpeed = 0f;
        durationSpeed = 0f;

        // 这里调用神明退场逻辑
        curGodBase.Deactivate();
        curGodBase.OnIsEnergy -= OnIsEnergyHandler;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isGodActive)
        {
            // 显示剩余持续时间(从1到0)
            m_Fill.fillAmount = durationProgress;
            m_Fill.color = Color.yellow; // 在场时显示黄色
        }
        else
        {
            // 显示充能进度(从0到1)
            m_Fill.fillAmount = chargeProgress;
            m_Fill.color = Color.green; // 充能时显示绿色
        }
    }

    // 可以外部调用来加速充能(例如击杀敌人时)
    public void ff(float amount)
    {
        if (!isGodActive)
        {
            chargeProgress = Mathf.Min(1f, chargeProgress + amount);
            UpdateUI();

            if (chargeProgress >= 1f)
            {
                m_InviteGodBtn.interactable = true;
            }
        }
    }

    // 设置充能速度
    public void SetChargeSpeed(float speed)
    {
        chargeSpeed = speed;
    }

    // 设置持续时间速度
    public void SetDurationSpeed(float speed)
    {
        durationSpeed = speed;
    }
}