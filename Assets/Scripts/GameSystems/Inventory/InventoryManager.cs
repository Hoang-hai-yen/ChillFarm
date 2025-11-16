using UnityEngine;
using System.Collections.Generic;
using System;

// Dùng class InventorySlot từ file 1, vì nó dùng ItemData (ScriptableObject)
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    // Thêm một constructor để tạo slot mới cho tiện
    public InventorySlot(ItemData data, int quant)
    {
        itemData = data;
        quantity = quant;
    }

    // Constructor rỗng mà file 1 dùng
    public InventorySlot()
    {
        itemData = null;
        quantity = 0;
    }
}

// CHỈ CÓ MỘT CLASS InventoryManager
public class InventoryManager : MonoBehaviour
{
    // --- Singleton (Thứ mà PlayerController đang tìm) ---
    public static InventoryManager Instance { get; private set; }
    public event Action OnInventoryChanged;

    [Header("Hotbar (Thanh công cụ)")]
    public InventorySlot[] hotbarSlots = new InventorySlot[9];
    public int SelectedHotbarSlot { get; private set; } = 0; 

    [Header("Backpack (Túi đồ)")]
    public List<InventorySlot> backpackSlots = new List<InventorySlot>();
    public int backpackSize = 20; // Lấy từ File 2 (maxSlots)

    void Awake()
    {
        // --- Singleton ---
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // --- Khởi tạo Hotbar ---
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] == null) hotbarSlots[i] = new InventorySlot();
        }
    }
    
    void Start()
    {
        // --- Code test (để thêm item vào hotbar) ---
        ItemData hoe = Resources.Load<ToolData>("Test_Hoe");
        ItemData waterCan = Resources.Load<ToolData>("Test_WaterCan");
        ItemData carrotSeed = Resources.Load<SeedData>("CarrotSeed_Data");
        
        if (hoe != null) AddItemToHotbarSlot(hoe, 0, 1); 
        if (waterCan != null) AddItemToHotbarSlot(waterCan, 1, 1); 
        if (carrotSeed != null) AddItemToHotbarSlot(carrotSeed, 2, 1); 
    }

    // --- Quản lý Hotbar ---
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
    
    // (Hàm này chỉ để chạy code test ở Start)
    public void AddItemToHotbarSlot(ItemData item, int slotIndex, int quantity)
    {
         if (slotIndex < 0 || slotIndex >= hotbarSlots.Length) return;
         
         hotbarSlots[slotIndex].itemData = item;
         hotbarSlots[slotIndex].quantity = quantity;
         OnInventoryChanged?.Invoke();
    }


    // --- Hàm `Add` mới (kết hợp logic File 2) ---
    public bool Add(ItemData item, int quantity)
    {
        // 1. Ưu tiên stack vào các ô đã có trong Hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.itemData == item)
            {
                slot.quantity += quantity;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 2. Stack vào các ô đã có trong Backpack
        foreach (InventorySlot slot in backpackSlots)
        {
            if (slot.itemData == item)
            {
                slot.quantity += quantity;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 3. Thêm vào ô trống đầu tiên trong Hotbar
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].itemData == null)
            {
                hotbarSlots[i].itemData = item;
                hotbarSlots[i].quantity = quantity;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 4. Thêm vào ô trống trong Backpack
        if (backpackSlots.Count < backpackSize)
        {
            backpackSlots.Add(new InventorySlot(item, quantity));
            OnInventoryChanged?.Invoke();
            return true;
        }

        // 5. Hòm đồ đầy
        Debug.Log("Inventory Full!");
        return false;
    }

    public bool Remove(ItemData itemToRemove, int quantity)
    {
        // --- SỬA ---
        InventorySlot slot = FindSlotByData(itemToRemove); // Sửa tên hàm gọi
        // --- KẾT THÚC SỬA ---
        
        if (slot == null || slot.quantity < quantity)
        {
            return false;
        }

        slot.quantity -= quantity;

        if (slot.quantity <= 0)
        {
            if (backpackSlots.Contains(slot))
            {
                backpackSlots.Remove(slot);
            }
            else
            {
                slot.itemData = null;
                slot.quantity = 0;
            }
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    // --- SỬA HÀM CŨ ---
    // Đổi tên hàm này từ "FindSlot" thành "FindSlotByData"
    private InventorySlot FindSlotByData(ItemData itemToFind)
    {
        // Kiểm tra hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.itemData == itemToFind) return slot;
        }
        // Kiểm tra backpack
        foreach (InventorySlot slot in backpackSlots)
        {
            if (slot.itemData == itemToFind) return slot;
        }
        return null;
    }
    public InventorySlot FindSlotByName(string itemName)
    {
        // Kiểm tra hotbar
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.itemData != null && slot.itemData.itemName == itemName) return slot;
        }
        // Kiểm tra backpack
        foreach (InventorySlot slot in backpackSlots)
        {
            if (slot.itemData != null && slot.itemData.itemName == itemName) return slot;
        }
        return null;
    }
}