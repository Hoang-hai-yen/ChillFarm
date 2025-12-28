using UnityEngine;
using TMPro;

public class ShippingBin : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text pressText;

    private bool playerInRange = false;
    private bool isOpen = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                SellShopManager.Instance.OpenSellShop();
                isOpen = true;
            }
            else
            {
                SellShopManager.Instance.CloseSellShop();
                isOpen = false;
            }
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            SellShopManager.Instance.CloseSellShop();
            isOpen = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressText != null) 
            {
                pressText.text = "Sell Items (E)";
                pressText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressText != null) pressText.gameObject.SetActive(false);
            
            if (isOpen)
            {
                SellShopManager.Instance.CloseSellShop();
                isOpen = false;
            }
        }
    }
}