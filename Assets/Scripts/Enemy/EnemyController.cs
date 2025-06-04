using Base;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject _head;
    [SerializeField] private PooledObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Light _light;

    private LineRenderer _laser;
    private int _hp = 1;
    private float _detectAngle = 45f;
    private float _detectRange = 75f;
    private LayerMask _ignoreLayer = ~((1 << 8) | (1 << 9) | (1 << 11));
    // private Collider[] _cols = new Collider[10];
    private bool _isPlayerDetected = false;
    private bool _canShot => _shotCoroutine == null;
    private Coroutine _shotCoroutine;
    private ObjectPool _bulletPool;
    private WaitForSeconds _wait = new WaitForSeconds(1f);
    private bool _isStop = false;
    private AudioSource _audioSource;

    private void Awake()
    {
        Init();
        GameManager.Instance.EnemyCount++;
    }

    private void Update()
    {
        HandleEnemy();
    }

    private void OnDisable()
    {
        GameManager.Instance.EnemyCount--;
    }

    private void Init()
    {
        _bulletPool = new ObjectPool(transform, _bulletPrefab, 10);
        _laser = GetComponent<LineRenderer>();
        _audioSource = GetComponent<AudioSource>();
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

        // 머리가 돌아가며 주변 탐색
        _head.transform.Rotate(Vector3.up * 60f * Time.deltaTime);

        // 플레이어 탐색 overlapsphere
        // if (Physics.OverlapSphereNonAlloc(_head.transform.position, _detectRange, _cols, _playerLayer) > 0)

        // 플레이어 탐색 raycast
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, 200f, _ignoreLayer))
        {
            float angle = Vector3.Angle(_head.transform.forward, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized);
            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            Debug.DrawRay(_head.transform.position, _head.transform.forward * _detectRange, Color.green);

            // 플레이어가 탐지 될 시 벽이 있는지 Raycast로 확인
            if (distance <= _detectRange)
            {
                // 플레이어가 탐지되면 _isPlayerDetected를 true로 설정
                if (hit.transform.gameObject.layer == 3 && angle <= _detectAngle)
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
        _light.gameObject.SetActive(false);
        // 플레이어를 바라봄

        if (!_isStop)
            _head.transform.LookAt(PlayerManager.Instance.Player._center);
        // _head.transform.rotation = Quaternion.Slerp(_head.transform.rotation, Quaternion.LookRotation(PlayerManager.Instance.Player._center.position - _head.transform.position), 0.6f);

        // 플레이어에게 계속 레이캐스트를 쏘며 플레이어가 시야 내에 있는지 확인
        if (Physics.Raycast(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized, out RaycastHit hit, _detectRange, _ignoreLayer))
        {
            // Debug.DrawRay(_head.transform.position, (PlayerManager.Instance.Player._center.position - _head.transform.position).normalized * _detectRange, Color.yellow);

            float distance = Vector3.Distance(PlayerManager.Instance.Player._center.position, _head.transform.position);

            // 플레이어가 시야 안이라면 사격
            if (hit.transform.gameObject.layer == 3 && distance < _detectRange)
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
                Vector3 euler = _head.transform.eulerAngles;
                euler.x = 0;
                _head.transform.rotation = Quaternion.Euler(euler);
            }
        }
    }

    IEnumerator Shot()
    {
        _isStop = false;
        _laser.enabled = true;
        RaycastHit hit = default;
        float timer = 0;

        // 초기 추적
        while (timer < 1.5f)
        {
            bool nothing = Physics.Raycast(_muzzle.transform.position, _muzzle.transform.forward, out hit, _detectRange, _ignoreLayer);

            Debug.DrawRay(_head.transform.position, _head.transform.forward * _detectRange, Color.green);

            if (hit.transform.gameObject.layer != 3)
            {
                _laser.enabled = false;
                _shotCoroutine = null;
                _isPlayerDetected = false;
                yield break;
            }

            _laser.SetPosition(0, _muzzle.transform.position);
            _laser.SetPosition(1, hit.point);

            timer += 1.5f * Time.deltaTime;
            yield return null;
        }

        timer = 0;
        _isStop = true;


        // 락온
        while (timer < 0.3f)
        {
            bool nothing = Physics.Raycast(_muzzle.transform.position, _muzzle.transform.forward, out hit, _detectRange, _ignoreLayer);

            if (timer % 0.1f < 0.05f)
            {
                _laser.SetPosition(0, _muzzle.transform.position);
                _laser.SetPosition(1, _muzzle.transform.position + _muzzle.transform.forward * 0.01f);
            }
            else
            {
                _laser.SetPosition(0, _muzzle.transform.position);
                _laser.SetPosition(1, hit.point);
            }

            timer += 1f * Time.deltaTime;
            yield return null;
        }

        _laser.enabled = false;

        _audioSource.Play();
        EnemyBullet bullet = _bulletPool.PopPool() as EnemyBullet;
        bullet.transform.position = _muzzle.position;
        bullet.transform.rotation = _muzzle.rotation;
        Vector3 vec = (hit.point - bullet.transform.position).normalized;
        bullet.Rigid.AddForce(vec * 250f, ForceMode.Impulse);

        yield return _wait;

        _shotCoroutine = null;
    }
}
