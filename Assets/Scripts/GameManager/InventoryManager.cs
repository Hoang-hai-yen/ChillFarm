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

    public void ForceSetSlot(int index)
    {
        if (index < 0) index = 0;
        if (index >= hotbarSlots.Length) index = hotbarSlots.Length - 1;

        SelectedHotbarSlot = index;
        OnInventoryChanged?.Invoke();
    }


    public ItemData GetSelectedItem()
    {
        return hotbarSlots[SelectedHotbarSlot].itemData;
    }

public void AddItem(ItemData item, int slotIndex, int count = 1)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length) return;
        
        InventorySlot slot = hotbarSlots[slotIndex];

        if (slot.itemData == null)
        {
            slot.itemData = item;
            slot.quantity = count;
        }
        else if (slot.itemData == item) 
        {
            slot.quantity += count;
        }
        else
        {
            Debug.LogWarning("Ô đã có vật phẩm khác!");
            return; 
        }

        OnInventoryChanged?.Invoke();
    }
    
    void Start()
    {
        ItemData hoe = Resources.Load<ToolData>("Test_Hoe");
        ItemData waterCan = Resources.Load<ToolData>("Test_WaterCan");

        if (hoe != null) AddItem(hoe, 0); 
        if (waterCan != null) AddItem(waterCan, 1); 
    }
}