using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceData", menuName = "Inventory/Resource Data")]
public class ResourceData : FoodData
{

    protected override void OnValidate()
    {
        base.OnValidate();
        itemType = ItemType.Crop; 
    }
}