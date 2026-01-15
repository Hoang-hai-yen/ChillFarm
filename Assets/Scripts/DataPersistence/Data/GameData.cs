using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{

    public Farmland FarmlandData { get; set; }
    public AnimalFarm AnimalFarmData { get; set; }
    public PlayerData PlayerDataData { get; set; }
    public PlayerProfile PlayerProfileData { get; set; }
    public Fishing FishingData { get; set; }
    // public List<Assets.Scripts.Cloud.Schemas.Quest> QuestsData { get; set; } = new List<Assets.Scripts.Cloud.Schemas.Quest>();
    public List<PlayerQuest> PlayerQuestsData { get; set; } = new List<PlayerQuest>();
    public List<string> HandInQuestIds { get; set; } = new List<string>();
    public GameData() 
    {

    }

   
}
