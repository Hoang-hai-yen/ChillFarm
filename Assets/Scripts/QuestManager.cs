using Assets.Scripts.Cloud.Schemas;
using System.Collections.Generic;
using UnityEngine;

//public class QuestManager : MonoBehaviour
//{

//    void Start()
//    {
//        EventManager.OnQuestAcceptAction += CreatePlayerQuest;
//        EventManager.OnQuestUpdateAction += CheckQuests;

//    }

//    private void CreatePlayerQuest(Quest quest)
//    {
//        var questProgresses = new List<PlayerQuest.QuestProgress>();

//        quest.Requirements.ForEach(p => { questProgresses.Add(new PlayerQuest.QuestProgress() {ItemId = p.ItemId, CurrentAmount = 0}); });

//        PlayerQuest playerQuest = new PlayerQuest
//        {
//            QuestId = quest.Id,
//            IsCompleted = false,
//            IsClaimed = false,
//            IsChanged = true,
//            progresses = questProgresses,
//        };

//        CloudManager.Instance.PlayerQuestsData.Add(playerQuest);
//        EventManager.TriggerDataDirtyAction(CloudManager.DataType.quest);
//    }


//    private void CheckQuests(string targetId, int amount)
//    {
//        foreach (var quest in CloudManager.Instance.QuestsData)
//        {
//            if (IsQuestCompleted(quest.Id)) continue;

//            if(Quest.IsTargetItem(quest, targetId))
//                UpdateProgress(quest.Id, targetId, amount);
            
//        }
//    }

//    private bool IsQuestCompleted(string questId)
//    {
//        var progress = CloudManager.Instance.PlayerQuestsData.Find(q => q.QuestId == questId);
//        return progress != null && progress.IsCompleted;
//    }

//    private void UpdateProgress(string questId, string targetId, int amount)
//    {
//        PlayerQuest currentQuest = CloudManager.Instance.PlayerQuestsData.Find(q => q.QuestId == questId);
        
//        if(currentQuest == null) return;

//        currentQuest.progresses.Find(p => p.ItemId == targetId).CurrentAmount += amount;
//        currentQuest.IsChanged = true;
//        EventManager.TriggerDataDirtyAction(CloudManager.DataType.quest);

//    }

//    private void OnDestroy()
//    {
//        EventManager.OnQuestAcceptAction += CreatePlayerQuest;
//        EventManager.OnQuestUpdateAction += CheckQuests;
//    }
//}
