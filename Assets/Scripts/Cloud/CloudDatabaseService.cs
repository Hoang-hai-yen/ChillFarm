using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CloudDatabaseService
{
    private ApiConfig apiConfig;

    private string playerId;

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
        playerId = CloudManager.Instance.Auth.LocalId;
    }

    public IEnumerator CreateDocument(string collectionId, string documentId, JObject fields, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";
        if (!String.IsNullOrWhiteSpace(documentId))
        {
            url += $"?documentId={documentId}";
        }

        JObject body = new JObject
        {
            ["fields"] = fields
        };

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body.ToString());
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

    public IEnumerator SavePlayerData(PlayerData playerData,Action<bool, string> callback)
    {

        var firestoreData = new
        {
            fields = new
            {
                playerId = new { stringValue = this.playerId },
                gold = new { integerValue = playerData.Gold.ToString() },
                stamina = new { doubleValue = playerData.Stamina },
                maxStamina = new { integerValue = playerData.MaxStamina.ToString() },
                currentDay = new { integerValue = playerData.CurrentDay.ToString() },
                currentTime = new { integerValue = playerData.CurrentTime.ToString() },

                position = new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            x = new { doubleValue = playerData.Position.x },
                            y = new { doubleValue = playerData.Position.y },
                            scene = new { stringValue = playerData.CurrentScene }
                        }
                    }
                },

                experiencePoint = new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            skill = CloudDatabaseHelper.CreateXPField(playerData.SkillXP),
                            fishing = CloudDatabaseHelper.CreateXPField(playerData.FishingXP),
                            animal = CloudDatabaseHelper.CreateXPField(playerData.AnimalXP),
                            farming = CloudDatabaseHelper.CreateXPField(playerData.FarmingXP)
                        }
                    }
                },

                inventory = CloudDatabaseHelper.CreateInventoryField(playerData.Inventory),
                storage = CloudDatabaseHelper.CreateInventoryField(playerData.Storage),

                //tools = new
                //{
                //    mapValue = new
                //    {
                //        fields = new
                //        {
                //            hoe = new { stringValue = playerData.Tools.Hoe ?? "" },
                //            wateringCan = new { stringValue = playerData.Tools.WateringCan ?? "" },
                //            fishingRod = new { stringValue = playerData.Tools.FishingRod ?? "" },
                //            shovel = new { stringValue = playerData.Tools.Shovel ?? "" }
                //        }
                //    }
                //},

                updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
            }
        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"playerData/{playerId}", jsonData, callback);
    }

    public IEnumerator SaveFarmland(Farmland farmland, Action<bool, string> callback)
    {

        var plotsArray = new List<object>();

        foreach (var plot in farmland.Plots)
        {
            var plotData = new
            {
                mapValue = new
                {
                    fields = new Dictionary<string, object>
                    {
                        { "plotId", new { stringValue = plot.PlotId } },
                        { "isUnlocked", new { booleanValue = plot.IsUnlocked } },
                        { "isDug", new { booleanValue = plot.IsDug } },
                        { "position", new
                            {
                                mapValue = new
                                {
                                    fields = new
                                    {
                                        x = new { integerValue = plot.Position.x.ToString() },
                                        y = new { integerValue = plot.Position.y.ToString() }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Add crop data if exists
            if (plot.Crop != null)
            {
                ((Dictionary<string, object>)plotData.mapValue.fields)["crop"] = new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            seedId = new { stringValue = plot.Crop.SeedId },
                            plantedAt = new { timestampValue = plot.Crop.PlantedAt.ToUniversalTime().ToString("o") },
                            growthStage = new { integerValue = plot.Crop.GrowthStage.ToString() },
                            growthProgress = new { doubleValue = plot.Crop.GrowthProgress },
                            isWatered = new { booleanValue = plot.Crop.IsWatered },
                            isFertilized = new { booleanValue = plot.Crop.IsFertilized },
                            readyToHarvest = new { booleanValue = plot.Crop.ReadyToHarvest }
                        }
                    }
                };
            }
            else
            {
                ((Dictionary<string, object>)plotData.mapValue.fields)["crop"] = new Dictionary<string, object>
                    {
                        { "nullValue", null }
                    };
            }

            plotsArray.Add(plotData);
        }

        var firestoreData = new
        {
            fields = new
            {
                playerId = new { stringValue = this.playerId },
                plots = new { arrayValue = new { values = plotsArray } },
                totalPlotsUnlocked = new { integerValue = farmland.TotalPlotsUnlocked.ToString() },
                updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
            }
        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"farmlandData/{playerId}", jsonData, callback);
    }

    /// <summary>
    /// Save animal farm
    /// </summary>
    public IEnumerator SaveAnimalFarm(AnimalFarm animalFarm,Action<bool, string> callback)
    {
        var animalsArray = new List<object>();

        foreach (var animal in animalFarm.Animals)
        {
            animalsArray.Add(new
            {
                mapValue = new
                {
                    fields = new
                    {
                        animalId = new { stringValue = animal.AnimalId },
                        type = new { stringValue = animal.Type },
                        name = new { stringValue = animal.Name },
                        age = new { integerValue = animal.Age.ToString() },
                        affection = new { integerValue = animal.Affection.ToString() },
                        isFed = new { booleanValue = animal.IsFed },
                        canProduce = new { booleanValue = animal.CanProduce },
                        productId = new { stringValue = animal.ProductId },
                        isDead = new { booleanValue = animal.IsDead }
                    }
                }
            });
        }

        var firestoreData = new
        {
            fields = new
            {
                playerId = new { stringValue = this.playerId },
                farmLevel = new { integerValue = animalFarm.FarmLevel.ToString() },
                maxCapacity = new { integerValue = animalFarm.MaxCapacity.ToString() },
                hasAutoMilker = new { booleanValue = animalFarm.HasAutoMilker },
                hasAutoIncubator = new { booleanValue = animalFarm.HasAutoIncubator },
                animals = new { arrayValue = new { values = animalsArray } },
                updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
            }
        };

        string jsonData = JsonConvert.SerializeObject(firestoreData);

        yield return PatchDocument($"animalFarmData/{playerId}", jsonData, callback);
    }

    //public IEnumerator SaveFishing()
    //{
    //    
    //    fishingDirty = false;
    //    yield return null;
    //}

    public static JObject CreatePlayerObject(string name, int exp, int gold, int stamina)
    {
        return new JObject
        {
            ["name"] = new JObject { ["stringValue"] = name },
            ["exp"] = new JObject { ["integerValue"] = $"{exp}" },
            ["gold"] = new JObject { ["integerValue"] = $"{gold}" },
            ["stamina"] = new JObject { ["integerValue"] = $"{stamina}" },
        };
    }
    
    public static JObject CreatePlayerDataObject(string playerId, Time time , Position position)
    {
        return new JObject
        {
            ["player_id"] = new JObject { ["stringValue"] = playerId },
            ["time"] = new JObject
            {
                ["current_time"] = new JObject { ["stringValue"] = time.CurrentTime },
                ["day"] = new JObject { ["integerValue"] = time.Day },
            },
            ["position"] = new JObject
            {
                ["position_x"] = new JObject { ["doubleValue"] = position.X },
                ["position_y"] = new JObject { ["doubleValue"] = position.Y },
                ["scene"] = new JObject { ["stringValue"] = position.Scene },
            },
        };
    }
}