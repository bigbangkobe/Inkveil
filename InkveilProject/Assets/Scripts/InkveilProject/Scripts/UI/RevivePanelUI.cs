using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RevivePanelUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button back;
    [SerializeField] private GameObject failPanel;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
        back.onClick.AddListener(OnBackClick);
    }

    private void OnBackClick()
    {

        //SceneManager.LoadSceneAsync("Main");
        failPanel.SetActive(true);
    }

    private void OnClick()
    {
        PlayerController.instance.InitializePlaierInfo();
        GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play;
        gameObject.SetActive(false);
    }
}
