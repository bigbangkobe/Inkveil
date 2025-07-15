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



    private async void Awake()
    {
        PlayerDispositionManager.instance.onPlayerAssetsChanged += OnPlayerAssetsChangde;
    }

    private void OnDestroy()
    {
        PlayerDispositionManager.instance.onPlayerAssetsChanged -= OnPlayerAssetsChangde;
    }

    private void OnPlayerAssetsChangde()
    {
        m_PlayerAssetsInfo = PlayerDispositionManager.instance.PlayerAssetsInfo;
        m_GGTest.text = m_PlayerAssetsInfo.moneyInit.ToString();
        m_MHTest.text = m_PlayerAssetsInfo.MHInit.ToString();
        m_XYTest.text = m_PlayerAssetsInfo.XianHInit.ToString();
        m_TLTest.text = m_PlayerAssetsInfo.staminaInit.ToString();
    }

    private void OnEnable()
    {
        OnPlayerAssetsChangde();
    }
}
