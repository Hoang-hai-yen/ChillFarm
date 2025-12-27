using UnityEngine;
public enum ToolType {Hand, Hoe, WateringCan, FishingRod }

[CreateAssetMenu(fileName = "New ToolData", menuName = "Farming/Tool Data")]
public class ToolData : ItemData
{
    public ToolType toolType;

    private void OnValidate()
    {
        itemType = ItemType.Tool;
    }
}