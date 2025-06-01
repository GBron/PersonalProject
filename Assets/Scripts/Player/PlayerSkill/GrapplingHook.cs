using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    // �÷��̾ �ٶ󺸴� �������� �� �߻�. �ٶ󺸴� ���� => ī�޶� �߾�
    // ���� �����Ÿ���ŭ ���ư��� ���� ������ ��ġ�� �÷��̾ ������
    // ���� ���޽� �ش� ��ġ�� �Ŵ޸� �� ����
    // �߰��� ��Ҵ� �Ұ�����.
    // ���� ��Ÿ���� ������ 3���� �������� ����. �������� �ִ밡 �ƴϸ� ��Ÿ���� ���ư��� �ƴ϶�� ��Ÿ���� �۵����� ����.

    [SerializeField] private Transform _muzzle;
    private Coroutine _hookCoroutine;

    // ���� �ܰ�
    private void Start()
    {
        InputManager.Instance.MouseRClick.Subscribe(HandleHook);
    }

    private void OnDestroy()
    {
        InputManager.Instance.MouseRClick.Unsubscribe(HandleHook);
    }

    private void Update()
    {
        ChargeHook();
    }

    private void HandleHook(bool value)
    {
        if (PlayerManager.Instance.IsHookMove) return;
        HookShoot();
    }
    // 1. �� �߻�
    private void HookShoot()
    {
        if (PlayerManager.Instance.Stats.CurHookCount.Value < 1) return;
        // 1-1. ī�޶��� �������� �߻� = _muzzle�� ī�޶��� �߾� ��������
        Hook hook = PlayerManager.Instance.GetHook();
        hook.SetHookShot(_muzzle.position, _muzzle.rotation);
        hook.Rigid.velocity = _muzzle.forward * 75f;
        // 1-2. ���� �߻��ϰ�, ���� ���� ������ �÷��̾ ������(hook, PlayerMovement���� ����)
        // 1-3. ���� ���� ���� ���ϸ� Ǯ�� ���ư�(hook���� ����)
        // 1-4. ���� �߻�Ǿ����� �� �������� �����ϰ� ��Ÿ���� ����
        PlayerManager.Instance.Stats.CurHookCount.Value--;

        // if(_hookCoroutine == null)
        // {
        //     _hookCoroutine = StartCoroutine(HookCoolDown());
        // }
    }

    // IEnumerator HookCoolDown()
    // {
    //     while (PlayerManager.Instance.Stats.CurHookCount.Value < PlayerManager.Instance.Stats.HookCount)
    //     {
    //         yield return new WaitForSeconds(PlayerManager.Instance.Stats.HookCooldown);
    //         PlayerManager.Instance.Stats.CurHookCount.Value++;
    //     }
    //     _hookCoroutine = null;
    // }

    private void ChargeHook()
    {
        // �� ������
        if (PlayerManager.Instance.Stats.CurHookCount.Value <= PlayerManager.Instance.Stats.HookCount)
        {
            // �������� ��Ÿ��
            PlayerManager.Instance.Stats.CurHookCount.Value += 1/3f * Time.deltaTime;
        }
    }
}
