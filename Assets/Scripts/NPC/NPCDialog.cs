using UnityEngine;

[CreateAssetMenu(fileName = "newNPCDialog", menuName ="NPC Dialog")]
public class NPCDialog: ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogLines;
    public bool[] autoProgressLines;
    public bool[] endDialogLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogChoice[] dialogChoices;

}