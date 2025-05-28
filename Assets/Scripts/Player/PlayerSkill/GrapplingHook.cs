using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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


    private void HandleHook(bool value)
    {
        if (PlayerManager.Instance.IsHookMove) return;
        HookShoot();
    }
    // 1. �� �߻�
    private void HookShoot()
    {
        if (PlayerManager.Instance.CurHookCount.Value == 0) return;
        // 1-1. ī�޶��� �������� �߻� = _muzzle�� ī�޶��� �߾� ��������
        Hook hook = PlayerManager.Instance.GetHook();
        hook.transform.SetParent(null);
        hook.transform.position = _muzzle.position;
        hook.transform.rotation = _muzzle.rotation;
        hook.Rigid.velocity = _muzzle.forward * PlayerManager.Instance._stats.HookSpeed;
        hook.SetHookShot();
        // 1-2. ���� �߻��ϰ�, ���� ���� ������ �÷��̾ ������(hook, PlayerMovement���� ����)
        // 1-3. ���� ���� ���� ���ϸ� Ǯ�� ���ư�(hook���� ����)
        // 1-4. ���� �߻�Ǿ����� �� �������� �����ϰ� ��Ÿ���� ����
        PlayerManager.Instance.CurHookCount.Value--;
        // ������ ���� �߻�ÿ���.
        if(_hookCoroutine == null)
        {
            _hookCoroutine = StartCoroutine(HookCoolDown());
        }
    }

    IEnumerator HookCoolDown()
    {
        while (PlayerManager.Instance.CurHookCount.Value < PlayerManager.Instance._stats.HookCount)
        {
            yield return new WaitForSeconds(PlayerManager.Instance._stats.HookCooldown);
            PlayerManager.Instance.CurHookCount.Value++;
        }
        _hookCoroutine = null;
    }
}
