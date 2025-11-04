using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Data")]
    public InventorySlot[] hotbarSlots = new InventorySlot[9];
    public int SelectedHotbarSlot { get; private set; } = 0; 
    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == null) hotbarSlots[i] = new InventorySlot();
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;
        if (SelectedHotbarSlot == index) return; 

        SelectedHotbarSlot = index;
        Debug.Log($"InventoryManager: Đã chọn ô {index + 1}");
        OnInventoryChanged?.Invoke();
    }

    public ItemData GetSelectedItem()
    {
        return hotbarSlots[SelectedHotbarSlot].itemData;
    }

    public void AddItem(ItemData item, int slotIndex)
    {
         if (slotIndex < 0 || slotIndex >= hotbarSlots.Length) return;
         
         hotbarSlots[slotIndex].itemData = item;
         hotbarSlots[slotIndex].quantity = 1;
         OnInventoryChanged?.Invoke();
    }
    
    void Start()
    {
        ItemData hoe = Resources.Load<ToolData>("Test_Hoe");
        ItemData waterCan = Resources.Load<ToolData>("Test_WaterCan");
        ItemData carrotSeed = Resources.Load<SeedData>("CarrotSeed_Data");
        if (carrotSeed != null) AddItem(carrotSeed, 2); 
        if (hoe != null) AddItem(hoe, 0); 
        if (waterCan != null) AddItem(waterCan, 1); 
    }
}