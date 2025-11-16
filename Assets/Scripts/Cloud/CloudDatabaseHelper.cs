using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class CloudDatabaseHelper
{
    

    static public object CreateXPField(ExperiencePoint xp)
    {
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    level = new { integerValue = xp.Level.ToString() },
                    currentXP = new { integerValue = xp.CurrentXP.ToString() },
                    totalXP = new { integerValue = xp.TotalXP.ToString() }
                }
            }
        };
    }

    static public object CreateXPField()
    {
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    level = new { integerValue = "1" },
                    currentXP = new { integerValue = "0" },
                    totalXP = new { integerValue = "0" }
                }
            }
        };
    }

    static public object CreateInventoryField(Inventory inventory)
    {
        var itemsArray = new List<object>();

        foreach (var item in inventory.Items)
        {
            itemsArray.Add(new
            {
                mapValue = new
                {
                    fields = new
                    {
                        itemId = new { stringValue = item.ItemId },
                        quantity = new { integerValue = item.Quantity.ToString() },
                        slotIndex = new { integerValue = item.SlotIndex.ToString() }
                    }
                }
            });
        }

        return new
        {
            mapValue = new
            {
                fields = new
                {
                    maxSlots = new { integerValue = inventory.MaxSlots.ToString() },
                    items = new { arrayValue = new { values = itemsArray } }
                }
            }
        };
    }

    static public object CreateInventoryField()
    {
        
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    maxSlots = new { integerValue = "1" },
                    items = new { arrayValue = new { values = Array.Empty<object>() } }
                }
            }
        };
    }

    static public object CreateDocument(string path, object data)
    {

        return new
        {
            name = path,
            fields = data
        };
    }

    static public object CreateFirestoreValue(object value, string valueType)
    {
        switch (valueType)
        {
            case "integerValue":
                return new { integerValue = value.ToString() };
            case "doubleValue":
                return new { doubleValue = value };
            case "stringValue":
                return new { stringValue = value.ToString() };
            case "booleanValue":
                return new { booleanValue = (bool)value };
            default:
                return new { stringValue = value.ToString() };
        }
    }

    //public IEnumerator CreateNewPlayerData()
    //{
    //    var defaultPlayerData = new
    //    {
    //        fields = new
    //        {
    //            gold = new { integerValue = "500" },
    //            stamina = new { doubleValue = 100.0 },
    //            maxStamina = new { integerValue = "100" },
    //            currentDay = new { integerValue = "1" },
    //            currentTime = new { integerValue = "480" },
    //            createdAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
    //        }
    //    };

    //    string url = $"{FIREBASE_DB_URL}/playerData/{userId}";
    //    string jsonData = JsonConvert.SerializeObject(defaultPlayerData);

    //    yield return PatchDocument(url, jsonData, success =>
    //    {
    //        if (success)
    //            Debug.Log("New player data created!");
    //    });
    //}

    //public PlayerData ParsePlayerData(FirestoreDocument doc)
    //{
    //    // Parse Firestore document to PlayerData object
    //    // Implementation depends on your data structure
    //    return new PlayerData();
    //}

    //public FarmlandData ParseFarmlandData(FirestoreDocument doc)
    //{
    //    return new FarmlandData();
    //}

    //public List<Quest> ParseQuests(List<QueryResult> results)
    //{
    //    return new List<Quest>();
    //}

    static public Animal ConvertToAnimal(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;

        return new Animal()
        {
           AnimalId = fields["animalId"].StringValue,
            Type = fields["type"].StringValue,
            Name = fields["name"].StringValue,
            Age = int.Parse(fields["age"].IntegerValue),
            Affection = int.Parse(fields["affection"].IntegerValue),
            IsFed = fields["isFed"].BooleanValue,
            CanProduce = fields["canProduce"].BooleanValue,
            ProductId = fields["productId"].StringValue,
            IsDead = fields["isDead"].BooleanValue
};
    }

    static public Plot ConvertToPlot(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;
        return new Plot()
        {
            PlotId = fields["plotId"].StringValue,
            IsUnlocked = fields["isUnlocked"].BooleanValue,
            IsDug = fields["isDug"].BooleanValue,
            Position = (fields["position"].MapValue.Fields["x"].DoubleValue, fields["position"].MapValue.Fields["y"].DoubleValue),
            
            Crop = ConvertToCropSaveData(fields["crop"].MapValue) 
        };
    }

    static public CropSaveData ConvertToCropSaveData(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;

        // Đổi tên class
        return new CropSaveData() 
        {
           SeedId = fields["seedId"].StringValue,
            PlantedAt = DateTime.Parse(fields["plantedAt"].TimestampValue, null, System.Globalization.DateTimeStyles.RoundtripKind),
            GrowthStage = int.Parse(fields["growthStage"].IntegerValue),
            GrowthProgress = fields["growthProgress"].DoubleValue,
            IsWatered = fields["isWatered"].BooleanValue,
            IsFertilized = fields["isFertilized"].BooleanValue,
            ReadyToHarvest = fields["readyToHarvest"].BooleanValue,
        };
    }

    static public ExperiencePoint ConvertToExperiencePoint(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;

        return new ExperiencePoint()
        {
            Level = int.Parse(fields["level"].IntegerValue),
            CurrentXP = int.Parse(fields["currentXP"].IntegerValue),
            TotalXP = int.Parse(fields["totalXP"].IntegerValue)
        };

    }

    static public Inventory.Item ConvertToItem(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;

        return new Inventory.Item()
        {
            ItemId = fields["itemId"].StringValue,
            Quantity = int.Parse(fields["quantity"].IntegerValue),
            SlotIndex = int.Parse(fields["slotIndex"].IntegerValue)

        };

    }

    static public Inventory ConvertToInventory(FirestoreMap firestoreMap)
    {
        var fields = firestoreMap.Fields;
        FirestoreArray itemsFirestore = fields["items"].ArrayValue;
        List<Inventory.Item> items = new List<Inventory.Item>();
        
        if(itemsFirestore.Values != null)
            foreach(FirestoreValue firestoreValue in itemsFirestore.Values)
            {
                items.Add(ConvertToItem(firestoreValue.MapValue));
            }

        return new Inventory()
        {
            MaxSlots = int.Parse(fields["maxSlots"].IntegerValue),
            Items = items
        };

    }

    static public PlayerProfile ConvertToPlayerProfile(FirestoreBatchGetResponse firestoreBatchGetResponse)
    {
        var fields = firestoreBatchGetResponse.Found.Fields;
        return new PlayerProfile()
        {
            Email = fields["email"].StringValue,
            Name = fields["name"].StringValue,
            Avatar = fields["avatar"].StringValue
        };
    }

    static public PlayerData ConvertToPlayerData(FirestoreBatchGetResponse firestoreBatchGetResponse)
    { 
        var fields = firestoreBatchGetResponse.Found.Fields;

        return new PlayerData()
        {
            Gold = int.Parse(fields["gold"].IntegerValue),
            Stamina = fields["stamina"].DoubleValue,
            MaxStamina = fields["maxStamina"].DoubleValue,
            CurrentDay = int.Parse(fields["currentDay"].IntegerValue),
            CurrentTime = int.Parse(fields["currentTime"].IntegerValue),
            Position = (fields["position"].MapValue.Fields["x"].DoubleValue, fields["position"].MapValue.Fields["y"].DoubleValue),
            CurrentScene = fields["position"].MapValue.Fields["scene"].StringValue,
            SkillXP = ConvertToExperiencePoint(fields["experiencePoint"].MapValue.Fields["skill"].MapValue),
            AnimalXP = ConvertToExperiencePoint(fields["experiencePoint"].MapValue.Fields["animal"].MapValue),
            FishingXP = ConvertToExperiencePoint(fields["experiencePoint"].MapValue.Fields["fishing"].MapValue),
            FarmingXP = ConvertToExperiencePoint(fields["experiencePoint"].MapValue.Fields["farming"].MapValue),
            Inventory = ConvertToInventory(fields["inventory"].MapValue),
            Storage = ConvertToInventory(fields["storage"].MapValue),
        };
    }

    static public AnimalFarm ConvertToAnimalFarm(FirestoreBatchGetResponse firestoreBatchGetResponse)
    {
        var fields = firestoreBatchGetResponse.Found.Fields;

        FirestoreArray animalsFirestore = fields["animals"].ArrayValue;
        List<Animal> animals = new List<Animal>();

        if(animalsFirestore.Values != null)
            foreach(FirestoreValue firestoreValue in animalsFirestore.Values)
            {
                animals.Add(ConvertToAnimal(firestoreValue.MapValue));
            }

        return new AnimalFarm()
        {
            Animals = animals,
            FarmLevel = int.Parse(fields["farmLevel"].IntegerValue),
            MaxCapacity = int.Parse(fields["maxCapacity"].IntegerValue)
        };
    }

    

    static public Farmland ConvertToFarmlam(FirestoreBatchGetResponse firestoreBatchGetResponse)
    {
        var fields = firestoreBatchGetResponse.Found.Fields;
        FirestoreArray plotsFirestore = fields["plots"].ArrayValue;
        List<Plot> plots = new List<Plot>();

        if(plotsFirestore.Values != null)
            foreach(FirestoreValue firestoreValue in plotsFirestore.Values)
            {
                plots.Add(ConvertToPlot(firestoreValue.MapValue));
            }

        return new Farmland()
        {
           Plots = plots,
           TotalPlotsUnlocked = int.Parse(fields["totalPlotsUnlocked"].IntegerValue)
        };
    }



    static public List<FirestoreBatchGetResponse> ParseBatchGetResponse(string ndjson)
    {
        List<FirestoreBatchGetResponse> list = new List<FirestoreBatchGetResponse>();

        string[] lines = ndjson.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var obj = JsonConvert.DeserializeObject<FirestoreBatchGetResponse>(line);

            if (obj != null)
                list.Add(obj);
        }

        return list;
    }

    public class FirestoreBatchGetResponse
    {
        [JsonProperty("found")]
        public FirestoreFound Found { get; set; }

        [JsonProperty("missing")]
        public string Missing { get; set; }

        [JsonProperty("readTime")]
        public string ReadTime { get; set; }
    }

    public class FirestoreFound
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, FirestoreValue> Fields { get; set; }
    }

    public class FirestoreValue
    {
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }

        [JsonProperty("integerValue")]
        public string IntegerValue { get; set; }

        [JsonProperty("doubleValue")]
        public double DoubleValue { get; set; }

        [JsonProperty("booleanValue")]
        public bool BooleanValue { get; set; }

        [JsonProperty("timestampValue")]
        public string TimestampValue { get; set; }

        [JsonProperty("mapValue")]
        public FirestoreMap MapValue { get; set; }

        [JsonProperty("arrayValue")]
        public FirestoreArray ArrayValue { get; set; }
    }

    public class FirestoreMap
    {
        [JsonProperty("fields")]
        public Dictionary<string, FirestoreValue> Fields { get; set; }
    }

    public class FirestoreArray
    {
        [JsonProperty("values")]
        public List<FirestoreValue> Values { get; set; }
    }




    [Serializable]
    public class FirestoreDocument
    {
        public string name;
        public Dictionary<string, FirestoreValue> fields;
        public string createTime;
        public string updateTime;
    }

    //[Serializable]
    //public class FirestoreValue
    //{
    //    public string stringValue;
    //    public string integerValue;
    //    public double doubleValue;
    //    public bool booleanValue;
    //    public string timestampValue;
    //    public FirestoreMapValue mapValue;
    //    public FirestoreArrayValue arrayValue;
    //}

    //[Serializable]
    //public class FirestoreMapValue
    //{
    //    public Dictionary<string, FirestoreValue> fields;
    //}

    //[Serializable]
    //public class FirestoreArrayValue
    //{
    //    public List<FirestoreValue> values;
    //}

    //[Serializable]
    //public class QueryResult
    //{
    //    public FirestoreDocument document;
    //}

    public class BatchOperation
    {
        public string Collection;
        public string DocumentId;
        public BatchOperationType Type;
        public Dictionary<string, object> FirestoreFields;
    }

    public enum BatchOperationType
    {
        Update,
        Delete
    }

}

