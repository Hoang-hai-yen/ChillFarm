
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUIController: MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    public QuestData testQuest;
    public int testQuestAmount;
    public List<QuestProgress> testQuests = new();

    void Start()
    {
        // for(int i = 0; i < testQuestAmount; i++)
        // {
        //     testQuests.Add(new QuestProgress(testQuest));
        // }
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(var quest in QuestController.Instance.activeQuests)
        { Debug.Log("Updating quest UI for quest: " + quest.quest.name);
            GameObject entry =  Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;

            foreach(var objective in quest.questObjectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description}: ({objective.currentAmount}/{objective.targetAmount})";
            }
        }
    }
}