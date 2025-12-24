using UnityEngine;

[CreateAssetMenu(fileName = "New Livestock", menuName = "Inventory/Livestock Item")]
public class LivestockItemData : ItemData
{
    [Header("Livestock Info")]
    public GameObject animalPrefab;
    public AnimalType animalType;

    private void OnValidate()
    {
        itemType = ItemType.Livestock;
    }
}