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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
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

        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    void OpenShop()
    {
        isShopOpen = true;
        ShopManager.Instance.OpenShop(products);
    }

    void CloseShop()
    {
        isShopOpen = false;
        ShopManager.Instance.CloseShop();
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