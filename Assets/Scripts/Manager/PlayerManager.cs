using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PooledObject _hookPrefab;
    [SerializeField] private GameObject _playerPrefab;
    public PlayerStats _stats;
    public ObjectPool _hookPool;
    private Vector3 _spawnPoint;
    public PlayerMovement _player;

    public bool IsHooked { get; set; } = false;
    public bool IsHookMove { get; set; } = false;

    public UnityEvent<Vector3> HookedEvent;

    private void Awake()
    {
        Init();
        SubscribedEvent();
        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoint, Quaternion.identity);
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
        _hookPool = new ObjectPool(transform, _hookPrefab, 3);
        _stats.MaxHp.Value = 100;
        _stats.CurHp.Value = _stats.MaxHp.Value;
        _stats.MoveSpeed = 5f;
        _stats.HookSpeed = 20f;
        _stats.HookRange = 30f;
        _stats.JumpPower = 5f;
        _stats.BulletCount = 3;
    }

    public Hook GetHook()
    {
        return _hookPool.PopPool() as Hook;
    }


    private void SubscribedEvent()
    {

    }

    private void UnsubscribedEvent()
    {

    }
}
