﻿//------------------------------------------------------------------------------
//
//      CosmosEngine - The Lightweight Unity3D Game Develop Framework
// 
//                     Version 0.8 (20140904)
//                     Copyright © 2011-2014
//                   MrKelly <23110388@qq.com>
//              https://github.com/mr-kelly/CosmosEngine
//
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// Unity原生UI桥接器 TODO:
/// </summary>
public class CUGUIBridge : ICUIBridge
{
    // Init the UI Bridge, necessary
    public void InitBridge()
    {
    }

    // Get a component inside the UI Bridge
    public object GetUIComponent(string comName)
    {
        return null;
    }

    public void UIObjectFilter(GameObject uiObject) { }
}
