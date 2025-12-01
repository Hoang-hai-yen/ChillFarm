using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class GameDataManager: MonoBehaviour
{

    public Farmland FarmlandData { get; set; }
    public AnimalFarm AnimalFarmData { get; set; }
    public PlayerData PlayerDataData { get; set; }
    public PlayerProfile PlayerProfileData { get; set; }
    public Fishing FishingData { get; set; }
    public bool IsDataLoaded { get; private set; }
    private bool isFetching = false;

    void Start()
    {
        TryLoadData();
    }

    void Update()
    {
        TryLoadData();
    }


    private void TryLoadData()
    {
        if (IsDataLoaded || isFetching || !CloudManager.Instance.Auth.IsLogin) return;

        isFetching = true; 
        Debug.Log("Bắt đầu tải dữ liệu...");

        StartCoroutine(CloudManager.Instance.Database.GetData(CloudManager.Instance.Auth.LocalId, (success, message, gameData) =>
        {
            if (success)
            {
                if (gameData.ContainsKey(nameof(PlayerProfile)))
                    PlayerProfileData = (PlayerProfile)gameData[nameof(PlayerProfile)];

                if (gameData.ContainsKey(nameof(PlayerData)))
                    PlayerDataData = (PlayerData)gameData[nameof(PlayerData)];

                if (gameData.ContainsKey(nameof(Farmland)))
                    FarmlandData = (Farmland)gameData[nameof(Farmland)];

                if (gameData.ContainsKey(nameof(AnimalFarm)))
                    AnimalFarmData = (AnimalFarm)gameData[nameof(AnimalFarm)];

                if (gameData.ContainsKey(nameof(Fishing)))
                    FishingData = (Fishing)gameData[nameof(Fishing)];


                IsDataLoaded = true;
                Debug.Log("Tải dữ liệu thành công!");
            }
            else
            {
                Debug.LogError("Lỗi tải: " + message);
            }

            isFetching = false;
        }));
    }
}


