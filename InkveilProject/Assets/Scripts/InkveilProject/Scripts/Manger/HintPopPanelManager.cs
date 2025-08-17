using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class HintPopPanelManager : MonoSingleton<HintPopPanelManager>
{
    private HintPopPanel hintPopPanel;

    public void OnInit() { }

    protected async override void Awake()
    {
        base.Awake();

        GameObject hint = (await ResourceService.LoadAsync<GameObject>("UI/HintPopPanel"));
        hintPopPanel = Instantiate(hint, transform).GetComponent<HintPopPanel>();
        hintPopPanel.gameObject.SetActive(false);
    }

    public void ShowHintPop(string content) 
    {
        hintPopPanel?.ShowHint(content);
    }
}