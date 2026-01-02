
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC: MonoBehaviour, IInteractable
{
    public NPCDialog dialogData;

    private DialogueController dialogueUI;
    private int dialogIndex;
    private bool isTyping, isDialogActive;
    private enum QuestState { NotStarted, InProgress, Completed, OnCooldown }
    private QuestState currentQuestState = QuestState.NotStarted;

    [Header("NPC Instance Data")]

    public bool IsQuestGiver = false;
    private QuestData currentOfferedQuest;
    private int currentOfferedQuestIndex; 
    private int targetQuestStartIndex;
    private float lastQuestHandInTime = -999f; 
    public float cooldownDuration = 300f; 

    public bool IsPatrolNPC = false;

    void Start()
    {
        dialogueUI = DialogueController.Instance;   
    }

    public void Interact()
    {
        if(dialogData == null)
            return;
        
        if(isDialogActive)
        {
            NextLine();
        }
        else
        {
            StartDialog();
        }
    }
    public bool CanInteract()
    {
        return true;
    }
    
    public void OnInteractableRangeEnter()
    {
        // Optional: Show interaction prompt
    }

    public void OnInteractableRangeExit()
    {
        // Optional: Hide interaction prompt
    }

    void StartDialog()
    {
        SyncQuestState();

       if (currentQuestState == QuestState.OnCooldown)
        {
            dialogIndex = dialogData.questOnCooldownIndex;
        }
        else if (currentQuestState == QuestState.InProgress)
        {
            dialogIndex = dialogData.questInProgressIndex;
        }
        else if (currentQuestState == QuestState.Completed)
        {
            dialogIndex = dialogData.questCompletedIndex;
        }
        else // NotStarted
        {
            dialogIndex = 0;

            if(IsQuestGiver && dialogData.questPool.Count > 0)
            {
                PickRandomQuest();
                targetQuestStartIndex = dialogData.targetQuestStartIndexs[currentOfferedQuestIndex];
            }
           
        }

        isDialogActive = true;
        dialogueUI.SetNPCInfo(dialogData.npcName, dialogData.npcPortrait);
        dialogueUI.ShowDialogUI(true);
        // PauseController.setPause(true);
        DisplayCurrentLine();
    }

    private void SyncQuestState()
    {
        // if(dialogData.quest == null) return;

        // string questId = dialogData.quest.questId;

        // if(QuestController.Instance.IsQuestCompleted(questId) || QuestController.Instance.IsQuestHandedIn(questId))
        // {
        //     currentQuestState = QuestState.Completed;
        // }
        // else if(QuestController.Instance.IsQuestActive(questId))
        // {
        //     currentQuestState = QuestState.InProgress;
        // }
        // else
        // {
        //     currentQuestState = QuestState.NotStarted;
        // }

        if(!IsQuestGiver) return;
        // 1. Kiểm tra Cooldown trước
        if (Time.time < lastQuestHandInTime + cooldownDuration)
        {
            currentQuestState = QuestState.OnCooldown;
            return;
        }

        if (currentOfferedQuest == null)
        {
            currentQuestState = QuestState.NotStarted;
            return;
        }

        string questId = currentOfferedQuest.questId;

        // 3. Kiểm tra trạng thái dựa trên quest đã chọn
        if(QuestController.Instance.IsQuestCompleted(questId) || QuestController.Instance.IsQuestHandedIn(questId))
        {
            currentQuestState = QuestState.Completed;
        }
        else if(QuestController.Instance.IsQuestActive(questId))
        {
            currentQuestState = QuestState.InProgress;
        }
        else
        {
            currentQuestState = QuestState.NotStarted;
        }
    }

    private void PickRandomQuest()
    {
        int randomIndex = Random.Range(0, dialogData.questPool.Count);
        currentOfferedQuestIndex = randomIndex;
        currentOfferedQuest = dialogData.questPool[randomIndex];

        // Cơ chế Clear HandIn: Nếu chọn trúng quest đã từng làm xong, reset nó để cho nhận lại
        QuestController.Instance.ResetQuestStatus(currentOfferedQuest.questId);
        Debug.Log($"NPC {dialogData.npcName} offering random quest: {currentOfferedQuest.questName}");
    }

    void NextLine()
    {
        // if(isTyping)
        // {
        //     StopAllCoroutines();
        //     dialogueUI.SetDialogText(dialogData.dialogLines[dialogIndex]);
        //     isTyping = false;
        // }

        // dialogueUI.ClearChoices();

        // if(dialogData.endDialogLines.Length > dialogIndex && dialogData.endDialogLines[dialogIndex])
        // {
        //     EndDialog();
        //     return;
        // }

        // foreach(DialogueChoice choice in dialogData.dialogChoices)
        // {
        //     if(choice.dialogueIndex == dialogIndex)
        //     {   
        //         Debug.Log("Displaying choices for dialogue index: " + dialogIndex);
        //         DisplayChoices(choice);
        //         return;
        //     }
        // }

       
            if(isTyping)
            {
                StopAllCoroutines();
                dialogueUI.SetDialogText(dialogData.dialogLines[dialogIndex]);
                isTyping = false;
                AudioManager.Instance.StopDialogVoice();
            }

            dialogueUI.ClearChoices();

            if(dialogData.endDialogLines.Length > dialogIndex && dialogData.endDialogLines[dialogIndex])
            {
                EndDialog();
            }
            else
                foreach(DialogueChoice choice in dialogData.dialogChoices)
                {
                    if(choice.dialogueIndex == dialogIndex)
                    {   
                        DisplayChoices(choice);

                    }
                }


            if(dialogData.autoProgressLines.Length > dialogIndex && dialogData.autoProgressLines[dialogIndex])
            {
                if (IsQuestGiver && currentQuestState == QuestState.NotStarted && dialogData.questTransitionIndexs.Length > dialogIndex && dialogData.questTransitionIndexs[dialogIndex])
                {
                    dialogIndex = targetQuestStartIndex; 
                }
                else
                {
                    dialogIndex++;
                }

                if (dialogIndex < dialogData.dialogLines.Length)
                {
                    DisplayCurrentLine();
                }
                else
                {
                    EndDialog();
                }
            }
        

       
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogText("");

        string line = dialogData.dialogLines[dialogIndex];
        AudioManager.Instance.PlayDialogVoice();
        foreach(char letter in line.ToCharArray())
        {
            dialogueUI.SetDialogText(dialogueUI.dialogText.text + letter);
            yield return new WaitForSecondsRealtime(dialogData.typingSpeed);
        }
        isTyping = false;
        AudioManager.Instance.StopDialogVoice();

        // if(isTyping)
        // {
        //     // StopAllCoroutines();
        //     dialogueUI.SetDialogText(dialogData.dialogLines[dialogIndex]);
        //     isTyping = false;
        // }

        // dialogueUI.ClearChoices();

        // if(dialogData.endDialogLines.Length > dialogIndex && dialogData.endDialogLines[dialogIndex])
        // {
        //     EndDialog();
        // }
        // else
        //     foreach(DialogueChoice choice in dialogData.dialogChoices)
        //     {
        //         if(choice.dialogueIndex == dialogIndex)
        //         {   
        //             DisplayChoices(choice);

        //         }
        //     }


        // if(dialogData.autoProgressLines.Length > dialogIndex && dialogData.autoProgressLines[dialogIndex])
        // {
            yield return new WaitForSecondsRealtime(dialogData.autoProgressDelay);
            
            NextLine();
        // }

    //    isTyping = true;
    // dialogueUI.SetDialogText("");

    // string line = dialogData.dialogLines[dialogIndex];
    // bool isAutoLine = dialogData.autoProgressLines.Length > dialogIndex && dialogData.autoProgressLines[dialogIndex];

    // // --- PHẦN 1: GÕ CHỮ ---
    // foreach (char letter in line.ToCharArray())
    // {
    //     dialogueUI.SetDialogText(dialogueUI.dialogText.text + letter);

    //     // Nếu nhấn Space, hiện hết chữ và dừng gõ
    //     if (isAutoLine && Input.GetKeyDown(KeyCode.Space))
    //     {
    //         dialogueUI.SetDialogText(line);
    //         break; 
    //     }

    //     yield return new WaitForSecondsRealtime(dialogData.typingSpeed);
    // }
    
    // isTyping = false;

    // // --- QUAN TRỌNG: Đợi 1 khung hình để "xả" phím Space cũ ---
    // // Điều này ngăn việc 1 lần nhấn Space bị tính cho cả 2 hành động
    // yield return null; 

    // // --- PHẦN 2: HIỂN THỊ CHOICE (Nếu có) ---
    // dialogueUI.ClearChoices();
    // bool hasChoices = false;
    // foreach (DialogueChoice choice in dialogData.dialogChoices)
    // {
    //     if (choice.dialogueIndex == dialogIndex)
    //     {
    //         DisplayChoices(choice);
    //         hasChoices = true;
    //     }
    // }

    // if (dialogData.endDialogLines.Length > dialogIndex && dialogData.endDialogLines[dialogIndex])
    // {
    //     EndDialog();
    //     yield break;
    // }

    // // --- PHẦN 3: ĐỢI AUTO PROGRESS ---
    // // Chỉ đợi nếu là dòng Auto và KHÔNG có lựa chọn (Choices) nào hiện ra
    // if (isAutoLine && !hasChoices)
    // {
    //     float timer = 0;
    //     float maxWait = dialogData.autoProgressDelay;

    //     while (timer < maxWait)
    //     {
    //         // Kiểm tra phím Space ở mỗi khung hình
    //         if (Input.GetKeyDown(KeyCode.Space))
    //         {
    //             break; // Thoát vòng lặp chờ ngay lập tức
    //         }

    //         timer += Time.unscaledDeltaTime; // Dùng unscaled để hoạt động khi pause
    //         yield return null; 
    //     }
        
    //     NextLine();
    // }
    }

    void DisplayChoices(DialogueChoice choice)
    {

        for(int i = 0; i < choice.choices.Length; i++)
        { 
            int nextIndex = choice.nextDialogueIndices[i];
            bool giveQuest = choice.givesQuest[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, giveQuest));
        }
    }

    void ChooseOption(int nextIndex, bool giveQuest)
    {   Debug.Log("Choice selected, next dialogue index: " + nextIndex + ", giveQuest: " + giveQuest);
        if (giveQuest && currentOfferedQuest != null)
        {
            QuestController.Instance.AcceptQuest(currentOfferedQuest);
            currentQuestState = QuestState.InProgress;
        }
        dialogIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialog()
    {
        if(currentQuestState == QuestState.Completed && currentOfferedQuest != null)
        {
            HandleQuestCompletion(currentOfferedQuest);
            lastQuestHandInTime = Time.time; // Bắt đầu tính Cooldown
            currentOfferedQuest = null;      // Reset để lần sau random quest mới
            currentQuestState = QuestState.OnCooldown;
        }
        StopAllCoroutines();
        StartCoroutine(DelayEndDialog());
        // isDialogActive = false;
        // dialogueUI.SetDialogText("");
        // dialogueUI.ShowDialogUI(false);
        // PauseController.setPause(false);
    }

    IEnumerator DelayEndDialog()
    {
        yield return new WaitForSecondsRealtime(1f);
        AudioManager.Instance.StopDialogVoice();
        isDialogActive = false;
        dialogueUI.SetDialogText("");
        dialogueUI.ShowDialogUI(false);
        // PauseController.setPause(false);
    }

    void HandleQuestCompletion(QuestData quest)
    {
        RewardManager.Instance.GrantRewards(quest);
        QuestController.Instance.HandInQuest(quest.questId);
        
    }
}