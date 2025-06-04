using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    private Canvas _canvas;
    
    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _button.onClick.AddListener(GameManager.Instance.GoTitle);
        _button.onClick.AddListener(DisableUI);
        PlayerManager.Instance.OnPlayerDie.AddListener(EnableUI);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerDie.RemoveListener(EnableUI);
    }

    private void EnableUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _canvas.enabled = true;
    }

    private void DisableUI()
    {
        _canvas.enabled = false;
    }
}
