using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceData", menuName = "Inventory/Resource Data")]
public class ResourceData : FoodData
{

    private void OnValidate()
    {
        itemType = ItemType.Crop; 
    }
}