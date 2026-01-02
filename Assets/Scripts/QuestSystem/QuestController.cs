
using System.Collections.Generic;
using UnityEngine;

public class QuestController: MonoBehaviour
{
    public static QuestController Instance {get; set;}
    public List<QuestProgress> activeQuests = new();
    private QuestUIController questUI;

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

    public void AcceptQuest(QuestData quest)
    {
        if(IsQuestActive(quest.questId)) return;
        activeQuests.Add(new QuestProgress(quest));
        if(questUI != null)
        {
            CheckInventoryForQuests();
            questUI.UpdateUI();
        }
    }

    public bool IsQuestActive(string questId) => activeQuests.Exists(q => q.questId == questId);

    public void CheckInventoryForQuests()
    {
        Dictionary<string, int> itemCounts = InventoryManager.Instance.GetItemCounts();
        foreach(var quest in activeQuests)
        { 
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
}