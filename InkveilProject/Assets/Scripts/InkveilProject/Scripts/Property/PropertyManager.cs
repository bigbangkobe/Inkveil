using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : MonoSingleton<PropertyManager>
{
    public void OnInit() 
    {
        PropertyDispositionManager.instance.OnInit();
    }

    internal void Clear()
    {
        //PropertyDispositionManager.instance.OnClear();
    }
}
