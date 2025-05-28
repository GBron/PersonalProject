using Base;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private Vector3 _moveDirection;
    public Vector3 MoveDirection
    {
        get { return _moveDirection; }
        set { _moveDirection = value; }
    }

    private void Awake()
    {
        SingletonInit();
    }

    private void Update()
    {
        MoveInput();
    }

    private void MoveInput()
    {
        // 각 입력을 받아 이동방향에 저장
        _moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            _moveDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _moveDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _moveDirection += Vector3.right;
        }

        _moveDirection.Normalize();
    }
}
