using UnityEngine;

public class CharacterUpgradeToggle : MonoBehaviour
{
    [SerializeField] private GameObject characterUpgradeUI;

    private bool isOpen = false;

    void Start()
    {
        if (characterUpgradeUI != null)
            characterUpgradeUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleUpgrade();
        }
    }

    void ToggleUpgrade()
    {
        isOpen = !isOpen;
        characterUpgradeUI.SetActive(isOpen);

        // (Optional) pause game khi mở UI
        Time.timeScale = isOpen ? 0f : 1f;
    }
}
