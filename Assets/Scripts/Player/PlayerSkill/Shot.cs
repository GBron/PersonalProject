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
            Debug.Log($"{hit.collider.name}�� ����");
            if (hit.collider.gameObject.layer == 8)
            {
                // ������ �������� �� �� ����
            }
            else
            {
                // ���� �ƴ϶�� ���� ���� �Ѿ��� ����
                GameObject bullet = Instantiate(_bulletPrefab, hit.point + hit.normal, Quaternion.identity);
                bullet.transform.up = Vector3.up;
            }
        }
    }
}
