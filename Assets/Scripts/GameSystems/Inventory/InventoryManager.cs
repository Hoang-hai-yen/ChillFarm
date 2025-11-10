using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int quantity;
    public int sellPrice;
}

public class InventoryManager : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public int maxSlots = 20;

    public bool AddItem(Item newItem)
    {
        Item existing = inventory.Find(i => i.itemName == newItem.itemName);
        if (existing != null)
        {
            existing.quantity += newItem.quantity;
            return true;
        }
        if (inventory.Count < maxSlots)
        {
            inventory.Add(newItem);
            return true;
        }
        return false;
    }

    public bool RemoveItem(string itemName, int quantity)
    {
        Item existing = inventory.Find(i => i.itemName == itemName);
        if (existing == null || existing.quantity < quantity)
            return false;
        existing.quantity -= quantity;
        if (existing.quantity <= 0)
            inventory.Remove(existing);
        return true;
    }
}
