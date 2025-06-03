using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BarrierUI : MonoBehaviour
{
    [SerializeField] private Image _RBarrier1;
    [SerializeField] private Image _RBarrier2;
    [SerializeField] private Image _RBarrier3;
    [SerializeField] private Image _LBarrier1;
    [SerializeField] private Image _LBarrier2;
    [SerializeField] private Image _LBarrier3;
    [SerializeField] private Volume _volume;
    private Vignette _vignette;
    private Coroutine _coroutine;

    private void OnEnable()
    {
        bool check = _volume.profile.TryGet<Vignette>(out _vignette);
        Debug.Log($"TryGet 성공 여부 : {check}");
        PlayerManager.Instance.Stats.CurBarrierCount.Subscribe(SetBarrierUI);
        PlayerManager.Instance.Stats.CurBarrierCount.Subscribe(BarrierHitEffect);
        SetBarrierUI(0);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.Stats.CurBarrierCount.Unsubscribe(SetBarrierUI);
        PlayerManager.Instance.Stats.CurBarrierCount.Unsubscribe(BarrierHitEffect);
    }

    private void BarrierHitEffect(int value)
    {
        if (_coroutine == null)
            _coroutine = StartCoroutine(VignetteAnim());
    }

    private void SetBarrierUI(int value)
    {
        switch (PlayerManager.Instance.Stats.CurBarrierCount.Value)
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

    IEnumerator VignetteAnim()
    {
        float timer = 0f;
        bool calcu = false;

        while (timer < 0.2f)
        {
            if (!calcu)
            {
                _vignette.intensity.value += 5f * Time.deltaTime;
                if (timer > 0.1f)
                    calcu = true;
            }
            else
            {
                _vignette.intensity.value -= 5f * Time.deltaTime;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        _coroutine = null;
    }
}
