using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

public class GameMainPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject GamePanel;
    [SerializeField] private GameObject RevivePanel;
    [SerializeField] private GameObject VictoryPanel;
    [SerializeField] private GameObject SkillPanel;

    private void Start()
    {
        GameManager.instance.onGameState += onGameStateHandler;
        PlayerController.instance.onSkillBar += OnSkillBarHandler;
    }

    private void OnSkillBarHandler(float obj)
    {
        if (obj >= 1)
        {
            GameManager.instance.GameStateEnum = GameState.State.Pause;
            SkillPanel.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.onGameState -= onGameStateHandler;
        PlayerController.instance.onSkillBar -= OnSkillBarHandler;
    }
    private void onGameStateHandler(GameState.State state)
    {
        if (state == GameState.State.Over)
        {
            RevivePanel.SetActive(true);
        }
        else if (state == GameState.State.Victory) 
        {
            VictoryPanel.SetActive(true);
        }
    }
}
