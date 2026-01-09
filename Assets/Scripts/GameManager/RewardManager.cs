
using UnityEngine;

public class RewardManager: MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GrantRewards(QuestData quest)
    {
        foreach(var reward in quest.rewards)
        {
            switch(reward.rewardType)
            {
                case RewardType.ITEM:
                    GiveItemReward(reward.rewardId, reward.amount);
                    break;
                case RewardType.GOLD:
                    GiveGoldReward(reward.amount);
                    break;  
                case RewardType.EXPERIENCE:
                    break;  
                case RewardType.CUSTOM:
                    break;  
                default:
                    Debug.LogWarning("Unknown reward type: " + reward.rewardType);
                    break;
            }
        }
    }

    public void GiveItemReward(string itemId, int amount)
    {
        // InventoryManager.Instance.AddItem(itemId, amount);
    }

    public void GiveGoldReward(int amount)
    {
        InventoryManager.Instance.AddGold(amount);
    }
}