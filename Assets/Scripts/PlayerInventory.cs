using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<string, int> items = new();

    public bool HasItem(string item, int amount)
    {
        if (!items.ContainsKey(item)) return false;
        return items[item] >= amount;
    }

    public void AddItem(string item, int amount)
    {
        if (!items.ContainsKey(item))
            items[item] = 0;

        items[item] += amount;
    }

    public bool RemoveItem(string item, int amount)
    {
        if (!HasItem(item, amount)) return false;

        items[item] -= amount;

        if (items[item] <= 0)
            items.Remove(item);

        return true;
    }

    public int GetItemAmount(string item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }
}
