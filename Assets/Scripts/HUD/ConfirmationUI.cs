using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationUI : MonoBehaviour
{
    public static ConfirmationUI Instance;

    [Header("UI Components")]
    public GameObject panel;
    public TextMeshProUGUI messageText; 
    public Button yesButton;
    public TextMeshProUGUI yesButtonText; 
    public Button noButton;

    private Action onConfirmAction;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panel.SetActive(false);
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void ShowQuestion(string content, int price, Action onConfirm)
    {
        messageText.text = content;
        
        yesButton.gameObject.SetActive(true);
        if (yesButtonText != null) yesButtonText.text = $"{price} G"; 

        onConfirmAction = onConfirm;
        panel.SetActive(true);
    }

    public void ShowNotification(string content)
    {
        messageText.text = content;

        yesButton.gameObject.SetActive(false);


        onConfirmAction = null;
        panel.SetActive(true);
    }

    private void OnYesClicked()
    {
        onConfirmAction?.Invoke();
        panel.SetActive(false);
    }

    private void OnNoClicked()
    {
        panel.SetActive(false);
    }
}