using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    [Header("Shop Data")]
    [Tooltip("Kéo thả các vật phẩm cửa hàng này bán vào đây")]
    public List<ItemData> products; 

    [Header("UI Hints")]
    public TMP_Text pressText; 

    private bool playerInRange = false;
    private bool isShopOpen = false;

    void Start()
    {
        if (pressText != null) pressText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isShopOpen)
                {
                    OpenShop();
                }
                else
                {
                    CloseShop();
                }
            }
        }

    }

    void OpenShop()
    {
        isShopOpen = true;
        
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop(products);
        }
        
        if (pressText != null) pressText.gameObject.SetActive(false);
    }

    void CloseShop()
    {
        isShopOpen = false;

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.CloseShop();
        }

        if (pressText != null && playerInRange) 
        {
            pressText.text = "Press E";
            pressText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressText != null) 
            {
                pressText.text = "Press E";
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
            
            if (isShopOpen) CloseShop();
        }
    }
}