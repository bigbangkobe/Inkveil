using Framework;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SignToggle : BaseUI
{
    [SerializeField] private Image mPropImg;

    [SerializeField] private GameObject mMaskGo;

    [SerializeField] private GameObject mIsGetGo;

    [SerializeField] private Button mClaimButton;

    public int mPropType; // �ڼ��죬1~7

    public Action<int> OnClaimReward; // �� SignPanel ע����콱�ص�

    private Toggle mToggle;

    /// <summary>
    /// �Ƿ�ǰ�ɵ��
    /// </summary>
    public bool isActive;

    /// <summary>
    /// �Ƿ�����ȡ
    /// </summary>
    public bool isGet;

    public SignRewardInfo mSignRewardInfo;

    protected override void OnAwake()
    {
        base.OnAwake();
        mToggle = GetComponent<Toggle>();

        if (mToggle != null)
        {
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        if (mClaimButton != null)
        {
            mClaimButton.onClick.AddListener(OnClaimButtonClicked);
        }
    }

    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }


    /// <summary>
    /// �����Ƿ�ɵ������ǰ�����Ƿ񼤻
    /// </summary>
    public void SetActiveState(bool active)
    {
        isActive = active;

        if (mToggle != null)
        {
            mToggle.isOn = isActive;
            mToggle.onValueChanged.Invoke(isActive);
            mToggle.interactable = isActive && !isGet;
        }

        if (mMaskGo != null)
        {
            mMaskGo.SetActive(!isActive);
        }

        if (mClaimButton != null)
        {
            mClaimButton.interactable = isActive && !isGet;
        }

        // ��ѡ������ͼƬ��ɫ
        if (mPropImg != null)
        {
            mPropImg.color = isActive ? Color.white : Color.gray;
        }
    }

    /// <summary>
    /// �����Ƿ�����ȡ
    /// </summary>
    public void SetGetState(bool get)
    {
        isGet = get;

        if (mIsGetGo != null)
        {
            mIsGetGo.SetActive(isGet);
        }

        if (mToggle != null)
        {
            mToggle.interactable = isActive && !isGet;
        }

        if (mClaimButton != null)
        {
            mClaimButton.interactable = isActive && !isGet;
        }

        // ��ѡ������ͼƬ��ɫ
        if (mPropImg != null)
        {
            mPropImg.color = isGet ? Color.gray : (isActive ? Color.white : Color.gray);
        }
    }

    /// <summary>
    /// Toggle �л�����Ҫ����ѡ�У�
    /// </summary>
    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn && mToggle.interactable)
        {
            ClaimReward();
        }
    }

    /// <summary>
    /// ��ť�����ȡ
    /// </summary>
    private void OnClaimButtonClicked()
    {
        if (mToggle == null || !mToggle.interactable)
        {
            Debug.LogWarning($"�� {mPropType} �� Toggle ���ɽ������޷���ȡ");
            return;
        }

        ClaimReward();
    }

    /// <summary>
    /// ���� SignPanel �Ľ�������
    /// </summary>
    private void ClaimReward()
    {
        Debug.Log($"�����ȡ�� {mPropType} �콱��");

        if (OnClaimReward != null)
        {
            OnClaimReward.Invoke(mPropType);
        }
        else
        {
            Debug.LogError("OnClaimReward δ�󶨻ص������� SignPanel �а󶨣�");
        }
    }

    /// <summary>
    /// ����չ�����õ���ͼƬ
    /// </summary>
    public void SetPropImage(Sprite sprite)
    {
        if (mPropImg != null && sprite != null)
        {
            mPropImg.sprite = sprite;
        }
    }
}
