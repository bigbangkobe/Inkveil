using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InviteGodUI : MonoBehaviour
{
    public Image m_Fill;                // ����/����ʱ�������
    public Button m_InviteGodBtn;       // �ٻ���ť
    public Button m_SummonBtn;       // �ٻ���ť
    public Sprite[] summonSprites;
    public Transform m_SummonList;       // �ٻ���ť
    public Button m_SummonBtn1;       // �ٻ���ť
    public Button m_SummonBtn2;       // �ٻ���ť
    public Button m_SummonBtn3;       // �ٻ���ť
    public Button m_SummonBtn4;       // �ٻ���ť

    [SerializeField] private Button m_DaZhao;

    private GodInfo godInfo;            // ��ǰ������Ϣ
    private bool isGodActive = false;   // �����Ƿ��ڳ�
    private float chargeProgress = 0f;  // ���ܽ���(0-1)
    private float durationProgress = 1f; // ����ʱ�����(1-0)
    private float chargeSpeed = 0f;
    private float durationSpeed = 0f;
    private GodBase curGodBase;
    public GuideHintPanelUI guideHintPanelUI;


    private void Awake()
    {
        godInfo = GodDispositionManager.instance.curGod;
        m_InviteGodBtn.onClick.AddListener(() => { OnInviteGodClickHandler(); });
        m_SummonBtn.onClick.AddListener(() => { m_SummonList.gameObject.SetActive(true); });
        m_SummonBtn1.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("���"); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn2.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("����"); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn3.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("���"); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_SummonBtn4.onClick.AddListener(() => { GodDispositionManager.instance.SetCurGod("��߸"); InitialSummon(); m_SummonList.gameObject.SetActive(false); });
        m_DaZhao.onClick.AddListener(OnDaZhaoClickHandler);

        // ��ʼ״̬
        UpdateUI();
        m_InviteGodBtn.interactable = false;
    }

    private void OnEnable()
    {
        InitialSummon();
    }

    private void InitialSummon()
    {
        switch (GodDispositionManager.instance.curGod.godName)
        {
            case "����":
                m_SummonBtn.image.sprite = summonSprites[1];
                break;
            case "���":
                m_SummonBtn.image.sprite = summonSprites[2];
                break;
            case "���":
                m_SummonBtn.image.sprite = summonSprites[0];
                break;
            case "��߸":
                m_SummonBtn.image.sprite = summonSprites[3];
                break;
            default:
                m_SummonBtn.image.sprite = summonSprites[3];
                break;
        }
        godInfo = GodDispositionManager.instance.curGod;
    }

    private void Update()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) return;
        if (!isGodActive)
        {
            // ���ܽ׶�
            if (chargeProgress < 1f)
            {
                chargeSpeed += Time.deltaTime;

                chargeProgress = Mathf.Min(1f, chargeSpeed / godInfo.baseCooldown);
                UpdateUI();

                // �������ʱ���ť
                if (chargeProgress >= 1f)
                {
                    m_InviteGodBtn.interactable = true;
                }
            }
        }
        else
        {
            // �����ڳ�ʱ��ݼ�
            if (durationProgress > 0f)
            {
                durationSpeed += Time.deltaTime;

                durationProgress = Mathf.Max(0f, 1 - durationSpeed / godInfo.baseDuration);
                UpdateUI();
            }
            else
            {
                // ʱ������������˳�
                OnGodExit();
            }
        }
    }

    private void OnDaZhaoClickHandler()
    {
        if (!GuideDispositionManager.instance.isGuide)
        {
            guideHintPanelUI.gameObject.SetActive(false);
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
                // �ٻ�����
                GameManager.instance.GameStateEnum = GameConfig.GameState.State.Pause;

                // �������ʵ���ٻ��߼�
                curGodBase = await GodManager.instance.GetGodByName(godInfo.godName);
                curGodBase.Activate();
                SoundObject sound = null;
                switch (godInfo.godName)
                {
                    case "��߸":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(7);
                        break;
                    case "���":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(8);
                        break;
                    case "����":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(9);
                        break;
                    case "���":
                        sound = GuideManager.instance.OnPlayRandomGuideByID(10);
                        break;
                    default:
                        break;
                }
                curGodBase.OnIsEnergy += OnIsEnergyHandler;
               if(sound != null) sound.onStopEvent += (sound) => { GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play; };
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

        // ������������˳��߼�
        curGodBase.Deactivate();
        curGodBase.OnIsEnergy -= OnIsEnergyHandler;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isGodActive)
        {
            // ��ʾʣ�����ʱ��(��1��0)
            m_Fill.fillAmount = durationProgress;
            m_Fill.color = Color.yellow; // �ڳ�ʱ��ʾ��ɫ
        }
        else
        {
            // ��ʾ���ܽ���(��0��1)
            m_Fill.fillAmount = chargeProgress;
            m_Fill.color = Color.green; // ����ʱ��ʾ��ɫ
        }
    }

    // �����ⲿ���������ٳ���(�����ɱ����ʱ)
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

    // ���ó����ٶ�
    public void SetChargeSpeed(float speed)
    {
        chargeSpeed = speed;
    }

    // ���ó���ʱ���ٶ�
    public void SetDurationSpeed(float speed)
    {
        durationSpeed = speed;
    }
}