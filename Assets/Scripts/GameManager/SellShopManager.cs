using UnityEngine;

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

        InventorySlot[] backpack = InventoryManager.Instance.backpackSlots;

        foreach (InventorySlot slot in backpack)
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