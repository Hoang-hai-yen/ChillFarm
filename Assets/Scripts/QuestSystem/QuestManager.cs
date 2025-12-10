using Assets.Scripts.Cloud.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IDataPersistence         
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    private List<GameQuest> gameQuests = new List<GameQuest>();
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
        GameEventsManager.instance.questEvents.onStartQuest += CreatePlayerQuest;
        //GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

        //GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        //GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartQuest -= CreatePlayerQuest;
        //GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;

        //GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        //GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        
        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    //private void PlayerLevelChange(int level)
    //{
    //    currentPlayerLevel = level;
    //}

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void StartQuest(string id) 
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        var quest = quests.Find(quests => quests.Id == id);
        if (quest != null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
            return;
        }
        ClaimRewards(quest);
        //ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(Assets.Scripts.Cloud.Schemas.Quest quest)
    {
        //GameEventsManager.instance.goldEvents.GoldGained(quest.info.goldReward);
        //GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
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

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);
            // saving to PlayerPrefs is just a quick example for this tutorial video,
            // you probably don't want to save this info there long-term.
            // instead, use an actual Save & Load system and write to a file, the cloud, etc..
            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try 
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }

    public void LoadData(GameData data)
    {
       foreach(var playerQuest in data.PlayerQuestsData)
        {
            if(playerQuest.QuestType == QuestType.HARVEST)
            {
                gameQuests.Add(new GameHarvestQuest(playerQuest));
            }
            else if (playerQuest.QuestType == QuestType.FISHING)
            {
                gameQuests.Add(new GameFishingQuest(playerQuest));
            }
        }

        playerQuests = data.PlayerQuestsData;
        quests = data.QuestsData;
    }

    public void SaveData(GameData data)
    {
       
    }

    private void CreatePlayerQuest(string id)
    {
        var questProgresses = new List<PlayerQuest.QuestProgress>();
        var quest = quests.Find(q => q.Id == id);
        quest.Requirements.ForEach(p => { questProgresses.Add(new PlayerQuest.QuestProgress() { ItemId = p.ItemId, CurrentAmount = 0 }); });

        PlayerQuest playerQuest = new PlayerQuest
        {
            QuestId = quest.Id,
            QuestType = quest.QuestType,
            IsCompleted = false,
            IsClaimed = false,
            IsChanged = true,
            progresses = questProgresses,
        };

        if (playerQuest.QuestType == QuestType.HARVEST)
        {
            gameQuests.Add(new GameHarvestQuest(playerQuest));
        }
        else if (playerQuest.QuestType == QuestType.FISHING)
        {
            gameQuests.Add(new GameFishingQuest(playerQuest));
        }
        playerQuests.Add(playerQuest);

        GameDataManager.instance.MarkDirty(GameDataManager.DataType.quest);
    }

    
}
