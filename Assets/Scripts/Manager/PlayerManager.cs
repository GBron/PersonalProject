using Base;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PooledObject _hookPrefab;
    [SerializeField] private PooledObject _bulletPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Animator _gunAnim;

    private Vector3 _spawnPoint;
    public PlayerStats Stats;
    public ObjectPool HookPool;
    public ObjectPool BulletPool;
    public PlayerMovement Player;
    public Coroutine HookCoroutine;
    public bool IsPlayerDied {  get; set; }
    public bool IsHooked { get; set; } = false;
    public bool IsHookMove { get; set; } = false;

    public UnityEvent<Vector3> HookedEvent;
    public UnityEvent OnPlayerDie;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        SubscribedEvent();
    }

    private void OnDestroy()
    {
        UnsubscribedEvent();
    }

    private void Init()
    {
        SingletonInit();
        _spawnPoint = new Vector3(3, 0, 3);
        Stats = new PlayerStats();
        IsPlayerDied = false;
        Stats.MaxHp.Value = 1;
        Stats.MoveSpeed = 5f;
        Stats.HookSpeed = 40f;
        Stats.HookRange = 40f;
        Stats.JumpPower = 5f;
        Stats.BulletCount = 3;
        Stats.HookCount = 3;
        Stats.BarrierCount = 3;
        HookPool = new ObjectPool(transform, _hookPrefab, Stats.HookCount);
        BulletPool = new ObjectPool(transform, _bulletPrefab, Stats.BulletCount);
    }

    private void PlayGunShot(bool value)
    {
        if (PlayerManager.Instance.Stats.CurBulletCount.Value > 0)
            _gunAnim.SetTrigger("Shot");
    }

    public void PlayerStatReset()
    {
        Stats.CurHp.Value = Stats.MaxHp.Value;
        Stats.CurBulletCount.Value = Stats.BulletCount;
        Stats.CurHookCount.Value = Stats.HookCount;
        Stats.CurBarrierCount.Value = Stats.BarrierCount;
    }

    public void InstantiatePlayer()
    {
        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoint, Quaternion.identity);
        Player = playerInstance.GetComponent<PlayerMovement>();
    }

    public Hook GetHook()
    {
        return HookPool.PopPool() as Hook;
    }

    public Bullet GetBullet()
    {
        return BulletPool.PopPool() as Bullet;
    }

    public void TakeDamage(int damage)
    {
        if (Stats.CurBarrierCount.Value > 0)
            Stats.CurBarrierCount.Value -= damage;
        else
            Stats.CurHp.Value -= damage;

        if (Stats.CurHp.Value <= 0)
        {
            Dying();
        }
    }

    private void Dying()
    {
        IsPlayerDied = true;
        OnPlayerDie?.Invoke();
    }

    // 훅이 2초 이상 연결되어 있을 경우(플레이어가 턱에 끼임) 자동으로 IsHookMove를 false로
    public IEnumerator CutHook()
    {
        yield return new WaitForSeconds(2f);
        IsHookMove = false;
        IsHooked = false;
        HookCoroutine = null;
    }

    private void SubscribedEvent()
    {
        InputManager.Instance.MouseLClick.Subscribe(PlayGunShot);
    }

    private void UnsubscribedEvent()
    {
        InputManager.Instance.MouseLClick.Unsubscribe(PlayGunShot);
    }
}
