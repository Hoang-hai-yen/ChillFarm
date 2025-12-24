using System;
using UnityEngine;

[System.Serializable]
public class DialogChoice
{
    [SerializeField] private string optionText;
    [SerializeField] private string responseText;
    [SerializeField] private Action onSelected;

    public string OptionText => optionText;
    public string ResponseText => responseText;
    public Action OnSelected => onSelected;

    public DialogChoice(string optionText, string responseText, Action onSelected = null)
    {
        this.optionText = optionText;
        this.responseText = responseText;
        this.onSelected = onSelected;
    }
}
