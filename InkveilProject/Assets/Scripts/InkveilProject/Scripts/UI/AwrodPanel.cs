using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;

public class AwrodPanel : MonoBehaviour
{
    private Button m_Btn;
    public RewardCard m_RewardCard;
    //public List<RewardCard> m_RewardCards = new List<RewardCard>();

    private void Start()
    {
        m_Btn = GetComponent<Button>();
        m_Btn.onClick.AddListener(OnClickHandler);
        m_RewardCard.gameObject.SetActive(true);
    }

    public void InitinlRewards(PropertyInfo propertyInfos)
    {
        gameObject.SetActive(true);
        m_RewardCard.OnInit(propertyInfos);
        BagManager.instance.AddItem(propertyInfos);
    }

    private void OnClickHandler()
    {
        gameObject.SetActive(false);
    }
}
