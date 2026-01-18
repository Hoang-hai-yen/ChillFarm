using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuest
{
    PlayerQuest PlayerQuest { get; set; }

    public ActiveQuest(PlayerQuest playerQuest)
    {
        PlayerQuest = playerQuest;
        // if (playerQuest.QuestType == QuestType.HARVEST)
        // {
        //     GameEventsManager.instance.farmlandEvents.onCropHarvest += QuestUpdate;

        // }
        // else
        // {
        //     GameEventsManager.instance.fishingEvents.onFishCaught += QuestUpdate;
        // }
    }

    public void QuestUpdate(string itemId, int amount)
    {
        // PlayerQuest.QuestProgress currentProress = PlayerQuest.progresses.Find(p => p.ItemId == itemId);
        // currentProress.CurrentAmount += amount;
        // if (currentProress.TargetAmount == currentProress.CurrentAmount)
        // {
        //     currentProress.IsCompleted = true;
        // }
        // GameEventsManager.instance.questEvents.AdvanceQuest(PlayerQuest.QuestId);

        // if (PlayerQuest.progresses.TrueForAll(p => p.IsCompleted))
        // {
        //     PlayerQuest.IsCompleted = true;
        //     GameEventsManager.instance.questEvents.FinishQuest(PlayerQuest.QuestId);
        // }
       

    }

}
