using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _hookCount;
    [SerializeField] private TMP_Text _bulletCount;

    private void Start()
    {
        PlayerManager.Instance.CurHookCount.Subscribe(SetHookUI);
        PlayerManager.Instance.CurBulletCount.Subscribe(SetBulletUI);
        SetHookUI(0);
        SetBulletUI(0);
    }

    private void SetHookUI(int value)
    {
        _hookCount.text = PlayerManager.Instance.CurHookCount.Value.ToString();
    }
    private void SetBulletUI(int value)
    {
        _bulletCount.text = PlayerManager.Instance.CurBulletCount.Value.ToString();
    }
}
