using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAssetsUI : MonoBehaviour
{
    public TextMeshProUGUI m_GGTest;
    public TextMeshProUGUI m_MHTest;
    public TextMeshProUGUI m_XYTest;
    public TextMeshProUGUI m_TLTest;

    private PlayerAssetsInfo m_PlayerAssetsInfo;

    private void Awake()
    {
        PlayerDispositionManager.instance.onPlayerAssetsChangde += OnPlayerAssetsChangde;
    }

    private void OnDestroy()
    {
        PlayerDispositionManager.instance.onPlayerAssetsChangde -= OnPlayerAssetsChangde;
    }

    private void OnPlayerAssetsChangde()
    {
        m_PlayerAssetsInfo = PlayerDispositionManager.instance.PlayerAssetsInfo;
        int curQSLNum = 0;
        int curXHNum = 0;       //œ…ªÍ ˝¡ø
        BagItemInfo bagQSLItemInfo = BagManager.instance.GetItem((int)PropertyIDType.pleaseDivineOrder);
        if(bagQSLItemInfo != null)
        {
            curQSLNum = bagQSLItemInfo.propertyInfo.number;
        }
        BagItemInfo bagXHItemInfo = BagManager.instance.GetItem((int)PropertyIDType.theFairyFate);
        if (bagXHItemInfo != null)
        {
            curXHNum = bagXHItemInfo.propertyInfo.number;
            m_PlayerAssetsInfo.XianHInit = curXHNum;
        }
        m_GGTest.text = curQSLNum.ToString();//m_PlayerAssetsInfo.moneyInit.ToString();
        m_MHTest.text = m_PlayerAssetsInfo.MHInit.ToString();
        m_XYTest.text = curXHNum.ToString();
        m_TLTest.text = m_PlayerAssetsInfo.staminaInit.ToString();
    }

    private void OnEnable()
    {
        OnPlayerAssetsChangde();
    }
}
