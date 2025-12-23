using UnityEngine;

[System.Serializable]
public class Quest
{
    [TextArea]
    public string questContent;

    public string itemRequired;
    public int amountRequired;

    public int rewardGold;
    public int rewardXP;

    [HideInInspector] public bool isAccepted;
    [HideInInspector] public bool isCompleted;
}
