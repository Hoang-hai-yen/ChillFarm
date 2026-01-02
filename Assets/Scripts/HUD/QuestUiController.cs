
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIController: MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    // public GameObject objectiveTextPrefab;
    public GameObject objectivePrefab;

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
        { 
            GameObject entry =  Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;

            // foreach(var objective in quest.questObjectives)
            // {
            //     GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
            //     TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
            //     objText.text = $"{objective.description}: ({objective.currentAmount}/{objective.targetAmount})";
            // }

            foreach(var objective in quest.questObjectives)
            {
                GameObject objPanel = Instantiate(objectivePrefab, objectiveList);
                TMP_Text objText = objPanel.transform.Find("ObjectiveText").GetComponent<TMP_Text>();
                Image iconImage = objPanel.transform.Find("IconImage").GetComponent<Image>();
                objText.text = $"{objective.description}: ({objective.currentAmount}/{objective.targetAmount})";
                if(iconImage == null) Debug.Log("Icon Image is null");
                if(objective.objectiveItem == null) Debug.Log("Objective Item is null for objective: " + objective.description);
                if(objective.objectiveItem.itemIcon == null) Debug.Log("Objective Item Icon is null for objective: " + objective.description);
                iconImage.sprite = objective.objectiveItem.itemIcon;
            }
        }
    }
}