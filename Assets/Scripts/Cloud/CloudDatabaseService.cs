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

public class CloudDatabaseService
{
    private ApiConfig apiConfig;

    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string Scene { get; set; }

        public Position(float x, float y, string scene)
        {
            X = x;
            Y = y;
            Scene = scene;
        }
    }

    public class Time
    {
        public string CurrentTime { get; set; } //hh:mm
        public int Day { get; set; }

        public Time(string currentTime, int day)
        {
            CurrentTime = currentTime;
            Day = day;
        }
    }

    public CloudDatabaseService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
    }

    public IEnumerator CreateDocument(string collectionId, string documentId, string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";
        if (!String.IsNullOrWhiteSpace(documentId))
        {
            url += $"?documentId={documentId}";
        }

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + CloudManager.Instance.Auth.IdToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "Create document successful");
            }
            else
            {
                callback?.Invoke(false, request.error);
            }
        }

        
    }

    public IEnumerator PatchDocument(string documentPath, string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{documentPath}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + CloudManager.Instance.Auth.IdToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "Patch document successful");
            }
            else
            {
                callback?.Invoke(false, request.error);
            }
        }
    }

    public IEnumerator PatchDocument(string documentPath, string jsonData, string query, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{documentPath}?{query}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + CloudManager.Instance.Auth.IdToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "Patch document successful");
            }
            else
            {
                callback?.Invoke(false, request.error);
            }
        }
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

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + CloudManager.Instance.Auth.IdToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                List<FirestoreBatchGetResponse> list = null;

                try
                {
                    list = JsonConvert.DeserializeObject<List<FirestoreBatchGetResponse>>(response);
                }
                catch (Exception e)
                {
                    callback?.Invoke(false,
                        "Parse error: " + e.Message + "\nRAW: " + response,
                        null);
                    yield break;
                }

                callback?.Invoke(true, "Batch get successful!", list);
            }
            else
            {
                callback?.Invoke(false, request.downloadHandler.text, null);
            }
        }
    }

    private IEnumerator BatchWrite(string jsonData, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents:commit?key={apiConfig.ApiKey}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log(jsonData);
            request.SetRequestHeader("Authorization", "Bearer " + CloudManager.Instance.Auth.IdToken);

            yield return request.SendWebRequest();


            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "BatchWrite successful!");
            }
            else
            {
                callback?.Invoke(false, request.error + request.downloadHandler.text);
            }
        }
    }

    //public IEnumerator CreatePlayerProfile(string playerId, string email, string name, string avatar, Action<bool, string> callback)
    //{
    //    var firestoreData = new
    //    {
    //        fields = new
    //        {
    //            email = new { stringValue = email },
    //            name = new { stringValue = name },
    //            avatar = new { stringValue = avatar },
               
    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //        }
    //    };
    //    string jsonData = JsonConvert.SerializeObject(firestoreData);

    //    yield return CreateDocument("playerProfiles", playerId, jsonData, callback);
    //}

    //public IEnumerator CreatePlayerData(string playerId, Action<bool, string> callback)
    //{
    //    var firestoreData = new
    //    {
    //        fields = new
    //        {
    //            userId = new { stringValue = playerId },

    //            gold = new { integerValue = "0" },
    //            stamina = new { doubleValue = 0d },
    //            maxStamina = new { doubleValue = 1d},
    //            currentDay = new { integerValue = "1" },
    //            currentTime = new { integerValue = "0" },

    //            position = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        x = new { doubleValue = 0d },
    //                        y = new { doubleValue = 0d },
    //                        scene = new { stringValue = "" }
    //                    }
    //                }
    //            },

    //            experiencePoint = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        skill = CloudDatabaseHelper.CreateXPField(),
    //                        fishing = CloudDatabaseHelper.CreateXPField(),
    //                        animal = CloudDatabaseHelper.CreateXPField(),
    //                        farming = CloudDatabaseHelper.CreateXPField()
    //                    }
    //                }
    //            },

    //            inventory = CloudDatabaseHelper.CreateInventoryField(),
    //            storage = CloudDatabaseHelper.CreateInventoryField(),

    //            //tools = new
    //            //{
    //            //    mapValue = new
    //            //    {
    //            //        fields = new
    //            //        {
    //            //            hoe = new { stringValue = playerData.Tools.Hoe ?? "" },
    //            //            wateringCan = new { stringValue = playerData.Tools.WateringCan ?? "" },
    //            //            fishingRod = new { stringValue = playerData.Tools.FishingRod ?? "" },
    //            //            shovel = new { stringValue = playerData.Tools.Shovel ?? "" }
    //            //        }
    //            //    }
    //            //},

    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //        }
    //    };
    //    string jsonData = JsonConvert.SerializeObject(firestoreData);

    //    yield return CreateDocument("playerData", playerId, jsonData, callback);

    //}

    //public IEnumerator CreateFarmland (string playerId, Action<bool, string> callback)
    //{

    //    var firestoreData = new
    //    {
    //        fields = new
    //        {
    //            userId = new { stringValue = playerId },

    //            plots = new { arrayValue = new { values = Array.Empty<object>() } },
    //            totalPlotsUnlocked = new { integerValue = "0" },
    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //        }
    //    };

    //    string jsonData = JsonConvert.SerializeObject(firestoreData);

    //    yield return CreateDocument($"farmlandData", playerId, jsonData, callback);
    //}

    //public IEnumerator CreateAnimalFarm(string playerId, Action<bool, string> callback)
    //{

    //    var firestoreData = new
    //    {
    //        fields = new
    //        {
    //            userId = new { stringValue = playerId },

    //            farmLevel = new { integerValue = "0" },
    //            maxCapacity = new { integerValue = "0" },
    //            //hasAutoMilker = new { booleanValue = animalFarm.HasAutoMilker },
    //            //hasAutoIncubator = new { booleanValue = animalFarm.HasAutoIncubator },
    //            animals = new { arrayValue = new { values = Array.Empty<object>() } },
    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //        }
    //    };

    //    string jsonData = JsonConvert.SerializeObject(firestoreData);

    //    yield return CreateDocument($"animalFarmData", playerId, jsonData, callback);
    //}


    //public object CreatePlayerProfileObject(string playerId, string email, string name, string avatar)
    //{
    //   return new
    //    {
            
    //            email = new { stringValue = email },
    //            name = new { stringValue = name },
    //            avatar = new { stringValue = avatar },

    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
            
    //    };

    //}

    //public object CreatePlayerDataObject(string playerId)
    //{
    //    return new
    //    {
    //        userId = new { stringValue = playerId },

    //        gold = new { integerValue = "0" },
    //            stamina = new { doubleValue = 0d },
    //            maxStamina = new { doubleValue = 1d },
    //            currentDay = new { integerValue = "1" },
    //            currentTime = new { integerValue = "0" },

    //            position = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        x = new { doubleValue = 0d },
    //                        y = new { doubleValue = 0d },
    //                        scene = new { stringValue = "" }
    //                    }
    //                }
    //            },

    //            experiencePoint = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        skill = CloudDatabaseHelper.CreateXPField(),
    //                        fishing = CloudDatabaseHelper.CreateXPField(),
    //                        animal = CloudDatabaseHelper.CreateXPField(),
    //                        farming = CloudDatabaseHelper.CreateXPField()
    //                    }
    //                }
    //            },

    //            inventory = CloudDatabaseHelper.CreateInventoryField(),
    //            storage = CloudDatabaseHelper.CreateInventoryField(),

    //            //tools = new
    //            //{
    //            //    mapValue = new
    //            //    {
    //            //        fields = new
    //            //        {
    //            //            hoe = new { stringValue = playerData.Tools.Hoe ?? "" },
    //            //            wateringCan = new { stringValue = playerData.Tools.WateringCan ?? "" },
    //            //            fishingRod = new { stringValue = playerData.Tools.FishingRod ?? "" },
    //            //            shovel = new { stringValue = playerData.Tools.Shovel ?? "" }
    //            //        }
    //            //    }
    //            //},

    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
            
    //    };

    //}

    //public object CreateFarmlandObject(string playerId)
    //{

    //    return new
    //    {
    //        userId = new { stringValue = playerId },

    //        plots = new { arrayValue = new { values = Array.Empty<object>() } },
    //            totalPlotsUnlocked = new { integerValue = "0" },
    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //    };


    //}

    //public object CreateAnimalFarmObject(string playerId)
    //{

    //    return new
    //    {
    //        userId = new { stringValue = playerId },

    //        farmLevel = new { integerValue = "0" },
    //            maxCapacity = new { integerValue = "0" },
    //            //hasAutoMilker = new { booleanValue = animalFarm.HasAutoMilker },
    //            //hasAutoIncubator = new { booleanValue = animalFarm.HasAutoIncubator },
    //            animals = new { arrayValue = new { values = Array.Empty<object>() } },
    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }

    //    };


    //}

    //public object CreateFishingObject(string playerId)
    //{ 

    //    return new
    //    {
            
    //            userId = new { stringValue = playerId },

    //            // Pond data
    //            pond = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        isUnlocked = new { booleanValue = false },
    //                        maxCapacity = new { integerValue = "10" },
    //                        fish = new { arrayValue = new { values = Array.Empty<object>() } }
    //                    }
    //                }
    //            },

    //            // Fishing statistics
    //            stats = new
    //            {
    //                mapValue = new
    //                {
    //                    fields = new
    //                    {
    //                        totalCaught = new { integerValue = "0" },
    //                        biggestFish = new { nullValue = (object)null }

    //                    }
    //                }
    //            },

    //            updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //    };


    //}


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
        updateDocs.Add(new
        {
            update = FirestoreSerializer.ToFirestoreBatchDocument($"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/fishingData", InitialDataFactory.CreateFishingData(playerId), true)
        });

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

    public IEnumerator SavePlayerProfile(string playerId, PlayerProfile playerProfile, Action<bool, string> callback)
    {
        //var firestoreData = new
        //{
        //    fields = new
        //    {
        //        email = new { stringValue = email },
        //        name = new { stringValue = name },
        //        avatar = new { stringValue = avatar },

        //        updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
        //    }
        //};

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(playerProfile, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerProfiles/{playerId}", jsonData, callback);
    }


    public IEnumerator SavePlayerData(string playerId, PlayerData playerData,Action<bool, string> callback)
    {

        //var firestoreData = new
        //{
        //    fields = new
        //    {
        //        userId = new { stringValue = playerId },

        //        gold = new { integerValue = playerData.Gold.ToString() },
        //        stamina = new { doubleValue = playerData.Stamina },
        //        maxStamina = new { doubleValue = playerData.MaxStamina },
        //        currentDay = new { integerValue = playerData.CurrentDay.ToString() },
        //        currentTime = new { integerValue = playerData.CurrentTime.ToString() },

        //        position = new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    x = new { doubleValue = playerData.Position.X },
        //                    y = new { doubleValue = playerData.Position.Y },
        //                    scene = new { stringValue = playerData.Position.CurrentScene }
        //                }
        //            }
        //        },

        //        experiencePoint = new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    skill = CloudDatabaseHelper.CreateXPField(playerData.ExperiencePoint.SkillXP),
        //                    fishing = CloudDatabaseHelper.CreateXPField(playerData.ExperiencePoint.FishingXP),
        //                    animal = CloudDatabaseHelper.CreateXPField(playerData.ExperiencePoint.AnimalXP),
        //                    farming = CloudDatabaseHelper.CreateXPField(playerData.ExperiencePoint.FarmingXP)
        //                }
        //            }
        //        },

        //        inventory = CloudDatabaseHelper.CreateInventoryField(playerData.Inventory),
        //        storage = CloudDatabaseHelper.CreateInventoryField(playerData.Storage),

        //        //tools = new
        //        //{
        //        //    mapValue = new
        //        //    {
        //        //        fields = new
        //        //        {
        //        //            hoe = new { stringValue = playerData.Tools.Hoe ?? "" },
        //        //            wateringCan = new { stringValue = playerData.Tools.WateringCan ?? "" },
        //        //            fishingRod = new { stringValue = playerData.Tools.FishingRod ?? "" },
        //        //            shovel = new { stringValue = playerData.Tools.Shovel ?? "" }
        //        //        }
        //        //    }
        //        //},

        //        updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
        //    }
        //};

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(playerData, true);

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{CloudManager.Instance.Auth.LocalId}", jsonData, callback);
    }

    public IEnumerator SaveFarmland(string playerId, Farmland farmland, Action<bool, string> callback)
    {

        //var plotsArray = new List<object>();

        //foreach (var plot in farmland.Plots)
        //{
        //    var plotData = new
        //    {
        //        mapValue = new
        //        {
        //            fields = new Dictionary<string, object>
        //            {
        //                { "plotId", new { stringValue = plot.PlotId } },
        //                { "isUnlocked", new { booleanValue = plot.IsUnlocked } },
        //                { "isDug", new { booleanValue = plot.IsDug } },
        //                { "position", new
        //                    {
        //                        mapValue = new
        //                        {
        //                            fields = new
        //                            {
        //                                x = new { integerValue = plot.Position.x.ToString() },
        //                                y = new { integerValue = plot.Position.y.ToString() }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    // Add crop data if exists
        //    if (plot.Crop != null)
        //    {
        //        ((Dictionary<string, object>)plotData.mapValue.fields)["crop"] = new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    seedId = new { stringValue = plot.Crop.SeedId },
        //                    plantedAt = new { timestampValue = plot.Crop.PlantedAt.ToUniversalTime().ToString("o") },
        //                    growthStage = new { integerValue = plot.Crop.GrowthStage.ToString() },
        //                    growthProgress = new { doubleValue = plot.Crop.GrowthProgress },
        //                    isWatered = new { booleanValue = plot.Crop.IsWatered },
        //                    isFertilized = new { booleanValue = plot.Crop.IsFertilized },
        //                    readyToHarvest = new { booleanValue = plot.Crop.ReadyToHarvest }
        //                }
        //            }
        //        };
        //    }
        //    else
        //    {
        //        ((Dictionary<string, object>)plotData.mapValue.fields)["crop"] = new Dictionary<string, object>
        //            {
        //                { "nullValue", null }
        //            };
        //    }

        //    plotsArray.Add(plotData);
        //}

        //var firestoreData = new
        //{
        //    fields = new
        //    {
        //        plots = new { arrayValue = new { values = plotsArray } },
        //        totalPlotsUnlocked = new { integerValue = farmland.TotalPlotsUnlocked.ToString() },
        //        updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
        //    }
        //};

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(farmland, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/farmlandData", jsonData, callback);
    }

    /// <summary>
    /// Save animal farm
    /// </summary>
    public IEnumerator SaveAnimalFarm(string playerId, AnimalFarm animalFarm,Action<bool, string> callback)
    {
        //var animalsArray = new List<object>();

        //foreach (var animal in animalFarm.Animals)
        //{
        //    animalsArray.Add(new
        //    {
        //        mapValue = new
        //        {
        //            fields = new
        //            {
        //                animalId = new { stringValue = animal.AnimalId },
        //                type = new { stringValue = animal.Type },
        //                name = new { stringValue = animal.Name },
        //                age = new { integerValue = animal.Age.ToString() },
        //                affection = new { integerValue = animal.Affection.ToString() },
        //                isFed = new { booleanValue = animal.IsFed },
        //                canProduce = new { booleanValue = animal.CanProduce },
        //                productId = new { stringValue = animal.ProductId },
        //                isDead = new { booleanValue = animal.IsDead }
        //            }
        //        }
        //    });
        //}

        //var firestoreData = new
        //{
        //    fields = new
        //    {
        //        farmLevel = new { integerValue = animalFarm.FarmLevel.ToString() },
        //        maxCapacity = new { integerValue = animalFarm.MaxCapacity.ToString() },
        //        //hasAutoMilker = new { booleanValue = animalFarm.HasAutoMilker },
        //        //hasAutoIncubator = new { booleanValue = animalFarm.HasAutoIncubator },
        //        animals = new { arrayValue = new { values = animalsArray } },
        //        updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
        //    }
        //};

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(animalFarm, true);
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/animalFarmData", jsonData, callback);
    }

    public IEnumerator SaveFishing(string playerId, Fishing fishing, Action<bool, string> callback)
    {

        // Prepare pond fish array
        //var fishArray = new List<object>();

        //if (fishing.Pond != null && fishing.Pond.Fish != null)
        //{
        //    foreach (var fish in fishing.Pond.Fish)
        //    {
        //        fishArray.Add(new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    fishId = new { stringValue = fish.FishId },
        //                    species = new { stringValue = fish.Species },
        //                    addedAt = new { timestampValue = fish.AddedAt.ToUniversalTime().ToString("o") },
        //                    size = new { stringValue = fish.Size },
        //                    canCatch = new { booleanValue = fish.CanCatch }
        //                }
        //            }
        //        });
        //    }
        //}

        //object biggestFish = new { nullValue = (object)null };

        //if (fishing.Stats.BiggestFish != null)
        //{
        //    biggestFish = new
        //    {
        //        mapValue = new
        //        {
        //            fields = new
        //            {
        //                species = new { stringValue = fishing.Stats.BiggestFish?.Species ?? "" },
        //                size = new { stringValue = fishing.Stats.BiggestFish?.Size ?? "" },
        //                caughtAt = new
        //                {
        //                    timestampValue = fishing.Stats.BiggestFish?.CaughtAt.ToUniversalTime().ToString("o") ?? DateTime.UtcNow.ToString("o")
        //                }
        //            }
        //        }
        //    };
        //}

        //// Build Firestore document
        //var firestoreData = new
        //{
        //    fields = new
        //    {
        //        userId = new { stringValue = playerId },

        //        // Pond data
        //        pond = new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    isUnlocked = new { booleanValue = fishing.Pond?.IsUnlocked ?? false },
        //                    maxCapacity = new { integerValue = (fishing.Pond?.MaxCapacity ?? 10).ToString() },
        //                    fish = new { arrayValue = new { values = fishArray } }
        //                }
        //            }
        //        },

        //        // Fishing statistics
        //        stats = new
        //        {
        //            mapValue = new
        //            {
        //                fields = new
        //                {
        //                    totalCaught = new { integerValue = fishing.Stats.TotalCaught.ToString() },
        //                    biggestFish = new
        //                    {
        //                        mapValue = new
        //                        {
        //                            fields = new
        //                            {
        //                                species = new { stringValue = fishing.Stats.BiggestFish?.Species ?? "" },
        //                                size = new { stringValue = fishing.Stats.BiggestFish?.Size ?? "" },
        //                                caughtAt = new
        //                                {
        //                                    timestampValue = fishing.Stats.BiggestFish?.CaughtAt.ToUniversalTime().ToString("o") ?? DateTime.UtcNow.ToString("o")
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        },

        //        updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
        //    }
        //};

        var firestoreData = FirestoreSerializer.ToFirestoreDocument(fishing, true);

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/fishingData", jsonData, callback);
    }

    // not done
    public IEnumerator SaveUserQuest(string playerId, Fishing fishing, Action<bool, string> callback)
    {

        
        var firestoreData = FirestoreSerializer.ToFirestoreDocument(fishing, true);

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}/farmData/fishingData", jsonData, callback);
    }

    public IEnumerator GetData(string playerId, Action<bool, string, Dictionary<string, object>> callback)
    {
        List<string> documentPaths = new List<string>
    {
        // Lưu ý: Các đường dẫn này phải khớp chính xác với ID bạn đã tạo
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerProfiles/{playerId}",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/farmlandData",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/animalFarmData",
        $"projects/{apiConfig.ProjectId}/databases/(default)/documents/playerData/{playerId}/farmData/fishingData"
    };

        var firestoreData = new { documents = documentPaths };
        string jsonData = JsonConvert.SerializeObject(firestoreData);

        // Gọi BatchGet (API: documents:batchGet)
        yield return BatchGet(jsonData, (success, message, firestoreBatchGetResponses) =>
        {
            if (!success || firestoreBatchGetResponses == null)
            {
                callback?.Invoke(false, message, null);
                return;
            }

            Dictionary<string, object> gameData = new Dictionary<string, object>();

            foreach (FirestoreBatchGetResponse response in firestoreBatchGetResponses)
            {
                // Kiểm tra xem document có tồn tại không (nếu null là chưa tạo)
                if (response.Found == null) continue;

                string fullPath = response.Found.Name;

                // LOGIC NHẬN DIỆN: Dựa vào đuôi của đường dẫn hoặc từ khóa đặc trưng
                // Dùng nameof() để Key của Dictionary trùng khít với tên Class C#

                // 1. Check các Subcollection trước (Do đường dẫn dài hơn và cụ thể hơn)
                if (fullPath.EndsWith("farmlandData"))
                {
                    gameData[nameof(Farmland)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Farmland>(response.Found.Fields);
                }
                else if (fullPath.EndsWith("animalFarmData"))
                {
                    gameData[nameof(AnimalFarm)] = CloudDatabaseHelper.FirestoreMapper.ToObject<AnimalFarm>(response.Found.Fields);
                }
                else if (fullPath.EndsWith("fishingData"))
                {
                    gameData[nameof(Fishing)] = CloudDatabaseHelper.FirestoreMapper.ToObject<Fishing>(response.Found.Fields);
                }
                // 2. Check các Root Collection (Chứa ID)
                else if (fullPath.Contains("/playerProfiles/"))
                {
                    gameData[nameof(PlayerProfile)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerProfile>(response.Found.Fields);
                }
                // 3. Check PlayerData (Lưu ý: Phải check cuối cùng hoặc đảm bảo không nhầm với subcollection ở trên)
                else if (fullPath.Contains("/playerData/"))
                {
                    // Fix lỗi copy-paste cũ: Map vào PlayerData chứ không phải PlayerProfile
                    gameData[nameof(PlayerData)] = CloudDatabaseHelper.FirestoreMapper.ToObject<PlayerData>(response.Found.Fields);
                }
            }

            callback?.Invoke(true, "Load data success", gameData);
        });
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