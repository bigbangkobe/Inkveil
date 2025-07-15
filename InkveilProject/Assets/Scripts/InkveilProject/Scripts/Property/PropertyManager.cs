using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyManager : MonoSingleton<PropertyManager>
{
    public async void OnInit() 
    {
        await PropertyDispositionManager.instance.OnInitAsync();
    }

    internal void Clear()
    {
        //PropertyDispositionManager.instance.OnClear();
    }
}
