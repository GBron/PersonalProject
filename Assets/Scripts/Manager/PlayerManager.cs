using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerStats _stats;

    private void Awake()
    {
        Init();
        SubscribedEvent();
    }

    private void OnDestroy()
    {
        UnsubscribedEvent();
    }

    private void Init()
    {
        SingletonInit();
        _stats = new PlayerStats();
        _stats.MaxHp.Value = 100;
        _stats.CurHp.Value = _stats.MaxHp.Value;
        _stats.MoveSpeed = 5f;
        _stats.HookSpeed = 10f;
        _stats.HookRange = 20f; 
    }


    private void SubscribedEvent()
    {

    }

    private void UnsubscribedEvent()
    {

    }
}
