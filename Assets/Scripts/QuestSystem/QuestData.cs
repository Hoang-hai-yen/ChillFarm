using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/QuestData")]
public class QuestData: GameSOData
{
    public string questId => Id;
    public string questName => Name;
    public List<QuestObjective> questObjectives;
    public string description;
    public List<QuestReward> rewards;

    
}
