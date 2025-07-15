using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChengZhanPanel : MonoBehaviour
{
    public Button m_Item1;
    public Button m_Item2;

    private void Awake()
    {
        m_Item1.onClick.AddListener(OnJinBiHandler);
        m_Item2.onClick.AddListener(OnGuanGaoHandler);
    }

    private void OnGuanGaoHandler()
    {
        PlayerDispositionManager.instance.AddStaminaInit(30);
    }

    public void OnJinBiHandler() 
    {
        bool isOK = PlayerDispositionManager.instance.HasEnoughCurrency(20);

        if (isOK) 
        {
            PlayerDispositionManager.instance.DeductCurrency(20);
            PlayerDispositionManager.instance.AddStaminaInit(15);
        }
    }
}
