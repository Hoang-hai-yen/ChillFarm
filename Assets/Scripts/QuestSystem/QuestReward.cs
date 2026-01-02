
[System.Serializable]
public class QuestReward
{
    public RewardType rewardType;
    public string rewardId; // For ITEM type, this is the item ID
    public int amount = 1; // For GOLD, EXPERIENCE, or ITEM quantity
}

public enum RewardType
{
    ITEM,
    GOLD,
    EXPERIENCE,
    CUSTOM
}