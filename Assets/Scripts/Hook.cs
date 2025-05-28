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
            float distance = Vector3.Distance(Rigid.position, PlayerManager.Instance._player._center.position);

            if (distance > 30f)
            {
                ReturnPool();
                _isFlying = false;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 6)
        {
            _isFlying = false;
            PlayerManager.Instance.IsHooked = false;
            Rigid.velocity = Vector3.zero;
            hookDest = Rigid.position;
            ReturnPool();
            PlayerManager.Instance.IsHooked = true;
            PlayerManager.Instance.HookedEvent.Invoke(hookDest);
        }
    }

    public void SetHookShot()
    {
        _isFlying = true;
    }
}
