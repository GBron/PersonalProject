using Cinemachine;
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
            _rigid.useGravity = true; // �ſ� �ɷ����� ���� �� �߷� Ȱ��ȭ
            PlayerMove();
        }
        else
        {
            _rigid.useGravity = false; // �ſ� �ɷ����� �� �߷� ��Ȱ��ȭ
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
        PlayerManager.Instance.Stats.MoveSpeed = _moveSpeed;
        PlayerManager.Instance.Stats.JumpPower = _jumpPower;
        _rigid = GetComponent<Rigidbody>();
    }

    private void PlayerMove()
    {
        Vector3 velocity = _rigid.velocity;
        velocity.x = transform.TransformDirection(InputManager.Instance.MoveDirection).x * PlayerManager.Instance.Stats.MoveSpeed;
        velocity.z = transform.TransformDirection(InputManager.Instance.MoveDirection).z * PlayerManager.Instance.Stats.MoveSpeed;

        if (!_isGrounded)
        {
            velocity.y = _rigid.velocity.y;
        }

        _rigid.velocity = velocity;
    }

    private void PlayerJump()
    {
        if (InputManager.Instance.IsJump && _isGrounded)
        {
            _rigid.AddForce(Vector3.up * PlayerManager.Instance.Stats.JumpPower, ForceMode.Impulse);
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
        if (PlayerManager.Instance.HookCoroutine == null)
            PlayerManager.Instance.HookCoroutine = StartCoroutine(PlayerManager.Instance.CutHook());

        Vector3 moveDir = (_destPos - _center.position).normalized;
        float distance = Vector3.Distance(_center.position, _destPos);

        // ���� ������ ���� ��� �÷��̾ ���ư��� ����. ���ư��� ������ ���� �߻��� �� ����.
        if (distance > 1.45f)
        {
            PlayerManager.Instance.IsHookMove = true;
            _rigid.velocity = moveDir * PlayerManager.Instance.Stats.HookSpeed;
        }
        else
        {
            PlayerManager.Instance.IsHookMove = false;
            _rigid.velocity = Vector3.zero;
            StopCoroutine(PlayerManager.Instance.HookCoroutine);
            PlayerManager.Instance.HookCoroutine = null;
        }
    }
}
