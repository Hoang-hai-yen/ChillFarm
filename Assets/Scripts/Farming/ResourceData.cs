using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceData", menuName = "Inventory/Resource Data")]
public class ResourceData : ItemData
{

    private void OnValidate()
    {
        itemType = ItemType.Crop; 
    }
}