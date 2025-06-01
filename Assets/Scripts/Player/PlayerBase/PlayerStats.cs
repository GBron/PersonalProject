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
    // 플레이어 사망 후 이루어지는 것들은 Start에서 구독해야됨. Awake에서 false로 초기화.
    public ObseravableProperty<bool> IsDied = new ObseravableProperty<bool>();

    public float MoveSpeed { get; set; }
    public float JumpPower { get; set; }
    public float HookSpeed { get; set; }
    public float HookRange { get; set; }
    public float HookCooldown { get; set; }
    public int HookCount { get; set; }
    public int BulletCount { get; set; }
}
