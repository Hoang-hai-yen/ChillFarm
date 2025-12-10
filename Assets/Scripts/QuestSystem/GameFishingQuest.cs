using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFishingQuest: GameQuest
{

    public GameFishingQuest(PlayerQuest playerQuest)
    {
        PlayerQuest = playerQuest;
        GameEventsManager.instance.fishingEvents.onFishCaught += QuestUpdate;
    }

    public void QuestUpdate(string itemId)
    {
        PlayerQuest.QuestProgress currentProress = PlayerQuest.progresses.Find(p => p.ItemId == itemId);
        currentProress.CurrentAmount += 1;
        if (currentProress.TargetAmount == currentProress.CurrentAmount)
        {
            currentProress.IsCompleted = true;
        }

        if (PlayerQuest.progresses.TrueForAll(p => p.IsCompleted))
        {
            PlayerQuest.IsCompleted = true;
            GameEventsManager.instance.questEvents.FinishQuest(PlayerQuest.QuestId);
        }

    }

}
