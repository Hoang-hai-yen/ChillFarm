using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public InventoryManager inventory;
    public PlayerStats playerStats;

    public void SellItem(string itemName, int quantity)
    {
        Item item = inventory.inventory.Find(i => i.itemName == itemName);
        if (item == null || item.quantity < quantity)
            return;
        int totalGold = item.sellPrice * quantity;
        inventory.RemoveItem(itemName, quantity);
        playerStats.gold += totalGold;
    }
}
