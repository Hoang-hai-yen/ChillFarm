using UnityEngine;

public class QuestLogsToggle : MonoBehaviour
{
    [SerializeField] private GameObject questLogs;

    private bool isOpen = false;

    void Start()
    {
        if (questLogs != null)
            questLogs.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        { 
            ToggleUpgrade();
        }
    }

    void ToggleUpgrade()
    {
        isOpen = !isOpen;
        questLogs.SetActive(isOpen);

        // (Optional) pause game khi mở UI
        Time.timeScale = isOpen ? 0f : 1f;
    }
}
