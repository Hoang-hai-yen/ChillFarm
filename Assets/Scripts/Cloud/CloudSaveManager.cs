using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CloudSaveManager : MonoBehaviour
{
    private bool playerDataDirty = false;
    private bool farmlandDirty = false;
    private bool animalFarmDirty = false;
    private bool fishingDirty = false;

    private float autoSaveInterval = 20f;
    private float timeSinceLastSave = 0f;


    private GameDataManager gameDataManager;

    public enum DataType
    {
        player,
        farmland,
        animals,
        fishing
    }

    private CloudManager cloudManager;
    
    void Start()
    {
        gameDataManager = GetComponent<GameDataManager>();
        cloudManager = CloudManager.Instance;
    }

    void Update()
    {
        timeSinceLastSave += Time.deltaTime;
        //if (timeSinceLastSave > 5f && gameDataManager.IsDataLoaded)
        //{
        //    gameDataManager.PlayerDataData.Gold += 5;
        //    gameDataManager.FarmlandData.TotalPlotsUnlocked += 1;
        //    MarkDirty(DataType.player);
        //    MarkDirty(DataType.farmland);
        //}

        if (timeSinceLastSave >= autoSaveInterval && HasPendingChanges())
        {
            if (gameDataManager == null)
                Debug.Log("GameDataManager is null");
            else
            {
                StartCoroutine(SaveAllDirtyData());
                timeSinceLastSave = 0f;
            }
                
        }
    }

    public void MarkDirty(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.player: playerDataDirty = true; break;
            case DataType.farmland: farmlandDirty = true; break;
            case DataType.animals: animalFarmDirty = true; break;
            //case DataType.fishing: fishingDirty = true; break;
        }
    }

    public bool HasPendingChanges()
    {
        return playerDataDirty || farmlandDirty || animalFarmDirty || fishingDirty;
    }

    public IEnumerator SaveAllDirtyData()
    {

        if (playerDataDirty)
            yield return cloudManager.Database.SavePlayerData(CloudManager.Instance.Auth.LocalId, gameDataManager.PlayerDataData, (success, message) =>
            {
                playerDataDirty = false;
                Debug.Log("Player Data save successful");
            });

        if (farmlandDirty)
            yield return cloudManager.Database.SaveFarmland(CloudManager.Instance.Auth.LocalId, gameDataManager.FarmlandData,(success, message) =>
            {
                playerDataDirty = false;
                Debug.Log("Farmland Data save successful");
            });

        if (animalFarmDirty)
            yield return cloudManager.Database.SaveAnimalFarm(CloudManager.Instance.Auth.LocalId, gameDataManager.AnimalFarmData, (success, message) =>
            {
                playerDataDirty = false;
                Debug.Log("Animal Farm Data save successful");
            });

        //if (fishingDirty)
        //    yield return SaveFishing();

        Debug.Log("Cloud save completed!");
    }



    

   

    //public IEnumerator GetPlayerQuests(string status, Action<List<Quest>> callback)
    //{
    //    // Structured Query
    //    var queryData = new
    //    {
    //        structuredQuery = new
    //        {
    //            from = new[] { new { collectionId = "quests" } },
    //            where = new
    //            {
    //                compositeFilter = new
    //                {
    //                    op = "AND",
    //                    filters = new[]
    //                    {
    //                        new
    //                        {
    //                            fieldFilter = new
    //                            {
    //                                field = new { fieldPath = "userId" },
    //                                op = "EQUAL",
    //                                value = new { stringValue = userId }
    //                            }
    //                        },
    //                        new
    //                        {
    //                            fieldFilter = new
    //                            {
    //                                field = new { fieldPath = "status" },
    //                                op = "EQUAL",
    //                                value = new { stringValue = status }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    };

    //    string url = $"{FIREBASE_DB_URL}:runQuery";
    //    string jsonData = JsonConvert.SerializeObject(queryData);

    //    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    //    {
    //        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        request.SetRequestHeader("Authorization", $"Bearer {idToken}");

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            var results = JsonConvert.DeserializeObject<List<QueryResult>>(request.downloadHandler.text);
    //            List<Quest> quests = ParseQuests(results);
    //            callback?.Invoke(quests);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Query failed: {request.error}");
    //            callback?.Invoke(null);
    //        }
    //    }
    //}

    //public IEnumerator BatchWrite(List<BatchOperation> operations, Action<bool> callback)
    //{
    //    var writes = new List<object>();

    //    foreach (var op in operations)
    //    {
    //        string docPath = $"projects/{FIREBASE_PROJECT_ID}/databases/(default)/documents/{op.Collection}/{op.DocumentId}";

    //        if (op.Type == BatchOperationType.Update)
    //        {
    //            writes.Add(new
    //            {
    //                update = new
    //                {
    //                    name = docPath,
    //                    fields = op.FirestoreFields
    //                }
    //            });
    //        }
    //        else if (op.Type == BatchOperationType.Delete)
    //        {
    //            writes.Add(new
    //            {
    //                delete = docPath
    //            });
    //        }
    //    }

    //    var batchData = new { writes };

    //    string url = $"{FIREBASE_DB_URL}:commit";
    //    string jsonData = JsonConvert.SerializeObject(batchData);

    //    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    //    {
    //        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        request.SetRequestHeader("Authorization", $"Bearer {idToken}");

    //        yield return request.SendWebRequest();

    //        bool success = request.result == UnityWebRequest.Result.Success;
    //        callback?.Invoke(success);

    //        if (success)
    //            Debug.Log($"Batch write completed: {operations.Count} operations");
    //        else
    //            Debug.LogError($"Batch write failed: {request.error}");
    //    }
    //}



    // CRITICAL EVENTS SAVE
    public IEnumerator ForceSaveAll()
    {
        Debug.Log("Force saving all data...");

        playerDataDirty = true;
        farmlandDirty = true;
        animalFarmDirty = true;
        fishingDirty = true;

        yield return SaveAllDirtyData();

        Debug.Log("Force save completed!");
    }

    void OnApplicationQuit()
    {
        //StartCoroutine(ForceSaveAll());
    }
}


