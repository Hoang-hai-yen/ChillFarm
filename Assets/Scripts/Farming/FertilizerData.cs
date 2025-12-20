using UnityEngine;

[CreateAssetMenu(fileName = "New Fertilizer", menuName = "Farming/Fertilizer Data")]
public class FertilizerData : ItemData
{
    [Header("Fertilizer Info")]
    [Tooltip("Hệ số nhân tối thiểu (VD: 1.0)")]
    public float minMultiplier = 1f;
    
    [Tooltip("Hệ số nhân tối đa (VD: 2.0)")]
    public float maxMultiplier = 2f;

    private void OnValidate()
    {
        itemType = ItemType.Fertilizer;
    }
}