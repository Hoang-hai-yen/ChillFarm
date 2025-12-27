using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("UI Setup")]
    public GameObject shopPanel;      
    public Transform contentPanel;   
    public GameObject shopItemPrefab; 
    
    [Header("Optional")]
    public GameObject hotbarPanel;    

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void OpenShop(List<ItemData> items)
    {
        if (shopPanel != null) shopPanel.SetActive(true);
        if (hotbarPanel != null) hotbarPanel.SetActive(false);

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in items)
        {
            GameObject newSlot = Instantiate(shopItemPrefab, contentPanel);
            ShopItemUI uiScript = newSlot.GetComponent<ShopItemUI>();
            
            if (uiScript != null)
            {
                uiScript.SetItemData(item);
            }
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (hotbarPanel != null) hotbarPanel.SetActive(true);
    }
}