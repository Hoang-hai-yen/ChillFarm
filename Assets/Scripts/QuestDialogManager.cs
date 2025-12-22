using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDialogManager : MonoBehaviour
{
    public static QuestDialogManager Instance;

    [Header("UI")]
    public GameObject dialog;
    public TextMeshProUGUI questText;
    public Button buttonAccept;
    public Button buttonReject;
    public Button buttonComplete;

    QuestGiver currentNPC;
    Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        dialog.SetActive(false);

        buttonAccept.onClick.AddListener(Accept);
        buttonReject.onClick.AddListener(Reject);
        buttonComplete.onClick.AddListener(Complete);
    }

    void Update()
    {
        if (dialog.activeSelf && Input.GetKeyDown(KeyCode.Z))
        {
            Close();
            return;
        }

        if (dialog.activeSelf && currentNPC != null && currentNPC.IsInCooldown())
        {
            ShowCooldown();
        }
    }

    public bool IsOpen()
    {
        return dialog.activeSelf;
    }

    public void Open(QuestGiver npc)
    {
        currentNPC = npc;
        dialog.SetActive(true);

        if (npc.IsInCooldown())
        {
            ShowCooldown();
            return;
        }

        if (!npc.HasQuest())
        {
            npc.GenerateQuest();
        }

        ShowQuest();
    }

    void Close()
    {
        dialog.SetActive(false);
        currentNPC = null;
    }

    void ShowQuest()
    {
        Quest q = currentNPC.quest;

        questText.text =
            $"{q.questContent}\n\n" +
            $"Yêu cầu: {q.amountRequired} {q.itemRequired}\n" +
            $"Thưởng: {q.rewardGold} vàng, {q.rewardXP} XP";

        buttonAccept.gameObject.SetActive(!q.isAccepted);
        buttonReject.gameObject.SetActive(!q.isAccepted);
        buttonComplete.gameObject.SetActive(q.isAccepted && !q.isCompleted);
    }

    void ShowCooldown()
    {
        questText.text = $"⏳ Quay lại sau {currentNPC.RemainingCooldown()}s";

        buttonAccept.gameObject.SetActive(false);
        buttonReject.gameObject.SetActive(false);
        buttonComplete.gameObject.SetActive(false);
    }

    void Accept()
    {
        currentNPC.AcceptQuest();
        ShowQuest();
    }

    void Reject()
    {
        currentNPC.RejectQuest();
        ShowCooldown();
    }

    void Complete()
    {
        Quest q = currentNPC.quest;

        if (!inventory.HasItem(q.itemRequired, q.amountRequired))
        {
            questText.text += "\n\n❌ Chưa đủ vật phẩm!";
            return;
        }

        currentNPC.CompleteQuest(inventory);
        Close();
    }
}
