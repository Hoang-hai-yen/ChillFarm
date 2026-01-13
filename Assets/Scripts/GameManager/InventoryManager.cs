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
    [Header("Audio - Shop")]
    [Tooltip("Âm thanh khi mua thành công (Tiếng tiền lẻ, Kaching...)")]
    public AudioClip buySuccessSound;
    [Tooltip("Âm thanh khi mua thất bại (Tiếng Buzz, Error...)")]
    public AudioClip buyFailSound;
    private AudioSource audioSource;
    public int SelectedHotbarSlot { get; private set; } = 0; 
    public event Action OnInventoryChanged;

    Dictionary<string, int> itemsCountCache = new ();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

        RebuildItemCounts();
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
                RebuildItemCounts();
                // OnInventoryChanged?.Invoke();
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
                RebuildItemCounts();
                // OnInventoryChanged?.Invoke();
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
                RebuildItemCounts();
                // OnInventoryChanged?.Invoke();
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
                RebuildItemCounts();
                // OnInventoryChanged?.Invoke();
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
                    // OnInventoryChanged?.Invoke();
                    RebuildItemCounts();
                    return true;
                }
            }
        }

        for (int i = 0; i < backpackSlots.Length; i++)
        {
            InventorySlot slot = backpackSlots[i];
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
                    RebuildItemCounts();
                    // OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        

        return false; 
    }

    public bool RemoveItemById(string itemId, int count = 1)
    {
        InventorySlot currentSlot = hotbarSlots[SelectedHotbarSlot];
        
        if (currentSlot.itemData.itemId == itemId)
        {
            if (currentSlot.quantity >= count)
            {
                currentSlot.quantity -= count;
                if (currentSlot.quantity <= 0)
                {
                    currentSlot.itemData = null;
                    currentSlot.quantity = 0;
                }
                RebuildItemCounts();
                // OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            InventorySlot slot = hotbarSlots[i];
            if (slot.itemData.itemId == itemId)
            {
                if (slot.quantity >= count)
                {
                    slot.quantity -= count;
                    if (slot.quantity <= 0)
                    {
                        slot.itemData = null;
                        slot.quantity = 0;
                    }
                    // OnInventoryChanged?.Invoke();
                    RebuildItemCounts();
                    return true;
                }
            }
        }

        for (int i = 0; i < backpackSlots.Length; i++)
        {
            InventorySlot slot = backpackSlots[i];
            if (slot.itemData.itemId == itemId)
            {
                if (slot.quantity >= count)
                {
                    slot.quantity -= count;
                    if (slot.quantity <= 0)
                    {
                        slot.itemData = null;
                        slot.quantity = 0;
                    }
                    RebuildItemCounts();
                    // OnInventoryChanged?.Invoke();
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
            
            if (buySuccessSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buySuccessSound);
            }

            Debug.Log($"Đã bán {item.itemName} nhận được {totalEarned} G");
        }
        else
        {
            Debug.Log("Không còn vật phẩm này để bán!");
        }
    }
    public bool BuyItem(ItemData item, int price)
    {
        if (currentGold < price)
        {
            Debug.Log("Không đủ tiền!");
            
            if (buyFailSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buyFailSound);
            }
            return false;
        }

        bool added = AddItem(item, 1);
        
        if (added)
        {
            currentGold -= price;
            OnGoldChanged?.Invoke(currentGold);

            if (buySuccessSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buySuccessSound);
            }

            Debug.Log($"Mua thành công {item.itemName}. Tiền còn: {currentGold}");
            return true;
        }
        else
        {
            Debug.Log("Túi đồ đã đầy! Không thể mua thêm.");
            if (buyFailSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buyFailSound);
            }
            return false; 
        }
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach(var slot in hotbarSlots)
        {
            if (slot.itemData != null)
            {
                string itemId = slot.itemData.itemId;
                if (itemsCountCache.ContainsKey(itemId))
                    itemsCountCache[itemId] += slot.quantity;
                else
                    itemsCountCache[itemId] = slot.quantity;
                Debug.Log($"Đếm vật phẩm trong hotbar: {itemId} = {itemsCountCache[itemId]}");
            }
        }

        foreach(var slot in backpackSlots)
        {
            if (slot.itemData != null)
            {
                string itemId = slot.itemData.itemId;
                if (itemsCountCache.ContainsKey(itemId))
                    itemsCountCache[itemId] += slot.quantity;
                else
                    itemsCountCache[itemId] = slot.quantity;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public Dictionary<string, int> GetItemCounts() => itemsCountCache;
}