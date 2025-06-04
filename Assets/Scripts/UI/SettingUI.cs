using CustomUtility.IO;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingData : SaveData
{
    [field: SerializeField] public int ScreenHeight { get; set; }
    [field: SerializeField] public int ScreenWidth { get; set; }
    [field: SerializeField] public bool IsFulled { get; set; }
    [field: SerializeField] public int Vsync { get; set; }
    [field: SerializeField] public int FPS { get; set; }
    [field: SerializeField] public float Volume { get; set; }

    public SettingData()
    {
    }

    public SettingData(int width, int height, bool full, int vsync, int fps, float volume)
    {
        ScreenWidth = width;
        ScreenHeight = height;
        IsFulled = full;
        Vsync = vsync;
        FPS = fps;
        Volume = volume;
    }
}

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject _screenBox;
    [SerializeField] private TMP_Text _curResol;
    [SerializeField] private TMP_Text _curFPS;
    [SerializeField] private Toggle _fullToggle;
    [SerializeField] private Slider _volumeSlider;

    private SettingData _jsonSave;
    private SettingData _jsonLoad;

    private int _screenHeight;
    private int _screenWidth;
    private bool _isFulled;
    private int _vsync;
    private int _fps;
    private float _volume;
    private Canvas _canvas;

    private void Awake()
    {
        Init();
    }

    private void OnDisable()
    {
        GameManager.Instance.Setting.onClick.RemoveListener(OpenSetting);
    }

    private void Init()
    {
        _canvas = GetComponent<Canvas>();
        GameManager.Instance.Setting.onClick.AddListener(OpenSetting);
        if (!File.Exists(Path.Combine(SaveHandle.BasePath, "SettingData.json")))
        {
            _screenHeight = 1080;
            _screenWidth = 1920;
            _vsync = 0;
            _fps = -1;
            _volume = 0.9f;
            _isFulled = true;
            SaveJson();
        }
        LoadJson();
        ApplySetting();
        _volumeSlider.value = _volume;
        _fullToggle.isOn = _isFulled;
        _curResol.text = $"{_screenWidth}x{_screenHeight}p";
        if (_vsync == 1)
            _curFPS.text = "Vsync";
        else
        {
            switch (_fps)
            {
                case -1:
                    _curFPS.text = "Unlimit";
                    break;
                case 60:
                case 144:
                    _curFPS.text = _fps.ToString();
                    break;
            }
        }
    }

    public void ApplySetting()
    {
        Screen.SetResolution(_screenWidth, _screenHeight, _isFulled);
        QualitySettings.vSyncCount = _vsync;
        Application.targetFrameRate = _fps;
        AudioListener.volume = _volume;
    }

    public void OnScreenBox()
    {
        if (_screenBox.activeSelf == true)
        {
            _screenBox.SetActive(false);
            return;
        }

        _screenBox.SetActive(true);
    }

    public void OpenSetting()
    {
        _canvas.enabled = true;
    }

    public void CloseSetting()
    {
        _canvas.enabled = false;
    }

    public void Set720p()
    {
        _screenWidth = 1280;
        _screenHeight = 720;
        _curResol.text = $"{_screenWidth}x{_screenHeight}p";
        OnScreenBox();
    }

    public void Set1080p()
    {
        _screenWidth = 1920;
        _screenHeight = 1080;
        _curResol.text = $"{_screenWidth}x{_screenHeight}p";
        OnScreenBox();
    }
    public void Set1440p()
    {
        _screenWidth = 2560;
        _screenHeight = 1440;
        _curResol.text = $"{_screenWidth}x{_screenHeight}p";
        OnScreenBox();
    }

    public void SetFPS60()
    {
        _fps = 60;
        _vsync = 0;
        _curFPS.text = _fps.ToString();
    }

    public void SetFPS144()
    {
        _fps = 144;
        _vsync = 0;
        _curFPS.text = _fps.ToString();
    }

    public void SetVsync()
    {
        _vsync = 1;
        _curFPS.text = "Vsync";
    }

    public void SetFPSUnlimit()
    {
        _fps = -1;
        _vsync = 0;
        _curFPS.text = "Unlimit";
    }

    public void SaveAndApply()
    {
        SaveJson();
        LoadJson();
        ApplySetting();
    }

    private void SaveJson()
    {
        _isFulled = _fullToggle.isOn;
        _volume = _volumeSlider.value;

        _jsonSave = new(_screenWidth, _screenHeight, _isFulled, _vsync, _fps, _volume);

        DataSaveController.Save(_jsonSave, SaveType.JSON);
    }

    private void LoadJson()
    {
        _jsonLoad = new();

        DataSaveController.Load(ref _jsonLoad, SaveType.JSON);

        _screenWidth = _jsonLoad.ScreenWidth;
        _screenHeight = _jsonLoad.ScreenHeight;
        _isFulled = _jsonLoad.IsFulled;
        _vsync = _jsonLoad.Vsync;
        _fps = _jsonLoad.FPS;
        _volume = _jsonLoad.Volume;
    }
}
