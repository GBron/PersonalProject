using CustomUtility.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class RecordData : SaveData
{
    [field: SerializeField] public float BestTime { get; set; }

    public RecordData()
    {

    }

    public RecordData(float bestTime)
    {
        BestTime = bestTime;
    }
}
public class BestRecord : MonoBehaviour
{
    [SerializeField] private TMP_Text _minute;
    [SerializeField] private TMP_Text _second;


    private float _bestTime;
    private RecordData _jsonSave;
    private RecordData _jsonLoad;


    private void Awake()
    {
        if (!File.Exists(Path.Combine(SaveHandle.BasePath, "RecordData.json")))
        {
            _bestTime = 100000f;
            SaveJson();
        }

        LoadJson();
    }

    private void Start()
    {
        if (_bestTime > GameManager.Instance.Timer)
        {
            _bestTime = GameManager.Instance.Timer;
            SaveJson();
        }

        _minute.text = ((int)(_bestTime / 60)).ToString();
        _second.text = (_bestTime % 60f).ToString("F2");
    }

    private void SaveJson()
    {
        _jsonSave = new(_bestTime);

        DataSaveController.Save(_jsonSave, SaveType.JSON);
    }

    private void LoadJson()
    {
        _jsonLoad = new();

        DataSaveController.Load(ref _jsonLoad, SaveType.JSON);

        _bestTime = _jsonLoad.BestTime;
    }
}
