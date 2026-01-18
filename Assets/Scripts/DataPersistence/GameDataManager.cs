using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Assets.Scripts.Cloud.Schemas;
using Unity.VisualScripting;
using System;

public class GameDataManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 20f;
    [SerializeField] private float loadTimeIntervalSeconds = 30f;

    public GameSODatabase gameSODatabase;

    public event Action OnDataLoaded;

    private GameData gameData = new GameData();
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;
    private Coroutine tryLoadGameCoroutine;


    public static GameDataManager instance { get; private set; }

    public bool IsDataLoaded { get; private set; }
    private bool isFetching = false;

    //save data
    private bool playerDataDirty = false;
    private bool farmlandDirty = false;
    private bool animalFarmDirty = false;
    private bool fishingDirty = false;
    private bool questDirty = false;


    public enum DataType
    {
        player,
        farmland,
        animals,
        fishing,
        quest
    }

    private void Awake() 
    {
        if (instance != null) 
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence) 
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        //  dataPersistenceObjects = FindAllDataPersistenceObjects();
        //this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        //InitializeSelectedProfileId();
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // GameEventsManager.instance.inputEvents.onLogInPress += LoadGame;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // GameEventsManager.instance.inputEvents.onLogInPress -= LoadGame;

    }

    public void AutoSaveActivate()
    {
        if (autoSaveCoroutine != null) 
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        if( scene.name != "Test") return;
        
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (IsDataLoaded && gameData != null)
        {
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                if(dataPersistenceObj != null) dataPersistenceObj.LoadData(gameData);
            }
            Debug.Log("Scene loaded: Distributed existing data to objects.");
        }
        else
        {
            if (tryLoadGameCoroutine != null) StopCoroutine(tryLoadGameCoroutine);
            tryLoadGameCoroutine = StartCoroutine(TryLoadData());
        }

        if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    // public void ChangeSelectedProfileId(string newProfileId) 
    // {
    //     // update the profile to use for saving and loading
    //     this.selectedProfileId = newProfileId;
    //     // load the game, which will use that profile, updating our game data accordingly
    //     LoadGame();
    // }

    // public void DeleteProfileData(string profileId) 
    // {
    //     // delete the data for this profile id
    //     dataHandler.Delete(profileId);
    //     // initialize the selected profile id
    //     InitializeSelectedProfileId();
    //     // reload the game so that our data matches the newly selected profile id
    //     LoadGame();
    // }

    // private void InitializeSelectedProfileId() 
    // {
    //     this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
    //     if (overrideSelectedProfileId) 
    //     {
    //         this.selectedProfileId = testSelectedProfileId;
    //         Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
    //     }
    // }

    // public void NewGame() 
    // {
    //     this.gameData = new GameData();
    // }

    public void LoadGame()
    {
        //// return right away if data persistence is disabled
        //if (disableDataPersistence) 
        //{
        //    return;
        //}

        //// load any saved data from a file using the data handler
        //this.gameData = dataHandler.Load(selectedProfileId);

        //// start a new game if the data is null and we're configured to initialize data for debugging purposes
        //if (this.gameData == null && initializeDataIfNull) 
        //{
        //    NewGame();
        //}

        //// if no data can be loaded, don't continue
        //if (this.gameData == null) 
        //{
        //    Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
        //    return;
        //}

        //// push the loaded data to all other scripts that need it
        //foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        //{
        //    dataPersistenceObj.LoadData(gameData);
        //}

        StartCoroutine(TryLoadData());
    }

    public IEnumerator TryLoadData()
    {
        if (IsDataLoaded) yield break;

        while (!IsDataLoaded)
        {
            if (!isFetching)
            {
                isFetching = true;
                Debug.Log("Staring loading data...");

                yield return CloudManager.Instance.Database.GetData(CloudManager.Instance.Auth.LocalId, (success, message, gameDataRes) =>
                {
                    if (success)
                    {
                        try
                        {
                            if (gameDataRes.ContainsKey(nameof(PlayerProfile)))
                                gameData.PlayerProfileData = (PlayerProfile)gameDataRes[nameof(PlayerProfile)];

                            if (gameDataRes.ContainsKey(nameof(PlayerData)))
                                gameData.PlayerDataData = (PlayerData)gameDataRes[nameof(PlayerData)];

                            if (gameDataRes.ContainsKey(nameof(Farmland)))
                                gameData.FarmlandData = (Farmland)gameDataRes[nameof(Farmland)];

                            if (gameDataRes.ContainsKey(nameof(AnimalFarm)))
                                gameData.AnimalFarmData = (AnimalFarm)gameDataRes[nameof(AnimalFarm)];

                            if (gameDataRes.ContainsKey(nameof(Fishing)))
                                gameData.FishingData = (Fishing)gameDataRes[nameof(Fishing)];

                            if (gameDataRes.ContainsKey("PlayerQuests"))
                                gameData.PlayerQuestsData = (List<PlayerQuest>)gameDataRes["PlayerQuests"];

                            if (dataPersistenceObjects != null)
                            {
                                foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
                                {
                                    if (dataPersistenceObj != null)
                                    {
                                        try
                                        {
                                            dataPersistenceObj.LoadData(gameData);
                                        }
                                        catch (System.Exception ex)
                                        {
                                            Debug.LogError($"Lỗi khi đẩy data vào {dataPersistenceObj}: {ex.Message}");
                                        }
                                    }
                                }
                            }

                            Debug.Log("Load data successful!");
                            OnDataLoaded?.Invoke();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Lỗi nghiêm trọng khi xử lý dữ liệu: " + e.Message);
                        }
                        finally
                        {
                            IsDataLoaded = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("Loading error from Firebase: " + message);
                        IsDataLoaded = true;
                    }

                    isFetching = false;
                });
            }

            if (!IsDataLoaded) yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("Data flow completed.");
    }


    // public void SaveGame()
    // {
    //     // return right away if data persistence is disabled
    //     if (disableDataPersistence) 
    //     {
    //         return;
    //     }

    //     // if we don't have any data to save, log a warning here
    //     if (this.gameData == null) 
    //     {
    //         Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
    //         return;
    //     }

    //     // pass the data to other scripts so they can update it
    //     foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
    //     {
    //         dataPersistenceObj.SaveData(gameData);
    //     }

    //     // timestamp the data so we know when it was last saved
    //     //gameData.lastUpdated = System.DateTime.Now.ToBinary();

    //     // save that data to a file using the data handler
    //     dataHandler.Save(gameData, selectedProfileId);
    // }


    IEnumerator TrySaveData()
    {
      

         if (!IsDataLoaded || this.gameData == null)
         {
             Debug.Log("Data is not loaded");

         }

        // if(!HasPendingChanges())
        // {
        //     Debug.Log("No changes to save");
        // }
        // else
        // {
            yield return SaveAllDirtyData();
        // }

    }


    public void MarkDirty(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.player: playerDataDirty = true; break;
            case DataType.farmland: farmlandDirty = true; break;
            case DataType.animals: animalFarmDirty = true; break;
            case DataType.fishing: fishingDirty = true; break;
            case DataType.quest: questDirty = true; break;

        }
    }

    public bool HasPendingChanges()
    {
        return playerDataDirty || farmlandDirty || animalFarmDirty || fishingDirty;
    }

    public IEnumerator SaveAllDirtyData()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            if (dataPersistenceObj == null || dataPersistenceObj.Equals(null)) 
            {
                continue; 
            }
            dataPersistenceObj.SaveData(gameData);
        }

        // if (playerDataDirty)
            yield return CloudManager.Instance.Database.SavePlayerData(CloudManager.Instance.Auth.LocalId, gameData.PlayerDataData, (success, message) =>
            {
                playerDataDirty = false;
                Debug.Log("Player Data save successful");
            });

        if (farmlandDirty)
            yield return CloudManager.Instance.Database.SaveFarmland(CloudManager.Instance.Auth.LocalId, gameData.FarmlandData, (success, message) =>
            {
                farmlandDirty = false;
                Debug.Log("Farmland Data save successful");
            });

        if (animalFarmDirty)
            yield return CloudManager.Instance.Database.SaveAnimalFarm(CloudManager.Instance.Auth.LocalId, gameData.AnimalFarmData, (success, message) =>
            {
                animalFarmDirty = false;
                Debug.Log("Animal Farm Data save successful");
            });

        if (fishingDirty)
            yield return CloudManager.Instance.Database.SaveFishing(CloudManager.Instance.Auth.LocalId, gameData.FishingData, (success, message) =>
            {
                fishingDirty = false;
                Debug.Log("Fishing Data save successful");
            });

        if (questDirty)
            yield return CloudManager.Instance.Database.SavePlayerQuest(CloudManager.Instance.Auth.LocalId, gameData.PlayerQuestsData, gameData.HandInQuestIds, (success, message) =>
            {
                questDirty = false;
                Debug.Log("Player Quest Data save successful");
            });

        Debug.Log("Cloud save completed!");
    }

    // CRITICAL EVENTS SAVE
    public IEnumerator ForceSaveAll()
    {
        Debug.Log("Force saving all data...");

        playerDataDirty = true;
        farmlandDirty = true;
        animalFarmDirty = true;
        fishingDirty = true;
        questDirty = true;

        yield return SaveAllDirtyData();

        Debug.Log("Force save completed!");
    }

    private void OnApplicationQuit() 
    {
        StartCoroutine(ForceSaveAll());
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
            // .OfType<IDataPersistence>();
        Debug.Log($"Found {dataPersistenceObjects.Count()} data persistence objects.");
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData() 
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() 
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave() 
    {
        while (true) 
        {   if(SceneManager.GetActiveScene().name != "Test")
            {
                yield return new WaitForSecondsRealtime(autoSaveTimeSeconds);
                continue;
            }
            yield return new WaitForSecondsRealtime(autoSaveTimeSeconds);
            yield return TrySaveData();
            Debug.Log("Auto Saved Game");
        }
    }
    public GameData GetGameData()
    {
        return gameData;
    }
}
