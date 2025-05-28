using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;

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
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"{hit.collider.name}에 명중");
            if (hit.collider.gameObject.layer == 8)
            {
                // 적에게 명중했을 때 할 로직
            }
            else
            {
                // 적이 아니라면 닿은 곳에 총알을 생성
                GameObject bullet = Instantiate(_bulletPrefab, hit.point + hit.normal, Quaternion.identity);
                bullet.transform.up = Vector3.up;
            }
        }
    }
}
