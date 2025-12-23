using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [Header("Quest Data")]
    public string[] possibleItems = { "Carrot", "Fish", "Egg", "Milk" };
    public Vector2Int amountRange = new Vector2Int(1, 5);
    public Vector2Int goldRange = new Vector2Int(20, 100);
    public Vector2Int xpRange = new Vector2Int(5, 30);

    [Header("Interaction")]
    public float interactDistance = 1.5f;

    public Quest quest { get; private set; }
    public float nextQuestTime { get; private set; }

    Transform player;
    bool questGenerated = false;

    const float QUEST_COOLDOWN = 60f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        quest = new Quest();
    }

    void Update()
    {
        if (player == null) return;
        if (QuestDialogManager.Instance.IsOpen()) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.Z))
        {
            QuestDialogManager.Instance.Open(this);
        }
    }

    public bool IsInCooldown()
    {
        return Time.time < nextQuestTime;
    }

    public float RemainingCooldown()
    {
        return Mathf.Ceil(nextQuestTime - Time.time);
    }

    public void GenerateQuest()
    {
        quest.itemRequired = possibleItems[Random.Range(0, possibleItems.Length)];
        quest.amountRequired = Random.Range(amountRange.x, amountRange.y + 1);
        quest.rewardGold = Random.Range(goldRange.x, goldRange.y + 1);
        quest.rewardXP = Random.Range(xpRange.x, xpRange.y + 1);

        quest.questContent = "Tôi đang cần một số nguyên liệu.";
        quest.isAccepted = false;
        quest.isCompleted = false;

        questGenerated = true;
    }

    public bool HasQuest()
    {
        return questGenerated;
    }

    public void AcceptQuest()
    {
        quest.isAccepted = true;
    }

    public void RejectQuest()
    {
        questGenerated = false;
        quest.isAccepted = false;
        quest.isCompleted = false;
        nextQuestTime = Time.time + QUEST_COOLDOWN;
    }

    public void CompleteQuest(Inventory inventory)
    {
        inventory.RemoveItem(quest.itemRequired, quest.amountRequired);
        quest.isCompleted = true;
        questGenerated = false;
    }
}
