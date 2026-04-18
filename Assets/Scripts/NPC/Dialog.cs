using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] private string npcText;
    [SerializeField] private List<DialogChoice> choices;

    public string NpcText => npcText;
    public List<DialogChoice> Choices => choices;

    public Dialog(string npcText, List<DialogChoice> choices)
    {
        this.npcText = npcText;
        this.choices = choices;
    }
}
