using UnityEngine;

[CreateAssetMenu(fileName = "New SeedData", menuName = "Farming/Seed Data")]
public class SeedData : ItemData
{
    public CropData cropToPlant; 

    private void OnValidate()
    {
        itemType = ItemType.Seed;
    }
}