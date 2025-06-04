using Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PooledObject
{
    private void Update()
    {
        if (!GameManager.Instance.InGame)
        {
            ReturnPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ������ ���� �Ѿ� ȸ��
        if (other.gameObject.layer == 3)
        {
            PlayerManager.Instance.Stats.CurBulletCount.Value++;
            ReturnPool();
        }
    }
}
