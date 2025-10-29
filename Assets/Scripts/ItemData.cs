using UnityEngine;

public enum ItemType { Tool, Seed, Crop, Resource }

public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public float staminaCost = 5f; 
}