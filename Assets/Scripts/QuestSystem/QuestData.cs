using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/QuestData")]
public class QuestData: ScriptableObject
{
    public string questId;
    public string questName;
    public List<QuestObjective> questObjectives;
    public string description;

    private void OnValidate()
    {
        if(String.IsNullOrEmpty(questId))
        {
            questId = questName + "_" + Guid.NewGuid().ToString();
        }
    }
}
