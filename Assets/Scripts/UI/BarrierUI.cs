using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierUI : MonoBehaviour
{
    [SerializeField] private Image _RBarrier1;
    [SerializeField] private Image _RBarrier2;
    [SerializeField] private Image _RBarrier3;
    [SerializeField] private Image _LBarrier1;
    [SerializeField] private Image _LBarrier2;
    [SerializeField] private Image _LBarrier3;

    private void Start()
    {
        PlayerManager.Instance.Stats.CurBarrierCount.Subscribe(SetBarrierUI);
        SetBarrierUI(0);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.Stats.CurBarrierCount.Unsubscribe(SetBarrierUI);
    }

    private void SetBarrierUI(int value)
    {
        switch(PlayerManager.Instance.Stats.CurBarrierCount.Value)
        {
            case 0:
                _RBarrier1.enabled = false;
                _RBarrier2.enabled = false;
                _RBarrier3.enabled = false;
                _LBarrier1.enabled = false;
                _LBarrier2.enabled = false;
                _LBarrier3.enabled = false;
                break;
            case 1:
                _RBarrier1.enabled = true;
                _RBarrier2.enabled = false;
                _RBarrier3.enabled = false;
                _LBarrier1.enabled = true;
                _LBarrier2.enabled = false;
                _LBarrier3.enabled = false;
                break;
            case 2:
                _RBarrier1.enabled = true;
                _RBarrier2.enabled = true;
                _RBarrier3.enabled = false;
                _LBarrier1.enabled = true;
                _LBarrier2.enabled = true;
                _LBarrier3.enabled = false;
                break;
            case 3:
                _RBarrier1.enabled = true;
                _RBarrier2.enabled = true;
                _RBarrier3.enabled = true;
                _LBarrier1.enabled = true;
                _LBarrier2.enabled = true;
                _LBarrier3.enabled = true;
                break;
            default:
                break;
        }
    }
}
