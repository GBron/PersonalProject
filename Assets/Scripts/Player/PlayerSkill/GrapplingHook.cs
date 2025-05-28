using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrapplingHook : MonoBehaviour
{
    // 플레이어가 바라보는 방향으로 훅 발사. 바라보는 방향 => 카메라 중앙
    // 훅은 사정거리만큼 날아가고 벽에 닿으면 위치로 플레이어가 끌려감
    // 벽에 도달시 해당 위치에 매달릴 수 있음
    // 중간에 취소는 불가능함.
    // 훅은 쿨타임을 가지고 3개의 충전량을 지님. 충전량이 최대가 아니면 쿨타임이 돌아가고 아니라면 쿨타임은 작동하지 않음.

    [SerializeField] private Transform _muzzle;

    // 구현 단계
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
    // 1. 훅 발사
    private void HookShoot()
    {
        // 1-1. 카메라의 정면으로 발사 = _muzzle은 카메라의 중앙 포지션임
        Hook hook = PlayerManager.Instance.GetHook();
        hook.transform.SetParent(null);
        hook.transform.position = _muzzle.position;
        hook.transform.rotation = _muzzle.rotation;
        hook.Rigid.velocity = _muzzle.forward * PlayerManager.Instance._stats.HookSpeed;
        hook.SetHookShot();
        // 1-2. 훅을 발사하고, 훅이 벽에 닿으면 플레이어를 끌어당김

        // 1-3. 훅이 벽에 닿지 못하면 풀로 돌아감(hook에서 구현)


    }
}
