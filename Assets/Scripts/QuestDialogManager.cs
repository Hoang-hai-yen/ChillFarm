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
    PlayerInventory inventory;

    public bool IsOpen
    {
        get { return dialog != null && dialog.activeSelf; }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();

        if (dialog == null) return;

        dialog.SetActive(false);

        if (buttonAccept != null)
            buttonAccept.onClick.AddListener(Accept);

        if (buttonReject != null)
            buttonReject.onClick.AddListener(Reject);

        if (buttonComplete != null)
            buttonComplete.onClick.AddListener(Complete);
    }

    void Update()
    {
        if (dialog == null) return;

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

    public void Open(QuestGiver npc)
    {
        if (npc == null || dialog == null)
        {
            Debug.LogError("QuestDialogManager.Open FAILED: npc or dialog null");
            return;
        }

        Debug.Log("QuestDialog OPEN CALLED");

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
        if (currentNPC == null || currentNPC.quest == null) return;

        QuestDialog q = currentNPC.quest;

        questText.text =
            $"{q.questContent}\n" +
            $"Yêu cầu: {q.amountRequired} {q.itemRequired}\n" +
            $"Thưởng: {q.rewardGold} vàng, {q.rewardXP} XP";

        buttonAccept.gameObject.SetActive(!q.isAccepted);
        buttonReject.gameObject.SetActive(!q.isAccepted);
        buttonComplete.gameObject.SetActive(q.isAccepted && !q.isCompleted);
    }

    void ShowCooldown()
    {
        if (currentNPC == null) return;

        questText.text = $"⏳ Quay lại sau {currentNPC.RemainingCooldown()} giây";

        buttonAccept.gameObject.SetActive(false);
        buttonReject.gameObject.SetActive(false);
        buttonComplete.gameObject.SetActive(false);
    }

    void Accept()
    {
        if (currentNPC == null) return;

        currentNPC.AcceptQuest();
        ShowQuest();
    }

    void Reject()
    {
        if (currentNPC == null) return;

        currentNPC.RejectQuest();
        ShowCooldown();
    }

    void Complete()
    {
        if (currentNPC == null || inventory == null) return;

        QuestDialog q = currentNPC.quest;
        if (q == null) return;

        if (!inventory.HasItem(q.itemRequired, q.amountRequired))
        {
            questText.text += "\n\n❌ Chưa đủ vật phẩm!";
            return;
        }

        currentNPC.CompleteQuest(inventory);
        Close();
    }
}
