using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private Transform _muzzle;
    [SerializeField] public Transform _center;
    private Rigidbody _rigid;
    private bool _isGrounded;
    private Vector3 _destPos;


    private void Start()
    {
        Init();
        PlayerManager.Instance.HookedEvent.AddListener(SetDestPos);
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.Instance.IsHooked)
        {
            _rigid.useGravity = true; // 훅에 걸려있지 않을 때 중력 활성화
            PlayerMove();
        }
        else
        {
            _rigid.useGravity = false; // 훅에 걸려있을 때 중력 비활성화
            HookMove();
        }
    }

    private void Update()
    {
        PlayerAim();
        PlayerJump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isGrounded = false;
        }
    }

    private void OnDisable()
    {
        PlayerManager.Instance.HookedEvent.RemoveListener(SetDestPos);
    }

    private void Init()
    {
        PlayerManager.Instance._stats.MoveSpeed = _moveSpeed;
        PlayerManager.Instance._stats.JumpPower = _jumpPower;
        _rigid = GetComponent<Rigidbody>();
    }

    private void PlayerMove()
    {
        Vector3 velocity = _rigid.velocity;
        velocity.x = transform.TransformDirection(InputManager.Instance.MoveDirection).x * PlayerManager.Instance._stats.MoveSpeed;
        velocity.z = transform.TransformDirection(InputManager.Instance.MoveDirection).z * PlayerManager.Instance._stats.MoveSpeed;

        if(!_isGrounded)
        {
            velocity.y = _rigid.velocity.y;
        }
        
        _rigid.velocity = velocity;
    }

    private void PlayerJump()
    {
        if (InputManager.Instance.IsJump && _isGrounded)
        {
            _rigid.AddForce(Vector3.up * PlayerManager.Instance._stats.JumpPower, ForceMode.Impulse);
        }
    }

    private void PlayerAim()
    {
        float mouseX = InputManager.Instance.MousePosition.x * GameManager.Instance.MouseSensitivity;
        float mouseY = InputManager.Instance.MousePosition.y * GameManager.Instance.MouseSensitivity;

        float clampedMouseY = _camera.transform.localEulerAngles.x - mouseY;
        if (clampedMouseY > 180)
        {
            clampedMouseY -= 360;
        }

        _camera.transform.localRotation = Quaternion.Euler(clampedMouseY, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void SetDestPos(Vector3 destPos)
    {
        _destPos = destPos; 
    }

    private void HookMove()
    {
        Vector3 moveDir = (_destPos - _center.position).normalized;
        float distance = Vector3.Distance(_center.position, _destPos);
        if (distance > 1.45f)
        {
            PlayerManager.Instance.IsHookMove = true;
            _rigid.velocity = moveDir * 20;
        }
        else
        {
            PlayerManager.Instance.IsHookMove = false;
            _rigid.velocity = Vector3.zero;
        }
    }
}
