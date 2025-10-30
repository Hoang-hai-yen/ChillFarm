using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;


class CloudDatabaseHelper
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

    // public object CreateFirestoreValue(object value, string valueType)
    //{
    //    switch (valueType)
    //    {
    //        case "integerValue":
    //            return new { integerValue = value.ToString() };
    //        case "doubleValue":
    //            return new { doubleValue = value };
    //        case "stringValue":
    //            return new { stringValue = value.ToString() };
    //        case "booleanValue":
    //            return new { booleanValue = (bool)value };
    //        default:
    //            return new { stringValue = value.ToString() };
    //    }
    //}

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
}

