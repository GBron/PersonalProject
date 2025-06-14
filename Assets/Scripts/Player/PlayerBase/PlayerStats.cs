using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

public class PlayerStats
{
    public ObseravableProperty<int> CurHp = new();
    public ObseravableProperty<int> MaxHp = new();
    public ObseravableProperty<float> CurHookCount = new ObseravableProperty<float>();
    public ObseravableProperty<int> CurBulletCount = new ObseravableProperty<int>();
    public ObseravableProperty<int> CurBarrierCount = new ObseravableProperty<int>();

    public float MoveSpeed { get; set; }
    public float JumpPower { get; set; }
    public float HookSpeed { get; set; }
    public float HookRange { get; set; }
    public float HookCooldown { get; set; }
    public int HookCount { get; set; }
    public int BulletCount { get; set; }
    public int BarrierCount { get; set; }
}
