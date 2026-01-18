using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Mushroom/Mushroom Data")]
public class MushroomData : FoodData
{
    public Sprite mushroomSprite;
    
    protected override void OnValidate()
    {
        base.OnValidate();
        itemType = ItemType.Mushroom;
    }
}