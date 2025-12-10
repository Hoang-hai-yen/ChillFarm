using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHarvestQuest: GameQuest
{

    public GameHarvestQuest(PlayerQuest playerQuest)
    {
        PlayerQuest = playerQuest;
        GameEventsManager.instance.farmlandEvents.onCropHarvest += QuestUpdate;
    }

    public void QuestUpdate(string itemId, int amount = 1)
    {
        PlayerQuest.QuestProgress currentProress = PlayerQuest.progresses.Find(p => p.ItemId == itemId);
        currentProress.CurrentAmount += amount;
        if (currentProress.TargetAmount == currentProress.CurrentAmount)
        {
            currentProress.IsCompleted = true;
        }

        if(PlayerQuest.progresses.TrueForAll(p => p.IsCompleted))
        {
            PlayerQuest.IsCompleted = true;
            GameEventsManager.instance.questEvents.FinishQuest(PlayerQuest.QuestId);
        }

    }

    

}
