using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _head;
    [SerializeField] private PooledObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;

    private int _hp = 1;
    private float _detectAngle = 45f;
    private float _detectRange = 75f;
    private LayerMask _playerLayer;
    private Collider[] _cols = new Collider[10];
    private bool _isPlayerDetected = false;
    private bool _canShot => _shotCoroutine == null;
    private Coroutine _shotCoroutine;
    private ObjectPool _bulletPool;
    private int _attackDamage;

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
        _attackDamage = 10;
        _playerLayer = 1 << 3;
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
        Debug.Log($"플레이어 추적 상태 : {_isPlayerDetected}");

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
        _head.transform.Rotate(Vector3.up * 10f * Time.deltaTime);

        // 플레이어 탐색
        if(Physics.OverlapSphereNonAlloc(_head.transform.position, _detectRange, _cols, _playerLayer) > 0)
        {
            // 플레이어가 탐지 될 시 벽이 있는지 Raycast로 확인
            float angle = Vector3.Angle(_head.transform.forward, _cols[0].transform.position);

            if (Physics.Raycast(_head.transform.position, (_cols[0].transform.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange) && angle <= _detectAngle)
            {
                // 플레이어가 탐지되면 _isPlayerDetected를 true로 설정
                if (hit.transform.gameObject.layer == 3)
                {
                    _isPlayerDetected = true;
                }
                // 플레이어가 탐지되지 않으면 _isPlayerDetected를 false로 설정
                else
                {
                    _isPlayerDetected = false;
                }
            }
        }
    }

    private void Trace()
    {
        // 플레이어를 바라봄
        _head.transform.LookAt(PlayerManager.Instance._player._center);

        // 플레이어에게 계속 레이캐스트를 쏘며 플레이어가 시야 내에 있는지 확인
        if (Physics.Raycast(_head.transform.position, (_cols[0].transform.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange))
        {
            // 플레이어가 시야 안이라면 사격
            if (hit.transform.gameObject.layer == 3)
            {
                if (_canShot)
                {
                    _shotCoroutine = StartCoroutine(Shot());
                }
            }
            // 플레이어가 시야 밖이라면 탐지가 안되는 것이므로 탐색모드로 변경
            else
            {
                _isPlayerDetected = false;
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_head.transform.position, _detectRange);
        Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(0, -_detectAngle, 0) * _head.transform.forward * _detectRange);
        Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(0, _detectAngle, 0) * _head.transform.forward * _detectRange);
        Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(-_detectAngle, 0, 0) * _head.transform.forward * _detectRange);
        Gizmos.DrawLine(_head.transform.position, _head.transform.position + Quaternion.Euler(_detectAngle, 0, 0) * _head.transform.forward * _detectRange);
    }

    IEnumerator Shot()
    {
        EnemyBullet bullet = _bulletPool.PopPool() as EnemyBullet;
        bullet.transform.position = _muzzle.position;
        bullet.transform.rotation = _muzzle.rotation;
        bullet.Rigid.AddForce(bullet.transform.forward * 35f, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);
        _shotCoroutine = null;
    }
}
