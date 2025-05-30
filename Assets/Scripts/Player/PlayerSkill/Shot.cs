using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;

    private LayerMask _ignoreLayer = ~((1 << 3) | (1 << 8) | (1 << 10));

    private void Start()
    {
        InputManager.Instance.MouseLClick.Subscribe(GunShot);
    }

    private void OnDestroy()
    {
        InputManager.Instance.MouseLClick.Unsubscribe(GunShot);
    }

    private void GunShot(bool value)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, _ignoreLayer) && PlayerManager.Instance.Stats.CurBulletCount.Value > 0)
        {
            if (hit.collider.gameObject.layer == 9)
            {
                // ������ �������� �� �� ����
                Debug.Log($"�� ����");
                hit.transform.GetComponent<EnemyController>()?.TakeDamage(1);
            }
            else
            {
                // ���� �ƴ϶�� ���� ���� �Ѿ��� ����
                PlayerManager.Instance.Stats.CurBulletCount.Value--;
                Bullet bullet = PlayerManager.Instance.GetBullet();
                bullet.transform.SetParent(null);
                bullet.transform.position = hit.point + hit.normal;
                bullet.transform.up = Vector3.up;
            }
        }
    }
}
