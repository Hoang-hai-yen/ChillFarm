using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Cloud.Schemas.PlayerQuest;

public class QuestManager : MonoBehaviour, IDataPersistence         
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    private List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    private List<PlayerQuest> playerQuests = new List<PlayerQuest>();
    private List<Assets.Scripts.Cloud.Schemas.Quest> quests = new List<Assets.Scripts.Cloud.Schemas.Quest>();



    // quest start requirements
    private int currentPlayerLevel;

    private void Awake()
    {
        //questMap = CreateQuestMap();
     }
        

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

      
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
    }

    private void Start()
    {

    }


    private void Update()
    {
       
    }

 

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

    }

    private void FinishQuest(string id)
    {
        var quest = quests.Find(quests => quests.Id == id);
        if (quest != null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
            return;
        }
        ClaimRewards(id);
    }

    private void ClaimRewards(string questId)
    {
        var playerQuest = playerQuests.Find(pq => pq.QuestId == questId);
        var quest = quests.Find(q => q.Id == questId);

        GameEventsManager.instance.goldEvents.GoldGained(quest.Rewards.GetTotalGold(playerQuest.CurrentLevel));
        foreach (var xpReward in quest.Rewards.Xp)
        {
            GameEventsManager.instance.playerEvents.ExperienceGained(xpReward.type, xpReward.GetTotalXP(quest.Rewards.rewardMultiplier, playerQuest.CurrentLevel));
        }
    }


    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }
   
    public void LoadData(GameData data)
    {  
        if(data == null)
        {
            Debug.Log("Game Data is null");
            return;
        }
       foreach(var playerQuest in data.PlayerQuestsData)
        {
            if(!playerQuest.IsCompleted)
                activeQuests.Add(new ActiveQuest(playerQuest));
        }

        playerQuests = data.PlayerQuestsData;
        quests = data.QuestsData;
    }

    public void SaveData(GameData data)
    {
       
    }

    private void StartQuest(string id)
    {
        var playerQuest = playerQuests.Find(pq => pq.QuestId == id);
        var quest = quests.Find(q => q.Id == id);
        if (playerQuest == null)
        {
            var questProgresses = new List<PlayerQuest.QuestProgress>();
            quest.Requirements.ForEach(p => { questProgresses.Add(new PlayerQuest.QuestProgress() { ItemId = p.ItemId, CurrentAmount = 0, TargetAmount = p.baseAmount, }); });


            PlayerQuest newPlayerQuest = new PlayerQuest
            {
                QuestId = quest.Id,
                QuestType = quest.QuestType,
                CurrentLevel = 1,
                IsCompleted = false,
                IsClaimed = false,
                IsChanged = true,
                progresses = questProgresses,
            };

            activeQuests.Add(new ActiveQuest(playerQuest));
            playerQuests.Add(playerQuest);
        }
        else if(playerQuest.IsCompleted && playerQuest.IsClaimed)
        {
            playerQuest.AdvanceQuest(quest.Requirements);
        }
        

        GameDataManager.instance.MarkDirty(GameDataManager.DataType.quest);
    }

    
}
