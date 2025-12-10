using Assets.Scripts.Cloud.Schemas;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CloudManager: MonoBehaviour 
{

    //load data
    public Farmland FarmlandData { get; set; }
    public AnimalFarm AnimalFarmData { get; set; }
    public PlayerData PlayerDataData { get; set; }
    public PlayerProfile PlayerProfileData { get; set; }
    public Fishing FishingData { get; set; }
    public List<Assets.Scripts.Cloud.Schemas.Quest> QuestsData { get; set; } = new List<Assets.Scripts.Cloud.Schemas.Quest>();
    public List<PlayerQuest> PlayerQuestsData { get; set; } = new List<PlayerQuest>();
    public bool IsDataLoaded { get; private set; }
    private bool isFetching = false;

    //save data
    private bool playerDataDirty = false;
    private bool farmlandDirty = false;
    private bool animalFarmDirty = false;
    private bool fishingDirty = false;
    private bool questDirty = false;


    private float autoSaveInterval = 20f;
    private float timeSinceLastSave = 0f;

    //services
    public CloudAuthService Auth { get; private set; }
    public CloudDatabaseService Database { get; private set; }

    public enum DataType
    {
        player,
        farmland,
        animals,
        fishing,
        quest
    }

    public static CloudManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        ApiConfig apiConfig = ApiConfig.Instance;
        Instance.Auth = new CloudAuthService(apiConfig);
        Instance.Database = new CloudDatabaseService(apiConfig);
    }

    //private static CloudManager instance;
    //public static CloudManager Instance
    //{

    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = new CloudManager();
    //            ApiConfig apiConfig = ApiConfig.Instance;
    //            instance.Auth = new CloudAuthService(apiConfig);
    //            instance.Database = new CloudDatabaseService(apiConfig);
    //        }

    //        return instance;
    //    }

    //    private set
    //    {
    //        instance = value;
    //    }
    //}



    //void Start()
    //{
    //    TryLoadData();
    //    EventManager.OnDataDirtyAction += MarkDirty;
    //}

    //void Update()
    //{
    //    TryLoadData();
    //    TrySaveData();
       
    //}


    //// SAVE DATA METHODS
    //private void TrySaveData()
    //{
    //    timeSinceLastSave += Time.deltaTime;
    //    //if (timeSinceLastSave > 5f && gameDataManager.IsDataLoaded)
    //    //{
    //    //    gameDataManager.PlayerDataData.Gold += 5;
    //    //    gameDataManager.FarmlandData.TotalPlotsUnlocked += 1;
    //    //    MarkDirty(DataType.player);
    //    //    MarkDirty(DataType.farmland);
    //    //}

    //    if (timeSinceLastSave >= autoSaveInterval && HasPendingChanges())
    //    {
    //        if (!IsDataLoaded)
    //            Debug.Log("Data is not loaded");
    //        else
    //        {
    //            StartCoroutine(SaveAllDirtyData());
    //            timeSinceLastSave = 0f;
    //        }

    //    }
    //}
    

    //public void MarkDirty(DataType dataType)
    //{
    //    switch (dataType)
    //    {
    //        case DataType.player: playerDataDirty = true; break;
    //        case DataType.farmland: farmlandDirty = true; break;
    //        case DataType.animals: animalFarmDirty = true; break;
    //        case DataType.fishing: fishingDirty = true; break;
    //        case DataType.quest: questDirty = true; break;

    //    }
    //}

    //public bool HasPendingChanges()
    //{
    //    return playerDataDirty || farmlandDirty || animalFarmDirty || fishingDirty;
    //}

    //public IEnumerator SaveAllDirtyData()
    //{

    //    if (playerDataDirty)
    //        yield return Database.SavePlayerData(Auth.LocalId, PlayerDataData, (success, message) =>
    //        {
    //            playerDataDirty = false;
    //            Debug.Log("Player Data save successful");
    //        });

    //    if (farmlandDirty)
    //        yield return Database.SaveFarmland(Auth.LocalId, FarmlandData, (success, message) =>
    //        {
    //            farmlandDirty = false;
    //            Debug.Log("Farmland Data save successful");
    //        });

    //    if (animalFarmDirty)
    //        yield return Database.SaveAnimalFarm(Auth.LocalId, AnimalFarmData, (success, message) =>
    //        {
    //            animalFarmDirty = false;
    //            Debug.Log("Animal Farm Data save successful");
    //        });

    //    if (fishingDirty)
    //        yield return Database.SaveFishing(Auth.LocalId, FishingData, (success, message) =>
    //        {
    //            fishingDirty = false;
    //            Debug.Log("Fishing Data save successful");
    //        });

    //    if (questDirty)
    //        yield return Database.SavePlayerQuest(Auth.LocalId, PlayerQuestsData, (success, message) =>
    //        {
    //            questDirty = false;
    //            Debug.Log("Player Quest Data save successful");
    //        });

    //    Debug.Log("Cloud save completed!");
    //}

    //// CRITICAL EVENTS SAVE
    //public IEnumerator ForceSaveAll()
    //{
    //    Debug.Log("Force saving all data...");

    //    playerDataDirty = true;
    //    farmlandDirty = true;
    //    animalFarmDirty = true;
    //    fishingDirty = true;
    //    questDirty = true;

    //    yield return SaveAllDirtyData();

    //    Debug.Log("Force save completed!");
    //}

    //void OnApplicationQuit()
    //{
    //    //StartCoroutine(ForceSaveAll());
    //}



    //// LOAD DATA METHODS
    //private void TryLoadData()
    //{
    //    if (IsDataLoaded || isFetching || !Auth.IsLogin) return;

    //    isFetching = true;
    //    Debug.Log("Staring loading data...");

    //    StartCoroutine(Database.GetData(Auth.LocalId, (success, message, gameData) =>
    //    {
    //        if (success)
    //        {
    //            if (gameData.ContainsKey(nameof(PlayerProfile)))
    //                PlayerProfileData = (PlayerProfile)gameData[nameof(PlayerProfile)];

    //            if (gameData.ContainsKey(nameof(PlayerData)))
    //                PlayerDataData = (PlayerData)gameData[nameof(PlayerData)];

    //            if (gameData.ContainsKey(nameof(Farmland)))
    //                FarmlandData = (Farmland)gameData[nameof(Farmland)];

    //            if (gameData.ContainsKey(nameof(AnimalFarm)))
    //                AnimalFarmData = (AnimalFarm)gameData[nameof(AnimalFarm)];

    //            if (gameData.ContainsKey(nameof(Fishing)))
    //                FishingData = (Fishing)gameData[nameof(Fishing)];

    //            if (gameData.ContainsKey("Quests"))
    //                QuestsData = (List<Assets.Scripts.Cloud.Schemas.Quest>)gameData["Quests"];

    //            if (gameData.ContainsKey("PlayerQuests"))
    //                PlayerQuestsData = (List<PlayerQuest>)gameData["PlayerQuests"];


    //            IsDataLoaded = true;
    //            Debug.Log("Load data successful!");
    //        }
    //        else
    //        {
    //            Debug.LogError("Loading error: " + message);
    //        }

    //        isFetching = false;
    //    }));
    //}

    //private void OnDestroy()
    //{
    //    EventManager.OnDataDirtyAction -= MarkDirty;
    //}
}