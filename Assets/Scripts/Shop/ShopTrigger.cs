using UnityEngine;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject hotbarPanel;
    public GameObject Key;
    public GameObject UI;
    public TMP_Text pressFText;

    private bool playerInRange = false;
    private bool shopOpen = false;

    void Start()
    {
        if (pressFText != null) pressFText.gameObject.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            shopOpen = !shopOpen;
            shopPanel.SetActive(shopOpen);
            hotbarPanel.SetActive(!shopOpen);
            Key.SetActive(!shopOpen);
            UI.SetActive(!shopOpen);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressFText != null) pressFText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressFText != null) pressFText.gameObject.SetActive(false);
            if (shopPanel != null) shopPanel.SetActive(false);
            if (hotbarPanel != null) hotbarPanel.SetActive(true);
            if (Key != null) Key.SetActive(true);   
            if (UI != null) UI.SetActive(true);
            shopOpen = false;
        }
    }
}
    