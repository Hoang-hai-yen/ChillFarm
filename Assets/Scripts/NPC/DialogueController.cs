using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueController: MonoBehaviour
{
    public static DialogueController Instance {get; private set;}
    public GameObject dialogPanel;
    public TMP_Text dialogText, nameText;
    public Image portraitImage;
    public Transform choiceContainter;
    public GameObject choiceButton;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowDialogUI(false);
    }

    public void ShowDialogUI(bool show)
    {
        dialogPanel.SetActive(show);
    }

    public void SetNPCInfo(string npcName, Sprite npcPortrait)
    {
        nameText.SetText(npcName);
        portraitImage.sprite = npcPortrait;
    }

    public void SetDialogText(string text)
    {
        dialogText.SetText(text);
    }

    public void ClearChoices()
    {
        foreach(Transform child in choiceContainter)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateChoiceButton(string choiceText, UnityAction onClickAction)
    {
        GameObject choiceBtnObj = Instantiate(choiceButton, choiceContainter);
        TMP_Text btnText = choiceBtnObj.GetComponentInChildren<TMP_Text>();
        btnText.SetText(choiceText);

        Button btn = choiceBtnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => onClickAction());
    }
}