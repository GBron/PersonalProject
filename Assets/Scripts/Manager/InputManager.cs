using Base;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public ObseravableProperty<bool> MouseLClick = new();
    public ObseravableProperty<bool> MouseRClick = new();
    public ObseravableProperty<bool> SpacePress = new();
    public ObseravableProperty<bool> ESCPress = new();
    public ObseravableProperty<bool> FPress = new();

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
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        if (PlayerManager.Instance.IsPlayerDied) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESCPress.Value = !ESCPress.Value;
        }

        if (GameManager.Instance.IsGamePaused) return;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 훅에 걸려있을 때 점프시 훅 제거
            if (!PlayerManager.Instance.IsHookMove)
                PlayerManager.Instance.IsHooked = false;
            SpacePress.Value = !SpacePress.Value;
            IsJump = true;
        }
        else
        {
            IsJump = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FPress.Value = !FPress.Value;
        }
    }

    private void MouseInput()
    {
        if (GameManager.Instance.IsGamePaused) return;

        _mousePosition.x = Input.GetAxis("Mouse X");
        _mousePosition.y = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonDown(0))
        {
            MouseLClick.Value = !MouseLClick.Value;
        }
        if (Input.GetMouseButtonDown(1))
        {
            MouseRClick.Value = !MouseRClick.Value;
        }
    }

    private void SubscribedEvent()
    {

    }

    private void UnsubscribedEvent()
    {

    }
}
