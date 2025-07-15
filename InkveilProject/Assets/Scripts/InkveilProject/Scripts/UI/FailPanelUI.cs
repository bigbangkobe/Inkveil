using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPanelUI : MonoBehaviour
{
    private Button m_Btn; 

    private void Start()
    {
        m_Btn = GetComponent<Button>();

        m_Btn.onClick.AddListener(OnClickHandler);
        GuideManager.instance.OnPlayRandomGuideByType(2);
    }

    private void OnClickHandler()
    {
        GameManager.instance.OnClear();
        SceneManager.LoadSceneAsync("UI");
    }
}
