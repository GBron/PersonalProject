using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _hookCount;
    [SerializeField] private TMP_Text _bulletCount;
    [SerializeField] private TMP_Text _hpCount;

    private void Start()
    {
        PlayerManager.Instance.Stats.CurHookCount.Subscribe(SetHookUI);
        PlayerManager.Instance.Stats.CurBulletCount.Subscribe(SetBulletUI);
        PlayerManager.Instance.Stats.CurHp.Subscribe(SetHpUI);
        SetHookUI(0);
        SetBulletUI(0);
        SetHpUI(0);
    }

    private void SetHookUI(int value)
    {
        _hookCount.text = PlayerManager.Instance.Stats.CurHookCount.Value.ToString();
    }
    private void SetBulletUI(int value)
    {
        _bulletCount.text = PlayerManager.Instance.Stats.CurBulletCount.Value.ToString();
    }
    private void SetHpUI(int value)
    {
        _hpCount.text = PlayerManager.Instance.Stats.CurHp.Value.ToString();
    }
}
