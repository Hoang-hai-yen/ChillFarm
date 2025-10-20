using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    public TimeController timeController;
    private TextMeshProUGUI clockText;

    void Start()
    {
        clockText = GetComponent<TextMeshProUGUI>();
        if (timeController == null)
        {
            timeController = FindObjectOfType<TimeController>();
        }
    }

    void Update()
    {
        if (timeController != null && clockText != null)
        {
            clockText.text = $"Day {timeController.GetCurrentDay()} - {timeController.GetFormattedTime()}";
        }
    }
}