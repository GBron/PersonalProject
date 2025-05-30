using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
public class Hook : PooledObject
{
    public Rigidbody Rigid;
    public Vector3 hookDest;
    private bool _isFlying = false;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleHook();
    }

    private void HandleHook()
    {
        if (_isFlying)
        {
            float distance = Vector3.Distance(Rigid.position, PlayerManager.Instance.Player._center.position);
            if (distance > PlayerManager.Instance.Stats.HookRange)
            {
                _isFlying = false;
                Rigid.velocity = Vector3.zero;
                ReturnPool();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6)
        {
            _isFlying = false;
            Rigid.velocity = Vector3.zero;
            hookDest = Rigid.position;
            ReturnPool();
            PlayerManager.Instance.IsHooked = true;
            PlayerManager.Instance.HookedEvent.Invoke(hookDest);
        }
        else if (collision.gameObject.layer == 9)
        {
            _isFlying = false;
            Rigid.velocity = Vector3.zero;
            ReturnPool();
            collision.gameObject.GetComponent<EnemyController>()?.SetDetected();
            Debug.Log("적에게는 훅을 걸 수가 없습니다! 적이 당신을 발견했습니다!");
        }
    }

    public void SetHookShot(Vector3 startPos, Quaternion startRot)
    {
        Rigid.position = startPos;
        Rigid.rotation = startRot;
        _isFlying = true;
    }
}