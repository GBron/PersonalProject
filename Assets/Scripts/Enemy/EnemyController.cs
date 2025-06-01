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
        Debug.Log("Ž�����");
        _light.gameObject.SetActive(true);

        // �Ӹ��� ���ư��� �ֺ� Ž��
        _head.transform.Rotate(Vector3.up * 60f * Time.deltaTime);

        // �÷��̾� Ž�� overlapsphere
        // if (Physics.OverlapSphereNonAlloc(_head.transform.position, _detectRange, _cols, _playerLayer) > 0)

        // �÷��̾� Ž�� raycast
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, 200f, _ignoreLayer))
        {
            Debug.Log($"���� ���� �÷��̾ ����");
            
            float angle = Vector3.Angle(_head.transform.forward, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized);
            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            Debug.DrawRay(_head.transform.position, _head.transform.forward * _detectRange, Color.green);

            // �÷��̾ Ž�� �� �� ���� �ִ��� Raycast�� Ȯ��
            if (distance <= _detectRange)
            {
                // �÷��̾ Ž���Ǹ� _isPlayerDetected�� true�� ����
                if (hit.transform.gameObject.layer == 3 && angle <= _detectAngle)
                {
                    Debug.Log($"�÷��̾� �߰�");
                    _isPlayerDetected = true;
                }
                // �÷��̾ Ž������ ������ _isPlayerDetected�� false�� ����
                else
                {
                    Debug.Log($"���� ����");
                    _isPlayerDetected = false;
                }
            }
        }
    }

    private void Trace()
    {
        Debug.Log("�������");
        _light.gameObject.SetActive(false);
        // �÷��̾ �ٶ�
        _head.transform.LookAt(PlayerManager.Instance.Player._center);

        // �÷��̾�� ��� ����ĳ��Ʈ�� ��� �÷��̾ �þ� ���� �ִ��� Ȯ��
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange, _ignoreLayer))
        {
            Debug.DrawRay(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized * _detectRange, Color.yellow);

            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            // �÷��̾ �þ� ���̶�� ���
            if (hit.transform.gameObject.layer == 3 && distance < _detectRange)
            {
                if (_canShot)
                {
                    Debug.Log($"���!");
                    _shotCoroutine = StartCoroutine(Shot());
                }
            }
            // �÷��̾ �þ� ���̶�� Ž���� �ȵǴ� ���̹Ƿ� Ž������ ����
            else
            {
                Debug.Log($"�÷��̾� ��ħ. Ž������ ���ư�");
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
