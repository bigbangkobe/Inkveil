using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChouJiangPanel : BaseUI
{
    /// <summary>
    /// 奖励界面
    /// </summary>
    [SerializeField] JiangPanel mJiangPanel;

    /// <summary>
    /// 抽奖界面
    /// </summary>
    [SerializeField] ChouPanel mChouPanel;

    /// <summary>
    /// 单抽按钮
    /// </summary>
    [SerializeField] Button mOneBtn;

    /// <summary>
    /// 十抽按钮
    /// </summary>
    [SerializeField] Button mTenBtn;

    /// <summary>
    /// 显示免费抽奖倒计时的文本
    /// </summary>
    [SerializeField] Text mFreeDrawCountdownText;
    [SerializeField] Image mFreeDrawCountdownImage;

    public BagItemUI[] bagItemUIs;

    private const string LastFreeDrawKey = "LastFreeDrawTime";
    private const string TotalDrawCountKey = "TotalDrawCount";
    private Coroutine mCountdownCoroutine;

    /// <summary>
    /// 免费抽奖是否可用
    /// </summary>
    public bool IsFreeDrawAvailable { get; private set; }

    private int totalDrawCount = 39;

    protected override void OnInit()
    {
        base.OnInit();
        mOneBtn.onClick.AddListener(OnOneButtonClick);
        mTenBtn.onClick.AddListener(OnTenButtonClick);
        CheckFreeDrawStatus();
        totalDrawCount = PlayerPrefs.GetInt(TotalDrawCountKey, 0);
    }

    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        CheckFreeDrawStatus();
        UpdateUI();

        if (mCountdownCoroutine != null)
        {
            StopCoroutine(mCountdownCoroutine);
        }
        mCountdownCoroutine = StartCoroutine(UpdateCountdownCoroutine());
    }

    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        if (mCountdownCoroutine != null)
        {
            StopCoroutine(mCountdownCoroutine);
            mCountdownCoroutine = null;
        }
        PlayerPrefs.SetInt(TotalDrawCountKey, totalDrawCount);
    }

    /// <summary>
    /// 执行抽卡逻辑
    /// </summary>
    /// <param name="drawCount">抽卡次数</param>
    /// <param name="isFree">是否免费抽卡</param>
    private void PerformDraw(int drawCount, bool isFree)
    {
        List<LotteryCardPools> rewards = new List<LotteryCardPools>();

        for (int i = 0; i < drawCount; i++)
        {
            totalDrawCount++;
            int cardGrade;

            // Check for guaranteed drops
            if (totalDrawCount % 40 == 0)
            {
                // Every 40th draw guarantees grade 3
                cardGrade = 3;
            }
            else if (totalDrawCount % 10 == 0)
            {
                // Every 10th draw guarantees grade 2
                cardGrade = 2;
            }
            else
            {
                // Normal random draw
                cardGrade = GetRandomCardGrade();
            }

            // 从对应品质池中随机一张卡
            if (LotteryDispositionManager.instance.LotteryCardPools.TryGetValue(cardGrade, out var cardPool) && cardPool.Count > 0)
            {
                int randomIndex = Random.Range(0, cardPool.Count);
                rewards.Add(cardPool[randomIndex]);
            }
        }

        // 显示奖励
        ShowRewards(rewards, isFree);
    }

    /// <summary>
    /// 根据概率获取随机卡牌品质
    /// </summary>
    private int GetRandomCardGrade()
    {
        float randomValue = Random.Range(0f, 1f);
        double cumulativeProbability = 0f;

        foreach (var probabilityInfo in LotteryDispositionManager.instance.LotteryList)
        {
            cumulativeProbability += probabilityInfo.probability;
            if (randomValue <= cumulativeProbability)
            {
                return probabilityInfo.lotteryGrade;
            }
        }

        // 默认返回最低品质
        return 1;
    }

    /// <summary>
    /// 显示获得的奖励
    /// </summary>
    private void ShowRewards(List<LotteryCardPools> rewards, bool isFree)
    {
        mJiangPanel.gameObject.SetActive(true);
        mChouPanel.gameObject.SetActive(false);

        for (int i = 0; i < bagItemUIs.Length; i++)
        {
            bagItemUIs[i].gameObject.SetActive(i < rewards.Count);

            if (i < rewards.Count)
            {
                PropertyInfo info = PropertyDispositionManager.instance.GetPropertyById(rewards[i].propertyID);
                info.number = rewards[i].number;
                BagItemInfo bagItemInfo = new BagItemInfo()
                {
                    propertyInfo = new PropertyInfo(info.propertyID, info.propertyName, info.propertyDes, info.propertyGrade, info.imagePath, info.number),
                    isLock = false,
                    isNew = true
                };
                bagItemUIs[i].OnInit(bagItemInfo);
                BagManager.instance.AddItem(bagItemInfo);

                switch (rewards[i].propertyID)
                {
                    case 8:
                        GodDispositionManager.instance.SetCurGod("关羽", PlayerPrefs.GetInt("关羽", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(3);
                        break;
                    case 5:
                        GodDispositionManager.instance.SetCurGod("悟空", PlayerPrefs.GetInt("悟空", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(4);
                        break;
                    case 7:
                        GodDispositionManager.instance.SetCurGod("杨戬", PlayerPrefs.GetInt("杨戬", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(5);
                        break;
                    case 6:
                        GodDispositionManager.instance.SetCurGod("哪吒", PlayerPrefs.GetInt("哪吒", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(6);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CheckFreeDrawStatus()
    {
        string lastDrawTimeStr = PlayerPrefs.GetString(LastFreeDrawKey, string.Empty);

        if (string.IsNullOrEmpty(lastDrawTimeStr))
        {
            IsFreeDrawAvailable = true;
            return;
        }

        DateTime lastDrawTime = DateTime.Parse(lastDrawTimeStr);
        DateTime now = DateTime.Now;

        IsFreeDrawAvailable = (now - lastDrawTime).TotalHours >= 24;
    }

    private void UpdateUI()
    {
        mFreeDrawCountdownText.gameObject.SetActive(!IsFreeDrawAvailable);
        mFreeDrawCountdownImage.gameObject.SetActive(IsFreeDrawAvailable);

        // 更新按钮状态
        Text btnText = mOneBtn.GetComponentInChildren<Text>();
        if (btnText != null)
        {
            btnText.text = IsFreeDrawAvailable ? "免费单抽" : "单抽";
        }
    }

    private IEnumerator UpdateCountdownCoroutine()
    {
        while (true)
        {
            UpdateCountdownText();
            yield return new WaitForSeconds(1);
        }
    }

    private void UpdateCountdownText()
    {
        if (IsFreeDrawAvailable || mFreeDrawCountdownText == null) return;

        string lastDrawTimeStr = PlayerPrefs.GetString(LastFreeDrawKey);
        if (string.IsNullOrEmpty(lastDrawTimeStr)) return;

        DateTime lastDrawTime = DateTime.Parse(lastDrawTimeStr);
        DateTime nextFreeTime = lastDrawTime.AddHours(24);
        TimeSpan remainingTime = nextFreeTime - DateTime.Now;

        if (remainingTime.TotalSeconds > 0)
        {
            mFreeDrawCountdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                remainingTime.Hours,
                remainingTime.Minutes,
                remainingTime.Seconds);
        }
        else
        {
            IsFreeDrawAvailable = true;
            UpdateUI();
        }
    }

    public void OnOneButtonClick()
    {
        if (IsFreeDrawAvailable)
        {
            // 免费抽奖
            PerformDraw(1, true);
            PlayerPrefs.SetString(LastFreeDrawKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
            IsFreeDrawAvailable = false;
            UpdateUI();
        }
        else
        {
            // 付费单抽
            if (CheckCanDraw(1))
            {
                PerformDraw(1, false);
            }
        }
    }

    public void OnTenButtonClick()
    {
        // 付费十连抽
        if (CheckCanDraw(10))
        {
            PerformDraw(10, false);
        }
    }

    /// <summary>
    /// 检查是否可以抽卡（资源是否足够）
    /// </summary>
    private bool CheckCanDraw(int drawCount)
    {
        return BagManager.instance.UserItem((int)PropertyIDType.pleaseDivineOrder, drawCount);
    }
}