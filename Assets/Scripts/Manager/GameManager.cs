using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field: SerializeField] public float MouseSensitivity { get; set; } = 1f;

    private void Awake()
    {
        SingletonInit();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
