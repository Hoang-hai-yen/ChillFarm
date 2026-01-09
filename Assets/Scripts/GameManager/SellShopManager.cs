using UnityEngine;
using System.Collections.Generic;

public class SellShopManager : MonoBehaviour
{
    public static SellShopManager Instance;

    [Header("UI References")]
    public GameObject sellPanel;
    public Transform contentPanel;
    public GameObject sellSlotPrefab; 
    
    [Header("Settings")]
    public GameObject hotbarPanel; 

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (sellPanel != null) sellPanel.SetActive(false);
    }

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshShop;
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshShop;
        }
    }

    public void OpenSellShop()
    {
        sellPanel.SetActive(true);
        if (hotbarPanel != null) hotbarPanel.SetActive(false);
        RefreshShop(); 
    }

    public void CloseSellShop()
    {
        if (sellPanel == null) return; 

        sellPanel.SetActive(false);
        
        if (hotbarPanel != null) hotbarPanel.SetActive(true);
    }

    private void RefreshShop()
    {
        if (!sellPanel.activeSelf) return;

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        System.Collections.Generic.List<InventorySlot> allSlotsToSell = new System.Collections.Generic.List<InventorySlot>();

        if (InventoryManager.Instance.backpackSlots != null)
        {
            allSlotsToSell.AddRange(InventoryManager.Instance.backpackSlots);
        }

        if (InventoryManager.Instance.hotbarSlots != null)
        {
            for (int i = 1; i < InventoryManager.Instance.hotbarSlots.Length; i++)
            {
                allSlotsToSell.Add(InventoryManager.Instance.hotbarSlots[i]);
            }
        }

        foreach (InventorySlot slot in allSlotsToSell)
        {
            if (slot != null && slot.itemData != null)
            {
                GameObject newSlot = Instantiate(sellSlotPrefab, contentPanel);
                SellSlotUI uiScript = newSlot.GetComponent<SellSlotUI>();
                
                if (uiScript != null)
                {
                    uiScript.SetSlotData(slot);
                }
            }
        }
    }
}