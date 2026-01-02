using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newNPCDialog", menuName ="NPC Dialog")]
public class NPCDialog: ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    [TextArea(3, 10)] public string[] dialogLines;
    public bool[] autoProgressLines;
    public bool[] endDialogLines;
    public float autoProgressDelay = 2f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogueChoice[] dialogChoices;

   [Header("Quest Settings")]
    public List<QuestData> questPool;
    public bool[] questTransitionIndexs; 
    public int[] targetQuestStartIndexs;   
    public int questInProgressIndex;
    public int questCompletedIndex;
    public int questOnCooldownIndex;

}