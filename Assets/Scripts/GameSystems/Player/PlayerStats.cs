using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int gold;
    public int XP;
    public int level = 1;
    public int XPToNextLevel = 100;

    public void GainXP(int amount)
    {
        XP += amount;
        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        XPToNextLevel = Mathf.RoundToInt(XPToNextLevel * 1.25f);
    }
    
    public void SellItem(string itemName, int quantity)
    {
        InventoryManager invManager = InventoryManager.Instance;
        if (invManager == null)
        {
            Debug.LogError("PlayerStats: Không tìm thấy InventoryManager!");
            return; 
        }

        InventorySlot itemSlot = invManager.FindSlotByName(itemName);
        
        if (itemSlot == null || itemSlot.quantity < quantity)
        {
            Debug.Log($"Không tìm thấy item '{itemName}' hoặc không đủ số lượng để bán.");
            return;
        }

        int totalGold = itemSlot.itemData.sellPrice * quantity;

        bool removed = invManager.Remove(itemSlot.itemData, quantity);

        if (removed)
        {
            this.gold += totalGold; 
            Debug.Log($"Đã bán {itemName} x{quantity} được {totalGold} vàng.");
        }
        else
        {
            Debug.LogError($"Lỗi: Không thể xóa {itemName} khỏi inventory.");
        }
    }
}
