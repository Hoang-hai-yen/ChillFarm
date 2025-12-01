using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<Quest> allQuests;
    private List<PlayerQuest> playerProgress;

    void Start()
    {
        EventManager.OnGameAction += CheckQuests;
    }

    private void CheckQuests(string targetId, int amount)
    {
        foreach (var quest in allQuests)
        {
            if (IsQuestCompleted(quest.Id)) continue;

            if(Quest.IsTargetItem(quest, targetId))
                UpdateProgress(quest.Id, targetId, amount);
            
        }
    }

    private bool IsQuestCompleted(string questId)
    {
        var progress = playerProgress.Find(q => q.QuestId == questId);
        return progress != null && progress.IsCompleted;
    }

    private void UpdateProgress(string questId, string targetId, int amount)
    {
        PlayerQuest currentQuest = playerProgress.Find(q => q.QuestId == questId);
        
        if(currentQuest == null) return;

        currentQuest.progresses.Find(p => p.ItemId == targetId).CurrentAmount += amount;
    }
}
