using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    public string objectiveId;
    public ItemData objectiveItem;
    public string description;
    public ObjectiveType objectiveType;
    public int currentAmount;
    public int targetAmount;
    public bool isCompleted => currentAmount >= targetAmount;

    [Header("Random Settings")]
    public bool isRandomEnable = false;
    public int minRandomAmount; // Ngưỡng dưới
    public int maxRandomAmount; // Ngưỡng trên
}

public enum ObjectiveType
{
    COLLECT_ITEM
}

public class QuestProgress
{
    public QuestData quest;
    public List<QuestObjective> questObjectives;

    public QuestProgress(QuestData quest)
    {
        this.quest = quest;
        questObjectives = new();
        foreach(var obj in quest.questObjectives)
        {
            int finalTargetAmount = obj.targetAmount;

            if (obj.isRandomEnable)
            {
                finalTargetAmount = Random.Range(obj.minRandomAmount, obj.maxRandomAmount + 1);
            }
            questObjectives.Add(
                new QuestObjective()
                {
                    objectiveId = obj.objectiveId,
                    objectiveItem = obj.objectiveItem,
                    description = obj.description,
                    objectiveType = obj.objectiveType,
                    targetAmount = finalTargetAmount,
                    currentAmount = 0,
                }
            );
        }
    }

    public QuestProgress(Assets.Scripts.Cloud.Schemas.PlayerQuest schema)
    {
        LoadFromSchema(schema);
    }

    public void LoadFromSchema(Assets.Scripts.Cloud.Schemas.PlayerQuest schema)
    {
        this.quest = GameDataManager.instance.gameSODatabase.GetItemById(schema.QuestId) as QuestData;
        
        questObjectives = new();
        foreach(var obj in quest.questObjectives)
        {
            var questObjectiveSchema = schema.progresses.Find(p => p.ObjectiveId == obj.objectiveId);
            questObjectives.Add(
                new QuestObjective()
                {
                    objectiveId = obj.objectiveId,
                    objectiveItem = obj.objectiveItem,
                    description = obj.description,
                    objectiveType = obj.objectiveType,
                    targetAmount = questObjectiveSchema?.TargetAmount ?? obj.targetAmount,
                    currentAmount = questObjectiveSchema?.CurrentAmount ?? 0,
                }
            );
        }
    }

    public bool isCompleted => questObjectives.TrueForAll(obj => obj.isCompleted);
    public string questId => quest.questId;
}