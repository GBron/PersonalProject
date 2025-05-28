using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigid;
    [SerializeField] private float _moveSpeed;


    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void Init()
    {
        PlayerManager.Instance._stats.MoveSpeed = _moveSpeed;
        _rigid = GetComponent<Rigidbody>();
    }

    private void PlayerMove()
    {
        _rigid.velocity = InputManager.Instance.MoveDirection * PlayerManager.Instance._stats.MoveSpeed;
    }
}
