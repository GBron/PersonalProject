using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private CinemachineVirtualCamera _camera;
    private Rigidbody _rigid;
    private bool _isGrounded;
    

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        PlayerMove();
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
}
