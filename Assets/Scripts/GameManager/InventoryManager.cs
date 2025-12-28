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

    [Header("Money")]
    public int currentGold = 500; 
    public event Action<int> OnGoldChanged; 

    [Header("Inventory Data")]
    public InventorySlot[] hotbarSlots = new InventorySlot[9];
    public InventorySlot[] backpackSlots = new InventorySlot[20];
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
        for (int i = 0; i < backpackSlots.Length; i++)
        {
            if (backpackSlots[i] == null) backpackSlots[i] = new InventorySlot();
        }
    }

    void Start()
    {
        ItemData hoe = Resources.Load<ToolData>("Test_Hoe");
        ItemData waterCan = Resources.Load<ToolData>("Test_WaterCan");

        if (hoe != null) AddItem(hoe, 1); 
        if (waterCan != null) AddItem(waterCan, 1); 
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
        InventorySlot slot = hotbarSlots[SelectedHotbarSlot];
        return slot != null ? slot.itemData : null;
    }

    public bool AddItem(ItemData item, int count = 1)
    {
        for (int i = 1; i < hotbarSlots.Length; i++) 
        {
            InventorySlot slot = hotbarSlots[i];
            if (slot.itemData == item)
            {
                slot.quantity += count;
                OnInventoryChanged?.Invoke();
                return true; 
            }
        }
        
        for (int i = 1; i < hotbarSlots.Length; i++)
        {
            InventorySlot slot = hotbarSlots[i];
            if (slot.itemData == null)
            {
                slot.itemData = item;
                slot.quantity = count;
                OnInventoryChanged?.Invoke();
                return true; 
            }
        }

        for (int i = 0; i < backpackSlots.Length; i++)
        {
            InventorySlot slot = backpackSlots[i];
            if (slot.itemData == null)
            {
                slot.itemData = item;
                slot.quantity = count;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Inventory Full!");
        return false;
    }

    public bool RemoveItem(ItemData itemToRemove, int count = 1)
    {
        InventorySlot currentSlot = hotbarSlots[SelectedHotbarSlot];
        
        if (currentSlot.itemData == itemToRemove)
        {
            if (currentSlot.quantity >= count)
            {
                currentSlot.quantity -= count;
                if (currentSlot.quantity <= 0)
                {
                    currentSlot.itemData = null;
                    currentSlot.quantity = 0;
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            InventorySlot slot = hotbarSlots[i];
            if (slot.itemData == itemToRemove)
            {
                if (slot.quantity >= count)
                {
                    slot.quantity -= count;
                    if (slot.quantity <= 0)
                    {
                        slot.itemData = null;
                        slot.quantity = 0;
                    }
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        return false; 
    }
    public void SwapItems(int indexA, bool isHotbarA, int indexB, bool isHotbarB)
    {
        InventorySlot[] listA = isHotbarA ? hotbarSlots : backpackSlots;
        InventorySlot[] listB = isHotbarB ? hotbarSlots : backpackSlots;

        InventorySlot slotDataA = listA[indexA];
        InventorySlot slotDataB = listB[indexB];

        ItemData tempItem = slotDataA.itemData;
        int tempQty = slotDataA.quantity;

        slotDataA.itemData = slotDataB.itemData;
        slotDataA.quantity = slotDataB.quantity;

        slotDataB.itemData = tempItem;
        slotDataB.quantity = tempQty;

        OnInventoryChanged?.Invoke();
    }

    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold); 
            Debug.Log($"Mua thành công! Còn lại: {currentGold} G");
            return true;
        }
        
        Debug.Log("Không đủ tiền!");
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public void SellItem(ItemData item, int quantity = 1)
    {
        if (item == null) return;

        bool isRemoved = RemoveItem(item, quantity);

        if (isRemoved)
        {
            int totalEarned = item.sellPrice * quantity;
            AddGold(totalEarned);
            Debug.Log($"Đã bán {item.itemName} nhận được {totalEarned} G");
        }
        else
        {
            Debug.Log("Không còn vật phẩm này để bán!");
        }
    }
}