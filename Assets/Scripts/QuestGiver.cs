using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    [Header("Quest")]
    public Quest quest;

    [Header("UI")]
    public GameObject questDialog;
    public TextMeshProUGUI questText;
    public Button buttonAccept;
    public Button buttonComplete;

    [Header("Button Text")]
    public TextMeshProUGUI acceptButtonText;
    public TextMeshProUGUI completeButtonText;

    [Header("Random Settings")]
    public string[] possibleItems = { "Carrot", "Fish", "Egg", "Milk" };
    public Vector2Int amountRange = new Vector2Int(1, 5);
    public Vector2Int goldRange = new Vector2Int(20, 100);
    public Vector2Int xpRange = new Vector2Int(5, 30);

    [Header("Interaction")]
    public float interactDistance = 1.5f;

    private Inventory inventory;
    private Transform player;
    private bool questGenerated = false;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        questDialog.SetActive(false);

        buttonAccept.onClick.AddListener(AcceptQuest);
        buttonComplete.onClick.AddListener(CompleteQuest);

        acceptButtonText.text = "Nhận nhiệm vụ";
        completeButtonText.text = "Hoàn thành";
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.Z))
        {
            if (!questDialog.activeSelf)
            {
                if (!questGenerated)
                {
                    GenerateRandomQuest();
                    questGenerated = true;
                }

                ShowQuest();
            }
            else
            {
                questDialog.SetActive(false);
            }
        }
    }

    void GenerateRandomQuest()
    {
        quest.itemRequired = possibleItems[Random.Range(0, possibleItems.Length)];
        quest.amountRequired = Random.Range(amountRange.x, amountRange.y + 1);
        quest.rewardGold = Random.Range(goldRange.x, goldRange.y + 1);
        quest.rewardXP = Random.Range(xpRange.x, xpRange.y + 1);

        quest.questContent = "Tôi đang cần một số nguyên liệu.";

        quest.isAccepted = false;
        quest.isCompleted = false;
    }

    void ShowQuest()
    {
        questDialog.SetActive(true);

        questText.text =
            $"{quest.questContent}\n\n" +
            $"Yêu cầu: {quest.amountRequired} {quest.itemRequired}\n" +
            $"Thưởng: {quest.rewardGold} vàng, {quest.rewardXP} XP";

        buttonAccept.gameObject.SetActive(!quest.isAccepted);
        buttonComplete.gameObject.SetActive(quest.isAccepted && !quest.isCompleted);
    }

    void AcceptQuest()
    {
        quest.isAccepted = true;
        ShowQuest();
    }

    void CompleteQuest()
    {
        if (!inventory.HasItem(quest.itemRequired, quest.amountRequired))
        {
            questText.text += "\n\n❌ Chưa đủ vật phẩm!";
            return;
        }

        inventory.RemoveItem(quest.itemRequired, quest.amountRequired);
        quest.isCompleted = true;

        Debug.Log($"Hoàn thành quest! +{quest.rewardGold} vàng, +{quest.rewardXP} XP");

        questDialog.SetActive(false);
    }
}
