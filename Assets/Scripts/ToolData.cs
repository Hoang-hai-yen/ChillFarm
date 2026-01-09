using UnityEngine;
public enum ToolType {Hand, Hoe, WateringCan, FishingRod }

[CreateAssetMenu(fileName = "New ToolData", menuName = "Farming/Tool Data")]
public class ToolData : ItemData
{
    public ToolType toolType;

    protected override void OnValidate()
    {
        base.OnValidate();
        itemType = ItemType.Tool;
    }
}