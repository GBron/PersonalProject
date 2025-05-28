using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _hookCount;

    private void Start()
    {
        PlayerManager.Instance.CurHookCount.Subscribe(SetUI);
        SetUI(0);
    }

    private void SetUI(int value)
    {
        _hookCount.text = PlayerManager.Instance.CurHookCount.Value.ToString();
    }
}
