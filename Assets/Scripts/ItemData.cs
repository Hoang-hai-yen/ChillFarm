using System;
using UnityEngine;

public enum ItemType { Tool, Seed, Crop, Resource, Fertilizer, AnimalFood, AnimalProduct, Livestock, Fish, Mushroom, Food } 
public enum AnimalType { Chicken, Cow }
public enum AnimalTier { Normal, Medium, High } 

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Generic Item")] 
// --------------------
public class ItemData : ScriptableObject
{
    public string itemId;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public float staminaCost = 5f; 
    [Header("Shop Info")]
    public int price = 100;
    [Header("Sell Info")]
    public int sellPrice = 50;

    protected virtual void OnValidate()
    {
        if(String.IsNullOrEmpty(itemId))
        {
            itemId = itemName + "_" + Guid.NewGuid().ToString();
        }
    }


}
