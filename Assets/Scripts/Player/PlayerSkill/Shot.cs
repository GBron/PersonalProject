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
        if (Physics.Raycast(ray, out RaycastHit hit, 1000000f, _ignoreLayer) && PlayerManager.Instance.CurBulletCount.Value > 0)
        {
            Debug.Log($"{hit.collider.name}에 명중");
            if (hit.collider.gameObject.layer == 8)
            {
                // 적에게 명중했을 때 할 로직
            }
            else
            {
                // 적이 아니라면 닿은 곳에 총알을 생성
                PlayerManager.Instance.CurBulletCount.Value--;
                Bullet bullet = PlayerManager.Instance.GetBullet();
                bullet.transform.SetParent(null);
                bullet.transform.position = hit.point + hit.normal;
                bullet.transform.up = Vector3.up;
            }
        }
    }
}
