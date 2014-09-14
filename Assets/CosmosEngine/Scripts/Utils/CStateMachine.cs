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
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class CState<T>
{
    public T ToState;

    public CState(T toState)
    {
        ToState = toState;
    }

    public abstract void OnInit();
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnBreathe();
}

/// <summary>
/// 有限狀態機, TODO: 未 update (breath)
/// </summary>
public class CStateMachine<OBJ, STATE>
{
    OBJ Object_;
    STATE CurState;
    STATE LastState;

    bool stateChangedFlag = false;
    Dictionary<STATE, CState<STATE>> StatesHandlers = new Dictionary<STATE, CState<STATE>>();

    public CStateMachine(OBJ obj, STATE initState, CState<STATE>[] stateMap) 
    {
        Object_ = obj;
        LastState = initState;
        CurState = initState;

        Array statesArray = Enum.GetValues(typeof(STATE));
        CBase.Assert(statesArray.Length == stateMap.Length);
        
        CBase.Assert(Object_);

        foreach (CState<STATE> state in stateMap)
        {
            StatesHandlers[state.ToState] = state;
            state.OnInit();
        }
    }

    void Update()
    {
        if (stateChangedFlag)
        {
            StatesHandlers[LastState].OnExit();

            StatesHandlers[CurState].OnEnter();
            stateChangedFlag = false;
        }

        StatesHandlers[CurState].OnBreathe();
    }

    public void SetState(STATE state)
    {
        LastState = CurState;
        CurState = state;
        stateChangedFlag = true;
    }
}
