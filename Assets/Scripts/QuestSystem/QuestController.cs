
using System.Collections.Generic;
using UnityEngine;

public class QuestController: MonoBehaviour
{
    public static QuestController Instance {get; set;}
    public List<QuestProgress> activeQuests = new();
    private QuestUIController questUI;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindAnyObjectByType<QuestUIController>();
        if(questUI == null)
            Debug.Log("Quest UI is null");
    }

    public void AcceptQuest(QuestData quest)
    {
        if(IsQuestActive(quest.questId)) return;
        activeQuests.Add(new QuestProgress(quest));
        if(questUI != null)
            questUI.UpdateUI();
    }

    public bool IsQuestActive(string questId) => activeQuests.Exists(q => q.questId == questId);
}