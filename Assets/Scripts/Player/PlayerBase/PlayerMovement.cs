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
    private LayerMask _ignoreLayer = ~(1 << 3);


    private void Start()
    {
        Init();
        PlayerManager.Instance.HookedEvent.AddListener(SetDestPos);
        InputManager.Instance.SpacePress.Subscribe(PlayerJump);
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
    }

    private void OnDisable()
    {
        PlayerManager.Instance.HookedEvent.RemoveListener(SetDestPos);
        InputManager.Instance.SpacePress.UnsubscribeAll();
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

    private void PlayerJump(bool value)
    {
        // 바닥으로 레이캐스트를 쏘고 법선 백터랑 비교해 각도로 점프가 가능한지 판별
        Ray ray = new Ray(PlayerManager.Instance.Player._center.position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit hit, 1.2f, _ignoreLayer))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            if(angle < 30f)
            {
                _rigid.AddForce(transform.up * _jumpPower, ForceMode.Impulse);
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }
    }

    private void PlayerAim()
    {
        if (GameManager.Instance.IsGamePaused) return;

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

        // 훅이 지형에 닿을 경우 플레이어가 날아가는 로직. 날아가는 동안은 훅을 발사할 수 없음.
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
