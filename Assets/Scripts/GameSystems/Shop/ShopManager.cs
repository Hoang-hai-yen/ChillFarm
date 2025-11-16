using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public PlayerStats playerStats;

    public void SellItem(string itemName, int quantity)
    {
        InventoryManager invManager = InventoryManager.Instance;
        if (invManager == null)
        {
            Debug.LogError("ShopManager: Không tìm thấy InventoryManager!");
            return; 
        }

        if (playerStats == null)
        {
            Debug.LogError("ShopManager: Chưa gán PlayerStats vào Inspector!");
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
            playerStats.gold += totalGold; 
            Debug.Log($"Đã bán {itemName} x{quantity} được {totalGold} vàng.");
        }
        else
        {
            Debug.LogError($"Lỗi: Không thể xóa {itemName} khỏi inventory.");
        }
    }
}
