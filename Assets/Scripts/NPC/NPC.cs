
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
    private enum QuestState { NotStarted, InProgress, Completed }
    private QuestState currentQuestState = QuestState.NotStarted;
    void Start()
    {
        dialogueUI = DialogueController.Instance;   
    }

    public void Interact()
    {
        if(dialogData == null || (PauseController.IsGamePaused && !isDialogActive) )
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
        return !isDialogActive;
    }

    void StartDialog()
    {
        SyncQuestState();

        if(currentQuestState == QuestState.NotStarted)
        {
            dialogIndex = 0;
        }
        else if(currentQuestState == QuestState.InProgress)
        {
            dialogIndex = dialogData.questInProgressIndex;
        }
        else if(currentQuestState == QuestState.Completed)
        {
            dialogIndex = dialogData.questCompletedIndex;
        }

        isDialogActive = true;
        
        dialogueUI.SetNPCInfo(dialogData.npcName, dialogData.npcPortrait);

        dialogueUI.ShowDialogUI(true);
        PauseController.setPause(true);

        DisplayCurrentLine();
    }

    private void SyncQuestState()
    {
        if(dialogData.quest == null) return;

        string questId = dialogData.quest.questId;

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

       
            
            if(++dialogIndex < dialogData.dialogLines.Length)
            {
                DisplayCurrentLine();
            }
            else
            {
                EndDialog();
            }
        

       
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogText("");

        string line = dialogData.dialogLines[dialogIndex];
        foreach(char letter in line.ToCharArray())
        {
            dialogueUI.SetDialogText(dialogueUI.dialogText.text + letter);
            yield return new WaitForSeconds(dialogData.typingSpeed);
        }
        isTyping = false;


        if(isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogText(dialogData.dialogLines[dialogIndex]);
            isTyping = false;
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
                    Debug.Log("Displaying choices for dialogue index: " + dialogIndex);
                    DisplayChoices(choice);

                }
            }


        if(dialogData.autoProgressLines.Length > dialogIndex && dialogData.autoProgressLines[dialogIndex])
        {
            yield return new WaitForSeconds(dialogData.autoProgressDelay);
            NextLine();
        }
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
    {
        if(giveQuest)
        {
            QuestController.Instance.AcceptQuest(dialogData.quest);
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
        if(currentQuestState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogData.quest.questId))
        {
           HandleQuestCompletion(dialogData.quest);
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
        yield return new WaitForSeconds(1f);
        isDialogActive = false;
        dialogueUI.SetDialogText("");
        dialogueUI.ShowDialogUI(false);
        PauseController.setPause(false);
    }

    void HandleQuestCompletion(QuestData quest)
    {
        RewardManager.Instance.GrantRewards(quest);
        QuestController.Instance.HandInQuest(quest.questId);
        
    }
}