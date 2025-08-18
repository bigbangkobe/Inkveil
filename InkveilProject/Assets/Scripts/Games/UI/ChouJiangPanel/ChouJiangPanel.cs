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
    /// ��������
    /// </summary>
    [SerializeField] JiangPanel mJiangPanel;

    /// <summary>
    /// �齱����
    /// </summary>
    [SerializeField] ChouPanel mChouPanel;

    /// <summary>
    /// ���鰴ť
    /// </summary>
    [SerializeField] Button mOneBtn;

    /// <summary>
    /// ʮ�鰴ť
    /// </summary>
    [SerializeField] Button mTenBtn;

    /// <summary>
    /// ��ʾ��ѳ齱����ʱ���ı�
    /// </summary>
    [SerializeField] Text mFreeDrawCountdownText;
    [SerializeField] Image mFreeDrawCountdownImage;

    public BagItemUI[] bagItemUIs;

    private const string LastFreeDrawKey = "LastFreeDrawTime";
    private const string TotalDrawCountKey = "TotalDrawCount";
    private Coroutine mCountdownCoroutine;

    /// <summary>
    /// ��ѳ齱�Ƿ����
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
    /// ִ�г鿨�߼�
    /// </summary>
    /// <param name="drawCount">�鿨����</param>
    /// <param name="isFree">�Ƿ���ѳ鿨</param>
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

            // �Ӷ�ӦƷ�ʳ������һ�ſ�
            if (LotteryDispositionManager.instance.LotteryCardPools.TryGetValue(cardGrade, out var cardPool) && cardPool.Count > 0)
            {
                int randomIndex = Random.Range(0, cardPool.Count);
                rewards.Add(cardPool[randomIndex]);
            }
        }

        // ��ʾ����
        ShowRewards(rewards, isFree);
    }

    /// <summary>
    /// ���ݸ��ʻ�ȡ�������Ʒ��
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

        // Ĭ�Ϸ������Ʒ��
        return 1;
    }

    /// <summary>
    /// ��ʾ��õĽ���
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
                        GodDispositionManager.instance.SetCurGod("����", PlayerPrefs.GetInt("����", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(3);
                        break;
                    case 5:
                        GodDispositionManager.instance.SetCurGod("���", PlayerPrefs.GetInt("���", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(4);
                        break;
                    case 7:
                        GodDispositionManager.instance.SetCurGod("���", PlayerPrefs.GetInt("���", 1));
                        GuideManager.instance.OnPlayRandomGuideByID(5);
                        break;
                    case 6:
                        GodDispositionManager.instance.SetCurGod("��߸", PlayerPrefs.GetInt("��߸", 1));
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

        // ���°�ť״̬
        Text btnText = mOneBtn.GetComponentInChildren<Text>();
        if (btnText != null)
        {
            btnText.text = IsFreeDrawAvailable ? "��ѵ���" : "����";
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
            // ��ѳ齱
            PerformDraw(1, true);
            PlayerPrefs.SetString(LastFreeDrawKey, DateTime.Now.ToString());
            PlayerPrefs.Save();
            IsFreeDrawAvailable = false;
            UpdateUI();
        }
        else
        {
            // ���ѵ���
            if (CheckCanDraw(1))
            {
                PerformDraw(1, false);
            }
        }
    }

    public void OnTenButtonClick()
    {
        // ����ʮ����
        if (CheckCanDraw(10))
        {
            PerformDraw(10, false);
        }
    }

    /// <summary>
    /// ����Ƿ���Գ鿨����Դ�Ƿ��㹻��
    /// </summary>
    private bool CheckCanDraw(int drawCount)
    {
        return BagManager.instance.UserItem((int)PropertyIDType.pleaseDivineOrder, drawCount);
    }
}