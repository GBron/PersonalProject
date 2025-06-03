using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookUI : MonoBehaviour
{
    [SerializeField] private Image _hookGauge;
    [SerializeField] private Image _bullet1;
    [SerializeField] private Image _bullet2;
    [SerializeField] private Image _bullet3;
    [SerializeField] private TMP_Text _minute;
    [SerializeField] private TMP_Text _second;

    private void Start()
    {
        PlayerManager.Instance.Stats.CurHookCount.Subscribe(SetHookGauge);
        PlayerManager.Instance.Stats.CurBulletCount.Subscribe(SetbulletUI);
        SetHookGauge(0);
        SetbulletUI(0);
    }

    private void Update()
    {
        _minute.text = ((int)(GameManager.Instance.Timer / 60)).ToString();
        _second.text = (GameManager.Instance.Timer % 60f).ToString("F2");
    }

    private void OnDisable()
    {
        PlayerManager.Instance.Stats.CurHookCount.Unsubscribe(SetHookGauge);
        PlayerManager.Instance.Stats.CurBulletCount.Unsubscribe(SetbulletUI);
    }


    public void SetHookGauge(float value)
    {
        float fillAmount = PlayerManager.Instance.Stats.CurHookCount.Value / 3;
        _hookGauge.fillAmount = fillAmount;
    }

    public void SetbulletUI(int value)
    {
        switch (PlayerManager.Instance.Stats.CurBulletCount.Value)
        {
            case 0:
                _bullet1.enabled = false;
                _bullet2.enabled = false;
                _bullet3.enabled = false;
                break;
            case 1:
                _bullet1.enabled = false;
                _bullet2.enabled = false;
                _bullet3.enabled = true;
                break;
            case 2:
                _bullet1.enabled = false;
                _bullet2.enabled = true;
                _bullet3.enabled = true;
                break;
            case 3:
                _bullet1.enabled = true;
                _bullet2.enabled = true;
                _bullet3.enabled = true;
                break;
            default:
                break;
        }
    }
}
