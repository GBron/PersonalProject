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

    public PlayerStats Stats;
    public ObjectPool HookPool;
    public ObjectPool BulletPool;
    public PlayerMovement Player;
    public Coroutine HookCoroutine;
    

    public bool IsHooked { get; set; } = false;
    public bool IsHookMove { get; set; } = false;

    public UnityEvent<Vector3> HookedEvent;

    private void Awake()
    {
        Init();
        SubscribedEvent();
        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        Player = playerInstance.GetComponent<PlayerMovement>();
    }
    private void OnDestroy()
    {
        UnsubscribedEvent();
    }

    private void Init()
    {
        SingletonInit();
        Stats = new PlayerStats();
        Stats.IsDied.Value = false;
        Stats.MaxHp.Value = 100;
        Stats.CurHp.Value = Stats.MaxHp.Value;
        Stats.MoveSpeed = 5f;
        Stats.HookSpeed = 40f;
        Stats.HookRange = 40f;
        Stats.JumpPower = 5f;
        Stats.BulletCount = 3;
        Stats.HookCooldown = 1f;
        Stats.HookCount = 5;
        Stats.CurHookCount.Value = Stats.HookCount;
        Stats.CurBulletCount.Value = Stats.BulletCount;
        HookPool = new ObjectPool(transform, _hookPrefab, Stats.HookCount);
        BulletPool = new ObjectPool(transform, _bulletPrefab, Stats.BulletCount);
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
        Stats.CurHp.Value -= damage;

        if (Stats.CurHp.Value <= 0)
        {
            Dying();
        }
    }

    private void Dying()
    {
        // TODO: �÷��̾ ������� �� ī�޶� �̵� �� ��� �ִϸ��̼� ����
        Stats.IsDied.Value = true;
    }

    // ���� 2�� �̻� ����Ǿ� ���� ���(�÷��̾ �ο� ����) �ڵ����� IsHookMove�� false��
    public IEnumerator CutHook()
    {
        yield return new WaitForSeconds(2f);
        IsHookMove = false;
        IsHooked = false;
        HookCoroutine = null;
    }

    private void SubscribedEvent()
    {

    }

    private void UnsubscribedEvent()
    {

    }
}
