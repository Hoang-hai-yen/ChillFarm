using UnityEngine;
using TMPro;

public class BarnInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject barnCanvas;
    public TextMeshProUGUI maxText;

    [Header("Config")]
    public int maxValue = 0;
    public int increasePerPress = 3;

    private bool playerInRange = false;

    void Start()
    {
        if (barnCanvas != null)
            barnCanvas.SetActive(false);

        UpdateMaxText();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            maxValue += increasePerPress;
            UpdateMaxText();
        }
    }

    void UpdateMaxText()
    {
        if (maxText != null)
            maxText.text = maxValue.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (barnCanvas != null)
                barnCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (barnCanvas != null)
                barnCanvas.SetActive(false);
        }
    }
}
