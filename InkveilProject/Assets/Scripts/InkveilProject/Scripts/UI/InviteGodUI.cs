using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteGodUI : MonoBehaviour
{
    public Image m_Fill;                // ����/����ʱ�������
    public Button m_InviteGodBtn;       // �ٻ���ť

    [SerializeField] private Button m_DaZhao;

    private GodInfo godInfo;            // ��ǰ������Ϣ
    private bool isGodActive = false;   // �����Ƿ��ڳ�
    private float chargeProgress = 0f;  // ���ܽ���(0-1)
    private float durationProgress = 1f; // ����ʱ�����(1-0)
    private float chargeSpeed = 0f;   
    private float durationSpeed = 0f; 
    private GodBase curGodBase;


    private void Awake()
    {
        godInfo = GodDispositionManager.instance.curGod;
        m_InviteGodBtn.onClick.AddListener(OnInviteGodClickHandler);
        m_DaZhao.onClick.AddListener(OnDaZhaoClickHandler);

        // ��ʼ״̬
        UpdateUI();
        m_InviteGodBtn.interactable = false;
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

                durationProgress = Mathf.Max(0f,1 - durationSpeed / godInfo.baseDuration);
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
        curGodBase.UseSkill();
    }
    private void OnInviteGodClickHandler()
    {
        if (chargeProgress >= 1f && !isGodActive)
        {
            // �ٻ�����
            isGodActive = true;
            durationProgress = 1f;
            chargeProgress = 0f;
            m_InviteGodBtn.interactable = false;

            // �������ʵ���ٻ��߼�
            curGodBase = GodManager.instance.GetGodByName(godInfo.godName);
            curGodBase.Activate();

            switch (godInfo.godName)
            {
                case "��߸":
                    GuideManager.instance.OnPlayRandomGuideByID(7);
                    break;
                case "���":
                    GuideManager.instance.OnPlayRandomGuideByID(8);
                    break;
                case "����":
                    GuideManager.instance.OnPlayRandomGuideByID(9);
                    break;
                case "���":
                    GuideManager.instance.OnPlayRandomGuideByID(10);
                    break;
                default:
                    break;
            }
            curGodBase.OnIsEnergy += OnIsEnergyHandler;
                
            UpdateUI();
        }
    }

    private void OnIsEnergyHandler(bool obj)
    {
        m_DaZhao.gameObject.SetActive(obj);
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