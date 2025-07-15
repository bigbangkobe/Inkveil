using System;
using System.Collections;
using UnityEngine;

public class GodAttackCtrl : MonoBehaviour
{
    public GodInfo _godInfo;
    public Transform _my;
    public Transform _target;
    internal virtual void OnInitialFlag(GodInfo godInfo,Transform tf1, Transform tf2)
    {
        _godInfo = godInfo;
        _my = tf1;
        _target = tf2;
    }
}