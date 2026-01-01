using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndices;
    public bool[] givesQuest;

    
}