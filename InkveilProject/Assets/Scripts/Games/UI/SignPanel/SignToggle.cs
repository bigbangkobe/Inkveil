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

    public int mPropType; // 第几天，1~7

    public Action<int> OnClaimReward; // 从 SignPanel 注入的领奖回调

    private Toggle mToggle;

    /// <summary>
    /// 是否当前可点击
    /// </summary>
    public bool isActive;

    /// <summary>
    /// 是否已领取
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
    /// 设置是否可点击（当前天数是否激活）
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

        // 可选：调整图片颜色
        if (mPropImg != null)
        {
            mPropImg.color = isActive ? Color.white : Color.gray;
        }
    }

    /// <summary>
    /// 设置是否已领取
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

        // 可选：调整图片颜色
        if (mPropImg != null)
        {
            mPropImg.color = isGet ? Color.gray : (isActive ? Color.white : Color.gray);
        }
    }

    /// <summary>
    /// Toggle 切换（主要用于选中）
    /// </summary>
    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn && mToggle.interactable)
        {
            ClaimReward();
        }
    }

    /// <summary>
    /// 按钮点击领取
    /// </summary>
    private void OnClaimButtonClicked()
    {
        if (mToggle == null || !mToggle.interactable)
        {
            Debug.LogWarning($"第 {mPropType} 天 Toggle 不可交互，无法领取");
            return;
        }

        ClaimReward();
    }

    /// <summary>
    /// 调用 SignPanel 的奖励处理
    /// </summary>
    private void ClaimReward()
    {
        Debug.Log($"点击领取第 {mPropType} 天奖励");

        if (OnClaimReward != null)
        {
            OnClaimReward.Invoke(mPropType);
        }
        else
        {
            Debug.LogError("OnClaimReward 未绑定回调，请在 SignPanel 中绑定！");
        }
    }

    /// <summary>
    /// 可扩展：设置道具图片
    /// </summary>
    public void SetPropImage(Sprite sprite)
    {
        if (mPropImg != null && sprite != null)
        {
            mPropImg.sprite = sprite;
        }
    }
}
