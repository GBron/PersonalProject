using UnityEngine;

public class GunSoundControl : MonoBehaviour
{
    [SerializeField] private AudioClip _shot;
    [SerializeField] private AudioClip _emptyShot;
    [SerializeField] private AudioSource _audioSource;

    private bool _needChangeClip = false;

    private void Start()
    {
        _audioSource.clip = _shot;
        InputManager.Instance.MouseLClick.Subscribe(PlaySound);
    }

    private void Update()
    {
        if (!_audioSource.isPlaying && _needChangeClip)
        {
            ChangeClip();
            _needChangeClip = false;
        }
        ClipCheck();
    }

    private void ClipCheck()
    {
        if (PlayerManager.Instance.Stats.CurBulletCount.Value < 1 && _audioSource.clip.Equals(_shot))
            _needChangeClip = true;
        else if (PlayerManager.Instance.Stats.CurBulletCount.Value > 0 && _audioSource.clip.Equals(_emptyShot))
            _needChangeClip = true;
    }

    private void OnDisable()
    {
        InputManager.Instance.MouseLClick.Unsubscribe(PlaySound);
    }

    private void ChangeClip()
    {
        if (PlayerManager.Instance.Stats.CurBulletCount.Value < 1)
            _audioSource.clip = _emptyShot;
        else
            _audioSource.clip = _shot;
    }

    private void PlaySound(bool value)
    {
        if (!_needChangeClip)
            _audioSource.Play();
    }
}
