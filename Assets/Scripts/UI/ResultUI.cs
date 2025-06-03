using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _minute;
    [SerializeField] private TMP_Text _second;
    [SerializeField] private Button _button;

    private void Start()
    {
        _minute.text = ((int)(GameManager.Instance.Timer / 60)).ToString();
        _second.text = (GameManager.Instance.Timer % 60f).ToString("F2");
        _button.onClick.AddListener(GameManager.Instance.GoTitle);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }
}
