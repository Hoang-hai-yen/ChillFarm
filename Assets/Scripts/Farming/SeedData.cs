using UnityEngine;

[CreateAssetMenu(fileName = "New SeedData", menuName = "Farming/Seed Data")]
public class SeedData : ItemData
{
    public CropData cropToPlant; 

    protected override void OnValidate()
    {
        base.OnValidate();
        itemType = ItemType.Seed;
    }
}