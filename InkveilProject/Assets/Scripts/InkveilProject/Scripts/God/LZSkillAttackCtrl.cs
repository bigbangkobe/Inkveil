using System;
using System.Collections;
using UnityEngine;

public class LZSkillAttackCtrl : GodAttackCtrl
{
    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);

        //transform.position = tf1.position;
    }

    private void OnEnable() 
    {
        Invoke(nameof(Hide),3);
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}