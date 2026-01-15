using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using static CloudDatabaseHelper;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Xml.Linq;
using System.Reflection;
using Assets.Scripts.Cloud.Schemas;

public class CloudDatabaseService
{
    private ApiConfig apiConfig;

  

    public CloudDatabaseService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
    }

    #region Token Refresh Wrapper

    /// <summary>
    /// Wrapper method để gửi authenticated request với auto token refresh
    /// </summary>
    /// <param name="createRequest">Factory function tạo UnityWebRequest</param>
    /// <param name="callback">Callback (success, message, responseBody)</param>
    /// <param name="retryCount">Số lần đã retry (internal use)</param>
    private IEnumerator SendAuthenticatedRequest(
        Func<UnityWebRequest> createRequest,
        Action<bool, string, string> callback,
        int retryCount = 0)
    {
        var auth = CloudManager.Instance.Auth;

        // 1. Proactive refresh nếu token sắp hết hạn
        if (auth.ShouldRefreshToken())
        {
            Debug.Log("[API] Token sắp hết hạn, đang refresh...");
            bool refreshSuccess = false;
            string refreshError = null;

            yield return auth.RefreshIdToken((success, msg) =>
            {
                refreshSuccess = success;
                if (!success) refreshError = msg;
            });

            if (!refreshSuccess)
            {
                Debug.LogWarning("[API] Proactive token refresh failed: " + refreshError);
                auth.ForceLogout();
                callback?.Invoke(false, "Session expired. Please login again.", null);
                yield break;
            }
        }

        // 2. Gửi request
        using (UnityWebRequest request = createRequest())
        {
            request.SetRequestHeader("Authorization", "Bearer " + auth.IdToken);
            yield return request.SendWebRequest();

            // 3. Xử lý response
            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "Success", request.downloadHandler.text);
            }
            else if (request.responseCode == 401 && retryCount == 0)
            {
                // Token expired - refresh và retry 1 lần
                Debug.Log("[API] Received 401, attempting token refresh...");
                bool refreshSuccess = false;
                string refreshError = null;

                yield return auth.RefreshIdToken((success, msg) =>
                {
                    refreshSuccess = success;
                    if (!success) refreshError = msg;
                });

                if (refreshSuccess)
                {
                    Debug.Log("[API] Token refreshed, retrying request...");
                    yield return SendAuthenticatedRequest(createRequest, callback, retryCount + 1);
                }
                else
                {
                    Debug.LogWarning("[API] Token refresh failed after 401: " + refreshError);
                    auth.ForceLogout();
                    callback?.Invoke(false, "Session expired. Please login again.", null);
                }
            }
            else
            {
                callback?.Invoke(false, request.error, request.downloadHandler?.text);
            }
        }
    }

    #endregion

    public IEnumerator CreateDocument(string collectionId, string documentId, string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";
        if (!String.IsNullOrWhiteSpace(documentId))
        {
            url += $"?documentId={documentId}";
        }

        yield return SendAuthenticatedRequest(
            () =>
            {
                var request = new UnityWebRequest(url, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            },
            (success, message, response) =>
            {
                callback?.Invoke(success, success ? "Create document successful" : message);
            }
        );
    }

    public IEnumerator GetCollection(string collectionId, Action<bool, string, FirestoreListResponse> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";

        // Nếu chưa login, gọi trực tiếp không cần auth
        if (!CloudManager.Instance.Auth.IsLogin)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var responseObj = JsonConvert.DeserializeObject<FirestoreListResponse>(request.downloadHandler.text);
                    callback?.Invoke(true, "Success", responseObj);
                }
                else if (request.responseCode == 404)
                {
                    callback?.Invoke(true, "Empty Collection", new FirestoreListResponse { Documents = new List<FirestoreFound>() });
                }
                else
                {
                    callback?.Invoke(false, request.error, null);
                }
            }
            yield break;
        }

        // Đã login - sử dụng wrapper với auto refresh
        yield return SendAuthenticatedRequest(
            () => UnityWebRequest.Get(url),
            (success, message, response) =>
            {
                if (success)
                {
                    var responseObj = JsonConvert.DeserializeObject<FirestoreListResponse>(response);
                    callback?.Invoke(true, "Success", responseObj);
                }
                else if (message.Contains("404"))
                {
                    callback?.Invoke(true, "Empty Collection", new FirestoreListResponse { Documents = new List<FirestoreFound>() });
                }
                else
                {
                    callback?.Invoke(false, message, null);
                }
            }
        );
    }

    public IEnumerator PatchDocument(string documentPath, string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{documentPath}";

        yield return SendAuthenticatedRequest(
            () =>
            {
                var request = new UnityWebRequest(url, "PATCH");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            },
            (success, message, response) =>
            {
                callback?.Invoke(success, success ? "Patch document successful" : message);
            }
        );
    }

    public IEnumerator PatchDocument(string documentPath, string jsonData, string query, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{documentPath}?{query}";

        yield return SendAuthenticatedRequest(
            () =>
            {
                var request = new UnityWebRequest(url, "PATCH");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            },
            (success, message, response) =>
            {
                callback?.Invoke(success, success ? "Patch document successful" : message);
            }
        );
    }

    public IEnumerator UpdateField(string fieldPath, object value, string valueType)
    {
        var firestoreData = new
        {
            fields = new Dictionary<string, object>
            {
                { fieldPath, CloudDatabaseHelper.CreateFirestoreValue(value, valueType) },
            }
        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument(fieldPath, jsonData, "updateMask.fieldPaths={fieldName}", (success, message) =>
        {
            if (success)
                Debug.Log($"Updated field: {fieldPath}");
            else
                Debug.Log(message);
        });
    }

    public IEnumerator BatchGet(string jsonData, Action<bool, string, List<FirestoreBatchGetResponse>> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents:batchGet";

        yield return SendAuthenticatedRequest(
            () =>
            {
                var request = new UnityWebRequest(url, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            },
            (success, message, response) =>
            {
                if (success)
                {
                    List<FirestoreBatchGetResponse> list = null;
                    try
                    {
                        list = JsonConvert.DeserializeObject<List<FirestoreBatchGetResponse>>(response);
                        callback?.Invoke(true, "Batch get successful!", list);
                    }
                    catch (Exception e)
                    {
                        Exception realError = e.InnerException != null ? e.InnerException : e;
    
                        string detailedMessage = $"Message: {realError.Message}\n" +
                             $"Stack Trace: {realError.StackTrace}";

                        Debug.LogError("FULL ERROR: " + detailedMessage);
                        callback?.Invoke(false, "Parse error: " + realError.Message, null);
                    }
                }
                else
                {
                    callback?.Invoke(false, message, null);
                }
            }
        );
    }

    private IEnumerator BatchWrite(string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents:commit?key={apiConfig.ApiKey}";

        yield return SendAuthenticatedRequest(
            () =>
            {
                var request = new UnityWebRequest(url, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                return request;
            },
            (success, message, response) =>
            {
                callback?.Invoke(success, success ? "BatchWrite successful!" : message);
            }
        );
    }


    public IEnumerator CreateInitialData(string playerId, string email, string name, string avatar, Action<bool, string> callback)
    {
        List<object> updateDocs = new List<object>();
        updateDocs.Add(new
        {
            update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerProfiles/{playerId}", new PlayerProfile {UserId = playerId, Name = name, Email = email, Avatar = avatar }, true)
        }); 
        updateDocs.Add(new
        {
            update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}", InitialDataFactory.CreatePlayerData(playerId), true)
        });
        updateDocs.Add(new
        {
            update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/farmlandData", InitialDataFactory.CreateFarmlandData(playerId), true)
        });
        updateDocs.Add(new
        {
            update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/animalFarmData", InitialDataFactory.CreateAnimalFarmData(playerId), true)
        });
        // updateDocs.Add(new
        // {
        //     update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/fishingData", InitialDataFactory.CreateFishingData(playerId), true)
        // });

        var firestoreData = new
        {
            writes = updateDocs,

        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);
        //Debug.Log(jsonData);

        yield return BatchWrite(jsonData, callback);

        //yield return CreatePlayerProfile(playerId, email, name, avatar, callback);
        //yield return CreatePlayerData(playerId, callback);
        //yield return CreateFarmland(playerId, callback);
        //yield return CreateAnimalFarm(playerId, callback);
        Debug.Log("Create initial data completed");
    }

    public IEnumerator CreatePlayerQuest(string playerId, PlayerQuest playerQuest, Action<bool, string> callback)
    {


        var firestoreData = FirestoreSerializer.ToFirestoreDocument(playerQuest, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return CreateDocument($"playerProfiles/{playerId}/playerQuestData/", playerQuest.QuestId, jsonData, callback);
    }

    public IEnumerator SavePlayerProfile(string playerId, PlayerProfile playerProfile, Action<bool, string> callback)
    {
    

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(playerProfile, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerProfiles/{playerId}", jsonData, callback);
    }


    public IEnumerator SavePlayerData(string playerId, PlayerData playerData,Action<bool, string> callback)
    {

   

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(playerData, true);

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{CloudManager.Instance.Auth.LocalId}", jsonData, callback);
    }

    public IEnumerator SaveFarmland(string playerId, Farmland farmland, Action<bool, string> callback)
    {

 

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(farmland, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/farmlandData", jsonData, callback);
    }

    /// <summary>
    /// Save animal farm
    /// </summary>
    public IEnumerator SaveAnimalFarm(string playerId, AnimalFarm animalFarm,Action<bool, string> callback)
    {
  

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(animalFarm, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/animalFarmData", jsonData, callback);
    }

    public IEnumerator SaveFishing(string playerId, Fishing fishing, Action<bool, string> callback)
    {

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(fishing, true);

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/fishingData", jsonData, callback);
    }

    // not done
    public IEnumerator SavePlayerQuest(string playerId, List<PlayerQuest> activeQuests, List<string> handInQuestIds, Action<bool, string> callback)
    {

        List<object> updateDocs = new List<object>();

        string basePath = $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/playerQuestData";    
        foreach (var playerQuest in activeQuests) 
        {
            // if (playerQuest.IsChanged)
            // {
                updateDocs.Add(new {
                    update = FirestoreSerializer.ToFirestoreBatchDocument($"{basePath}/{playerQuest.QuestId}", playerQuest, true)

                });
            // }
        }

        foreach (var questId in handInQuestIds)
        {
            updateDocs.Add(new
            {
                delete = $"{basePath}/{questId}"
            });
        }

        if (updateDocs.Count == 0)
        {
            callback?.Invoke(true, "No changes to save");
            yield break;
        }

        var firestoreData = new
        {
            writes = updateDocs,

        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return BatchWrite(jsonData, callback);
    }

    public IEnumerator GetData(string playerId, Action<bool, string, Dictionary<string, object>> callback)
    {
        //    List<string> documentPaths = new List<string>
        //{
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerProfiles/{playerId}",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/farmlandData",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/animalFarmData",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/fishingData",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/playerQuestData",
        //    $"projects/{apiConfig.ProjectId}/databases/(default)/documents/quests",
        //};

        //    var firestoreData = new { documents = documentPaths };
        //    string jsonData = JsonConvert.SerializeObject(firestoreData);

        //    // Gọi BatchGet (API: documents:batchGet)
        //    yield return BatchGet(jsonData, (success, message, firestoreBatchGetResponses) =>
        //    {
        //        if (!success || firestoreBatchGetResponses == null)
        //        {
        //            callback?.Invoke(false, message, null);
        //            return;
        //        }

        //        Dictionary<string, object> gameData = new Dictionary<string, object>();

        //        foreach (FirestoreBatchGetResponse response in firestoreBatchGetResponses)
        //        {
        //            // Kiểm tra xem document có tồn tại không (nếu null là chưa tạo)
        //            if (response.Found == null) continue;

        //            string fullPath = response.Found.Name;

        //            // LOGIC NHẬN DIỆN: Dựa vào đuôi của đường dẫn hoặc từ khóa đặc trưng
        //            // Dùng nameof() để Key của Dictionary trùng khít với tên Class C#

        //            // 1. Check các Subcollection trước (Do đường dẫn dài hơn và cụ thể hơn)
        //            if (fullPath.EndsWith("farmlandData"))
        //            {
        //                gameData[nameof(Farmland)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Farmland>(response.Found.Fields);
        //            }
        //            else if (fullPath.EndsWith("animalFarmData"))
        //            {
        //                gameData[nameof(AnimalFarm)] = CloudDatabaseHelper.FirestoreMapper.ToObject<AnimalFarm>(response.Found.Fields);
        //            }
        //            else if (fullPath.EndsWith("fishingData"))
        //            {
        //                gameData[nameof(Fishing)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Fishing>(response.Found.Fields);
        //            }
        //            // 2. Check các Root Collection (Chứa ID)
        //            else if (fullPath.Contains("/playerProfiles/"))
        //            {
        //                gameData[nameof(PlayerProfile)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerProfile>(response.Found.Fields);
        //            }
        //            // 3. Check PlayerData (Lưu ý: Phải check cuối cùng hoặc đảm bảo không nhầm với subcollection ở trên)
        //            else if (fullPath.Contains("/playerData/"))
        //            {
        //                // Fix lỗi copy-paste cũ: Map vào PlayerData chứ không phải PlayerProfile
        //                gameData[nameof(PlayerData)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerData>(response.Found.Fields);
        //            }
        //        }

        //        callback?.Invoke(true, "Load data success", gameData);
        //    });


        Dictionary<string, object> gameData = new Dictionary<string, object>();

        // --- PHẦN 1: BATCH GET (Lấy Document lẻ) ---
        List<string> singleDocs = new List<string>
    {
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerProfiles/{playerId}",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/farmlandData",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/animalFarmData",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/fishingData"
    };

        var firestoreData = new { documents = singleDocs };
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        bool batchSuccess = false;
        // Gọi BatchGet
        yield return BatchGet(jsonData, (success, message, responses) =>
        {
            Debug.Log(message);
            batchSuccess = success;
            if (!success) return;

            foreach (var res in responses)
            {
                if (res.Found == null) continue;
                string path = res.Found.Name;

                if (path.EndsWith("farmlandData")) gameData[nameof(Farmland)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Farmland>(res.Found.Fields);
                else if (path.EndsWith("animalFarmData")) gameData[nameof(AnimalFarm)] = CloudDatabaseHelper.FirestoreMapper.ToObject<AnimalFarm>(res.Found.Fields);
                else if (path.EndsWith("fishingData")) gameData[nameof(Fishing)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Fishing>(res.Found.Fields);
                else if (path.Contains("/playerProfiles/")) gameData[nameof(PlayerProfile)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerProfile>(res.Found.Fields);
                else if (path.Contains("/playerData/")) gameData[nameof(PlayerData)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerData>(res.Found.Fields);
            }
        });

        if (!batchSuccess)
        {
            callback?.Invoke(false, "Failed to load single docs", null);
            yield break;
        }

        // --- PHẦN 2: GET LIST QUESTS (Static Config) ---
        // Đường dẫn collection (tương đối): projects/.../documents/quests
        // string questsPath = $"projects/{apiConfig.ProjectId}/databases/(default)/documents/quests";

        // yield return GetCollection(questsPath, (success, msg, response) =>
        // {
        //     if (success)
        //     {
        //         // Mapping thành List<Quest>
        //         List<Assets.Scripts.Cloud.Schemas.Quest> questList = CloudDatabaseHelper.FirestoreMapper.MapCollectionToList<Assets.Scripts.Cloud.Schemas.Quest>(response);
        //         gameData["Quests"] = questList; 
        //     }
        // });

        // --- PHẦN 3: GET LIST PLAYER QUESTS (User Progress) ---
        string userQuestPath = $"playerData/{playerId}/playerQuestData";

        yield return GetCollection(userQuestPath, (success, msg, response) =>
        {
            if (success)
            {
                // Mapping thành List<PlayerQuest>
                List<PlayerQuest> userQuests = CloudDatabaseHelper.FirestoreMapper.MapCollectionToList<PlayerQuest>(response);
                // Debug.Log("Loaded " + userQuests.Count + " player quests from database.");
                gameData["PlayerQuests"] = userQuests; 
            }
        });

        // --- HOÀN TẤT ---
        callback?.Invoke(true, "All data loaded", gameData);
    }

    /// <summary>
    /// Load farmland
    /// </summary>
    //public IEnumerator LoadFarmland(Action<FarmlandData> callback)
    //{
    //    string url = $"{FIREBASE_DB_URL}/farmland/{userId}";

    //    using (UnityWebRequest request = UnityWebRequest.Get(url))
    //    {
    //        request.SetRequestHeader("Authorization", $"Bearer {idToken}");

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            var response = JsonConvert.DeserializeObject<FirestoreDocument>(request.downloadHandler.text);
    //            FarmlandData data = ParseFarmlandData(response);
    //            callback?.Invoke(data);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Load farmland failed: {request.error}");
    //            callback?.Invoke(null);
    //        }
    //    }
    //}
    //public IEnumerator SaveFishing()
    //{
    //    
    //    fishingDirty = false;
    //    yield return null;
    //}

}