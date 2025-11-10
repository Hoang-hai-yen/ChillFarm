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
    public PlayerData PlayerData { get; set; }
    public PlayerProfile PlayerProfile { get; set; }
    public bool IsDataLoaded { get; private set; } 

    void Start()
    {
        IsDataLoaded = false;
         if(CloudManager.Instance.Auth.IsLogin)
        {
            StartCoroutine(CloudManager.Instance.Database.GetData(CloudManager.Instance.Auth.LocalId, (success, message, gameData) =>
            {
                PlayerProfile = (PlayerProfile)gameData["playerProfiles"];
                PlayerData = (PlayerData)gameData["playerData"];
                FarmlandData = (Farmland)gameData["farmlandData"];
                AnimalFarmData = (AnimalFarm)gameData["animalFarmData"];


                Debug.Log(message);

                if (success)
                    IsDataLoaded = true;
            }));
        }
    }

    void Update()
    {
        if (!IsDataLoaded && CloudManager.Instance.Auth.IsLogin)
        {
            StartCoroutine(CloudManager.Instance.Database.GetData(CloudManager.Instance.Auth.LocalId, (success, message, gameData) =>
            {
                PlayerProfile = (PlayerProfile)gameData["playerProfiles"];
                PlayerData = (PlayerData)gameData["playerData"];
                FarmlandData = (Farmland)gameData["farmlandData"];
                AnimalFarmData = (AnimalFarm)gameData["animalFarmData"];


                Debug.Log(message);

                if (success)
                    IsDataLoaded = true;
            }));
        }
    }
}

