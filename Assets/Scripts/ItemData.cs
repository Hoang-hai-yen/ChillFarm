using UnityEngine;

public enum ItemType { Tool, Seed, Crop, Resource, Fertilizer, AnimalFood, AnimalProduct }

public enum AnimalType { Chicken, Cow }
public enum AnimalTier { Normal, Medium, High } 

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Generic Item")] 
// --------------------
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public float staminaCost = 5f; 
}