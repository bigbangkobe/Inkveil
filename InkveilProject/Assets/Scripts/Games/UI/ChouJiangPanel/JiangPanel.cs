using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JiangPanel : BaseUI
{
    public RewardCard mRewardCardPrefab;

    /// <summary>
    /// ȫ����ȡ��ť
    /// </summary>
    [SerializeField] Button mGetButton;

    /// <summary>
    /// ��Ƭ�����н���
    /// </summary>
    [SerializeField] GameObject mGardGroupGo;

    private List<RewardCard> mRewardCardList = new List<RewardCard>();

    /// <summary>
    /// ��ʼ������
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// ��ʾ���溯��
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }

    /// <summary>
    /// ��ӽ������齱�����
    /// </summary>
    /// <param name="info"></param>
    public void AddRewardInfo(RewardCardInfo info)
    {
        RewardCard rewardCard = Instantiate(mRewardCardPrefab);
        rewardCard.mRewardCardInfo = info;
        rewardCard.transform.SetParent(mGardGroupGo.transform);
        mRewardCardList.Add(rewardCard);
    }

    /// <summary>
    /// ȫ����ȡ������ť�¼�
    /// </summary>
    public void OnGetRewardButtonClick()
    {
        transform.gameObject.SetActive(false);
        //TO DO
        //������н���
    }

    public void OnClear()
    {

    }
}
