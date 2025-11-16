using UnityEngine;
public enum ToolType { Hoe, WateringCan }

[CreateAssetMenu(fileName = "New ToolData", menuName = "Farming/Tool Data")]
public class ToolData : ItemData
{
    public ToolType toolType;

    private void OnValidate()
    {
        itemType = ItemType.Tool;
    }
}