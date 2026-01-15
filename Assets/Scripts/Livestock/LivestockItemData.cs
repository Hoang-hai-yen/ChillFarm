using UnityEngine;

[CreateAssetMenu(fileName = "New Livestock", menuName = "Inventory/Livestock Item")]
public class LivestockItemData : ItemData
{
    [Header("Livestock Info")]
    public GameObject animalPrefab;
    public AnimalType animalType;

    protected override void OnValidate()
    {
        base.OnValidate();
        itemType = ItemType.Livestock;
    }
}