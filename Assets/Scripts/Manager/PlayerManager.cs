using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PooledObject _hookPrefab;
    [SerializeField] private PooledObject _bulletPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;

    public PlayerStats _stats;
    public ObjectPool _hookPool;
    public ObjectPool _bulletPool;
    public PlayerMovement _player;
    

    public bool IsHooked { get; set; } = false;
    public bool IsHookMove { get; set; } = false;

    public UnityEvent<Vector3> HookedEvent;

    private void Awake()
    {
        Init();
        SubscribedEvent();
        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        _player = playerInstance.GetComponent<PlayerMovement>();
    }
    private void OnDestroy()
    {
        UnsubscribedEvent();
    }

    private void Init()
    {
        SingletonInit();
        _stats = new PlayerStats();
        _stats.IsDied.Value = false;
        _stats.MaxHp.Value = 100;
        _stats.CurHp.Value = _stats.MaxHp.Value;
        _stats.MoveSpeed = 5f;
        _stats.HookSpeed = 45f;
        _stats.HookRange = 30f;
        _stats.JumpPower = 5f;
        _stats.BulletCount = 3;
        _stats.HookCooldown = 1f;
        _stats.HookCount = 5;
        _stats.CurHookCount.Value = _stats.HookCount;
        _stats.CurBulletCount.Value = _stats.BulletCount;
        _hookPool = new ObjectPool(transform, _hookPrefab, _stats.HookCount);
        _bulletPool = new ObjectPool(transform, _bulletPrefab, _stats.BulletCount);
    }

    public Hook GetHook()
    {
        return _hookPool.PopPool() as Hook;
    }

    public Bullet GetBullet()
    {
        return _bulletPool.PopPool() as Bullet;
    }

    public void TakeDamage(int damage)
    {
        _stats.CurHp.Value -= damage;

        if (_stats.CurHp.Value <= 0)
        {
            Dying();
        }
    }

    private void Dying()
    {
        // TODO: 플레이어가 사망했을 때 카메라 이동 후 사망 애니메이션 진행
        _stats.IsDied.Value = true;
    }

    private void SubscribedEvent()
    {

    }

    private void UnsubscribedEvent()
    {

    }
}
