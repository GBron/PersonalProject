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
    private Collider[] _cols = new Collider[10];
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
        Debug.Log($"�÷��̾� Ž�� : {_isPlayerDetected}");

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
        _light.gameObject.SetActive(true);

        // �Ӹ��� ���ư��� �ֺ� Ž��
        _head.transform.Rotate(Vector3.up * 45f * Time.deltaTime);

        // �÷��̾� Ž��
        if (Physics.OverlapSphereNonAlloc(_head.transform.position, _detectRange, _cols, _playerLayer) > 0)
        {
            // �÷��̾ Ž�� �� �� ���� �ִ��� Raycast�� Ȯ��
            float angle = Vector3.Angle(_head.transform.forward, _cols[0].transform.position);

            if (Physics.Raycast(_head.transform.position, (_cols[0].transform.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange) && angle <= _detectAngle)
            {
                // �÷��̾ Ž���Ǹ� _isPlayerDetected�� true�� ����
                if (hit.transform.gameObject.layer == 3)
                {
                    _isPlayerDetected = true;
                }
                // �÷��̾ Ž������ ������ _isPlayerDetected�� false�� ����
                else
                {
                    _isPlayerDetected = false;
                }
            }
        }
    }

    private void Trace()
    {
        _light.gameObject.SetActive(false);
        // �÷��̾ �ٶ�
        _head.transform.LookAt(PlayerManager.Instance.Player._center);

        // �÷��̾�� ��� ����ĳ��Ʈ�� ��� �÷��̾ �þ� ���� �ִ��� Ȯ��
        if (Physics.Raycast(_head.transform.position, (_cols[0].transform.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange, _ignoreLayer))
        {
            // �÷��̾ �þ� ���̶�� ���
            if (hit.transform.gameObject.layer == 3)
            {
                if (_canShot)
                {
                    _shotCoroutine = StartCoroutine(Shot());
                }
            }
            // �÷��̾ �þ� ���̶�� Ž���� �ȵǴ� ���̹Ƿ� Ž������ ����
            else
            {
                _isPlayerDetected = false;
                Vector3 euler = _head.transform.eulerAngles;
                euler.x = 0;
                _head.transform.rotation = Quaternion.Euler(euler);
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
        bullet.Rigid.AddForce(bullet.transform.forward * 75f, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);
        _shotCoroutine = null;
    }
}
