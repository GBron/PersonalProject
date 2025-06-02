using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

public class EnemyBullet : PooledObject
{
    [SerializeField] public Rigidbody Rigid;

    private void OnCollisionEnter(Collision collision)
    {
        Rigid.velocity = Vector3.zero;
        ReturnPool();

        if (collision.gameObject.layer == 3)
        {
            PlayerManager.Instance.TakeDamage(1);
        }
    }
}
