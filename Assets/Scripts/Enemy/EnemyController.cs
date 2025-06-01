using Base;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _head;
    [SerializeField] private PooledObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Light _light;

    private int _hp = 1;
    private float _detectAngle = 45f;
    private float _detectRange = 75f;
    private LayerMask _playerLayer = 1 << 3;
    private LayerMask _ignoreLayer = ~((1 << 8) | (1 << 9) | (1 << 11));
    // private Collider[] _cols = new Collider[10];
    private bool _isPlayerDetected = false;
    private bool _canShot => _shotCoroutine == null;
    private Coroutine _shotCoroutine;
    private ObjectPool _bulletPool;
    private int _attackDamage = 10;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        HandleEnemy();
    }

    private void Init()
    {
        _bulletPool = new ObjectPool(transform, _bulletPrefab, 10);
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void HandleEnemy()
    {
        Debug.Log($"플레이어 탐지 : {_isPlayerDetected}");

        if (_isPlayerDetected)
        {
            Trace();
        }
        else
        {
            Detecting();
        }
    }

    public void SetDetected()
    {
        _isPlayerDetected = true;
    }

    private void Detecting()
    {
        Debug.Log("탐색모드");
        _light.gameObject.SetActive(true);

        // 머리가 돌아가며 주변 탐색
        _head.transform.Rotate(Vector3.up * 60f * Time.deltaTime);

        // 플레이어 탐색 overlapsphere
        // if (Physics.OverlapSphereNonAlloc(_head.transform.position, _detectRange, _cols, _playerLayer) > 0)

        // 플레이어 탐색 raycast
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, 200f, _ignoreLayer))
        {
            Debug.Log($"범위 내에 플레이어가 있음");
            
            float angle = Vector3.Angle(_head.transform.forward, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized);
            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            Debug.DrawRay(_head.transform.position, _head.transform.forward * _detectRange, Color.green);

            // 플레이어가 탐지 될 시 벽이 있는지 Raycast로 확인
            if (distance <= _detectRange)
            {
                // 플레이어가 탐지되면 _isPlayerDetected를 true로 설정
                if (hit.transform.gameObject.layer == 3 && angle <= _detectAngle)
                {
                    Debug.Log($"플레이어 발견");
                    _isPlayerDetected = true;
                }
                // 플레이어가 탐지되지 않으면 _isPlayerDetected를 false로 설정
                else
                {
                    Debug.Log($"벽에 막힘");
                    _isPlayerDetected = false;
                }
            }
        }
    }

    private void Trace()
    {
        Debug.Log("추적모드");
        _light.gameObject.SetActive(false);
        // 플레이어를 바라봄
        _head.transform.LookAt(PlayerManager.Instance.Player._center);

        // 플레이어에게 계속 레이캐스트를 쏘며 플레이어가 시야 내에 있는지 확인
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange, _ignoreLayer))
        {
            Debug.DrawRay(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized * _detectRange, Color.yellow);

            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            // 플레이어가 시야 안이라면 사격
            if (hit.transform.gameObject.layer == 3 && distance < _detectRange)
            {
                if (_canShot)
                {
                    Debug.Log($"사격!");
                    _shotCoroutine = StartCoroutine(Shot());
                }
            }
            // 플레이어가 시야 밖이라면 탐지가 안되는 것이므로 탐색모드로 변경
            else
            {
                Debug.Log($"플레이어 놓침. 탐색모드로 돌아감");
                _isPlayerDetected = false;
                Vector3 euler = _head.transform.eulerAngles;
                euler.x = 0;
                _head.transform.rotation = Quaternion.Euler(euler);
            }

        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(_head.transform.position, _detectRange);
    //     Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(0, -_detectAngle, 0) * _head.transform.forward * _detectRange);
    //     Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(0, _detectAngle, 0) * _head.transform.forward * _detectRange);
    //     Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(-_detectAngle, 0, 0) * _head.transform.forward * _detectRange);
    //     Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(_detectAngle, 0, 0) * _head.transform.forward * _detectRange);
    // }

    IEnumerator Shot()
    {
        EnemyBullet bullet = _bulletPool.PopPool() as EnemyBullet;
        bullet.transform.position = _muzzle.position;
        bullet.transform.rotation = _muzzle.rotation;
        bullet.Rigid.AddForce(bullet.transform.forward * 75f, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);
        _shotCoroutine = null;
    }
}
