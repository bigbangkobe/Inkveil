using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class InkveilProjectMain : MonoBehaviour
{
    private void Awake()
    {
        Main.instance.OnInit();
        GameManager.instance.OnInit();

        //Invoke(nameof(OnPlayBg), 0.3f);
    }

    private void OnPlayBg()
    {
       // SoundSystem.instance.Play("MainBG", 1, true, true);
    }
}
