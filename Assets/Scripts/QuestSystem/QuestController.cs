
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestController: MonoBehaviour, IDataPersistence
{
    public static QuestController Instance {get; set;}
    public List<QuestProgress> activeQuests = new();
    private QuestUIController questUI;
    public event Action<string> OnQuestCompleted;
    public List<string> handInQuestIds = new();

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindAnyObjectByType<QuestUIController>();
        if(questUI == null)
            Debug.Log("Quest UI is null");
    }
    void OnEnable()
    {
        if(InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    void OnDisable()
    {
        if(InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= CheckInventoryForQuests;
    }

    public void LoadData(GameData data)
    {
         activeQuests = LoadActiveQuestsFromSchema(data.PlayerQuestsData);
         Debug.Log("Loaded " + activeQuests.Count + " active quests.");
         questUI.UpdateUI();
        // handInQuestIds = data.QuestDataData.HandInQuestIds;
    }

    public void SaveData(GameData data)
    {
        data.PlayerQuestsData = new List<Assets.Scripts.Cloud.Schemas.PlayerQuest>();
        foreach(var quest in activeQuests)
        {
            Assets.Scripts.Cloud.Schemas.PlayerQuest schema = new();
            schema.QuestId = quest.quest.questId;
            schema.IsCompleted = quest.isCompleted;
            schema.progresses = new List<Assets.Scripts.Cloud.Schemas.QuestObjective>();
            foreach(var obj in quest.questObjectives)
            {
                schema.progresses.Add(
                    new Assets.Scripts.Cloud.Schemas.QuestObjective()
                    {
                        ObjectiveId =  obj.objectiveId,
                        CurrentAmount = obj.currentAmount,
                        TargetAmount = obj.targetAmount,
                    }
                );
            }
            data.PlayerQuestsData.Add(schema);
        }
        data.HandInQuestIds = handInQuestIds;
    }

    
    public void MarkDataDirty()
    {
        GameDataManager.instance.MarkDirty(GameDataManager.DataType.quest);
    }

    public List<QuestProgress> LoadActiveQuestsFromSchema(List<Assets.Scripts.Cloud.Schemas.PlayerQuest> questSchemas)
    {
        List<QuestProgress> loadedQuests = new();
        if (questSchemas != null) 
        {
            foreach (var schema in questSchemas)
            {
                QuestProgress questProgress = new QuestProgress(schema);

                if (questProgress.quest != null)
                {
                    loadedQuests.Add(questProgress);
                }
                else
                {
                    Debug.LogWarning($"Bỏ qua quest lỗi ID: {schema.QuestId}");
                }
            }
        }
        return loadedQuests;
    }

    public void AcceptQuest(QuestData quest)
    {
        if(IsQuestActive(quest.questId)) return;
        activeQuests.Add(new QuestProgress(quest));
        if(questUI != null)
        {
            CheckInventoryForQuests();
            questUI.UpdateUI();
        }
        MarkDataDirty();
    }

    public bool IsQuestActive(string questId) => activeQuests.Exists(q => q.questId == questId);

    public void CheckInventoryForQuests()
    {
        Dictionary<string, int> itemCounts = InventoryManager.Instance.GetItemCounts();
        
        for(int i = 0; i < activeQuests.Count; i++)
        { 
            var quest = activeQuests[i];
            bool wasCompletedBefore = quest.isCompleted; 

            foreach(var objective in quest.questObjectives)
            {
                if(objective.objectiveType == ObjectiveType.COLLECT_ITEM)
                { 
                    if(itemCounts.TryGetValue(objective.objectiveId, out int count))
                    {   
                        objective.currentAmount = Mathf.Min(count, objective.targetAmount);
                    }
                    else
                    {
                        objective.currentAmount = 0;
                    }
                }
            }

            if (!wasCompletedBefore && quest.isCompleted)
            {
                OnQuestCompleted?.Invoke(quest.questId);
                
                Debug.Log($"Quest {quest.questId} completed!");
            }
        }

        questUI.UpdateUI();
    }

    public bool IsQuestCompleted(string questId)
    {
        QuestProgress quest = activeQuests.Find(q => q.questId == questId);
        if(quest != null)
        {
            return quest.isCompleted;
        }
        return false;
    }
    
    public void HandInQuest(string questId)
    {
        if(!RemoveItemsForQuest(questId)) return;
        QuestProgress quest = activeQuests.Find(q => q.questId == questId);
        if(quest != null)
        {
            handInQuestIds.Add(questId);
            activeQuests.Remove(quest);
            questUI.UpdateUI();
        }
        MarkDataDirty();
    }

    public bool IsQuestHandedIn(string questId)
    {
        return handInQuestIds.Contains(questId);
    }   

    public bool RemoveItemsForQuest(string questId)
    {
        QuestProgress quest = activeQuests.Find(q => q.questId == questId);
        if(quest == null) return false;

        Dictionary<string, int> requiredItems = new();
        
        foreach(var objective in quest.questObjectives)
        {
            if(objective.objectiveType == ObjectiveType.COLLECT_ITEM)
            {
                requiredItems[objective.objectiveId] = objective.targetAmount;
            }
        }

        Dictionary<string, int> itemCounts = InventoryManager.Instance.GetItemCounts();
        
        foreach(var req in requiredItems)
        {
            if(!itemCounts.ContainsKey(req.Key) || itemCounts[req.Key] < req.Value)
            {
                return false;
            }
        }

        foreach(var req in requiredItems)
        {
            InventoryManager.Instance.RemoveItemById(req.Key, req.Value);
        }
        return true;
    }

    public void ResetQuestStatus(string questId)
    {
        if (handInQuestIds.Contains(questId))
        {
            handInQuestIds.Remove(questId);
        }
    }
    public void FinishQuestWithoutRemovingItems(string questId)
    {
        QuestProgress quest = activeQuests.Find(q => q.questId == questId);
        if(quest != null)
        {
            if (!handInQuestIds.Contains(questId))
            {
                handInQuestIds.Add(questId);
            }

            activeQuests.Remove(quest);
            
            if (questUI != null) questUI.UpdateUI();
            
            MarkDataDirty();
            
            Debug.Log($"Đã hoàn thành Quest '{questId}' và giữ lại vật phẩm cho người chơi.");
        }
    }
}