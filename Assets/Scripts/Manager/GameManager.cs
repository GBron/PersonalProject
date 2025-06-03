using Base;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class GameManager : Singleton<GameManager>
{
    // UI부분을 UI매니저로 빼내는걸 고려해야 할 것
    [field: SerializeField] public float MouseSensitivity { get; set; } = 1f;
    [SerializeField] private Canvas _hud;
    [SerializeField] private Canvas _barrier;
    [SerializeField] private Canvas _menu;
    [SerializeField] private Canvas _setting;
    [SerializeField] private Canvas _title;
    [SerializeField] private GameObject _quitConfirm;
    [SerializeField] private Camera _barriersCamera;

    private Dictionary<string, int> _sceneDir = new Dictionary<string, int>()
    {
        { "Title", 0 },
        { "Map1", 1 },
        { "Result", 2}
    };
    public int CurScene { get; private set; }
    public bool IsGamePaused { get; private set; }
    public float Timer { get; set; }
    public UnityEvent OnPlayerInit { get; set; }
    public int EnemyCount { get; set; }
    public bool IsTimeStop { get; set; }
    public bool CanClear { get; set; }

    

    private void Awake()
    {
        SingletonInit();
        if (_menu.gameObject.activeSelf == true) _menu.gameObject.SetActive(false);
        CurScene = 0;
        IsTimeStop = true;
        Timer = 0;
    }

    private void Start()
    {
        InputManager.Instance.ESCPress.Subscribe(OnMenuUI);
        InputManager.Instance.FPress.Subscribe(ClearCheat);
    }

    private void Update()
    {
        ClearCheck();
        SetTimer();
    }

    private void OnDisable()
    {
        InputManager.Instance.ESCPress.Unsubscribe(OnMenuUI);
        InputManager.Instance.FPress.Unsubscribe(ClearCheat);
    }

    public void ClearCheat(bool value)
    {
        EnemyCount = 0;
    }

    public void ChangeScene(int value)
    {
        CurScene = value;

        AsyncOperation async = SceneManager.LoadSceneAsync(CurScene);
        async.completed += SceneInit;
        EnemyCount = 0;
    }

    private void SceneInit(AsyncOperation async)
    {
        switch (CurScene)
        {
            case 0:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _title.gameObject.SetActive(true);
                _hud.gameObject.SetActive(false);
                _barrier.gameObject.SetActive(false);
                break;
            case 1:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerManager.Instance.PlayerStatReset();
                PlayerManager.Instance.IsHookMove = false;
                PlayerManager.Instance.IsHooked = false;
                IsTimeStop = false;
                IsGamePaused = false;
                CanClear = false;
                Timer = 0;
                _title.gameObject.SetActive(false);
                _hud.gameObject.SetActive(true);
                _barrier.gameObject.SetActive(true);
                PlayerManager.Instance.InstantiatePlayer();
                CameraStacking();
                // OnPlayerInit.Invoke();
                break;
            case 2:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _title.gameObject.SetActive(false);
                _hud.gameObject.SetActive(false);
                _barrier.gameObject.SetActive(false);
                break;
        }
    }

    private void ClearCheck()
    {
        if (CurScene == 1 && EnemyCount < 1)
        {
            CanClear = true;
        }
    }

    public void OnMenuUI(bool value)
    {
        if (CurScene != 1) return;

        Debug.Log("OnMenuUI 실행 됨!");

        if (_menu.gameObject.activeSelf == true)
        {
            _menu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            IsGamePaused = false;
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        _menu.gameObject.SetActive(true);
        IsGamePaused = true;
    }

    public void OnQuitConfirm()
    {
        if (_quitConfirm.activeSelf == true)
        {
            _quitConfirm.SetActive(false);
            return;
        }

        _quitConfirm.SetActive(true);
    }

    private void SetTimer()
    {
        if(!IsTimeStop)
            Timer += Time.deltaTime;
    }

    private void CameraStacking()
    {
        UniversalAdditionalCameraData mainCamData = Camera.main.GetUniversalAdditionalCameraData();
        mainCamData.cameraStack.Add(_barriersCamera);
    }

    public void GameStart()
    {
        ChangeScene(1);
    }

    public void GoTitle()
    {
        ChangeScene(0);

        _quitConfirm.SetActive(false);
        _menu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
