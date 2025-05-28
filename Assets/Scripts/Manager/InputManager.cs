using Base;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private ObseravableProperty<bool> _mouseLClick = new();
    private ObseravableProperty<bool> _mouseRClick = new();

    private Vector3 _moveDirection;
    public Vector3 MoveDirection
    {
        get { return _moveDirection; }
        set { _moveDirection = value; }
    }

    private Vector2 _mousePosition;
    public Vector2 MousePosition
    {
        get { return _mousePosition; }
        set { _mousePosition = value; }
    }

    [field: SerializeField] public bool IsJump { get; private set; }

    private void Awake()
    {
        SingletonInit();
    }

    private void Update()
    {
        MouseInput();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsJump = true;
        }
        else
        {
            IsJump = false;
        }

        _moveDirection.Normalize();
    }

    private void MouseInput()
    {
        _mousePosition.x = Input.GetAxis("Mouse X");
        _mousePosition.y = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonDown(0))
        {
            _mouseLClick.Value = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _mouseLClick.Value = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            _mouseRClick.Value = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _mouseRClick.Value = false;
        }
    }
}
