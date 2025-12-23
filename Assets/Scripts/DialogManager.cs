using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TMP_Text dialogText;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;

    [SerializeField] private int lettersPerSecond = 30;

    public event Action OnShowDialog;
    public event Action OnHideDialog;

    private Dialog dialog;
    private bool isTyping;
    private bool isWaitingForClose;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        choicePanel.SetActive(false);
        dialogBox.SetActive(false);
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        this.dialog = dialog;
        isWaitingForClose = false;
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);

        yield return StartCoroutine(TypeDialog(dialog.NpcText));

        if (dialog.Choices != null && dialog.Choices.Count > 0)
        {
            ShowChoices(dialog.Choices);
        }
        else
        {
            isWaitingForClose = true;
        }

        yield return new WaitUntil(() => this.dialog == null);
    }

    private void ShowChoices(List<DialogChoice> choices)
    {
        if (choices.Count < 2) return;

        choicePanel.SetActive(true);

        choiceButton1.GetComponentInChildren<TMP_Text>().text = choices[0].OptionText;
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton1.onClick.AddListener(() => OnChoiceSelected(choices[0]));

        choiceButton2.GetComponentInChildren<TMP_Text>().text = choices[1].OptionText;
        choiceButton2.onClick.RemoveAllListeners();
        choiceButton2.onClick.AddListener(() => OnChoiceSelected(choices[1]));
    }

    private void OnChoiceSelected(DialogChoice choice)
    {
        choicePanel.SetActive(false);
        if (choice.OnSelected != null)
            choice.OnSelected.Invoke();
        else
            StartCoroutine(HandleResponse(choice.ResponseText));
    }

    private IEnumerator HandleResponse(string response)
    {
        yield return StartCoroutine(TypeDialog(response));
        isWaitingForClose = true;
    }

    public void HandleUpdate()
    {
        if (choicePanel.activeSelf || isTyping) return;

        if (Input.GetKeyDown(KeyCode.Z) && isWaitingForClose && dialog != null)
        {
            CloseDialog();
        }
    }

    private void CloseDialog()
    {
        dialogBox.SetActive(false);
        dialog = null;
        isWaitingForClose = false;
        OnHideDialog?.Invoke();
    }

    private IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
