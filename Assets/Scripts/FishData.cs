using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Fishing/Fish Data")]
public class FishData : ItemData
{
    
    [Header("Độ hiếm")]
    [Tooltip("Giá trị càng cao thì càng dễ câu trúng. Ví dụ: Cá Phổ biến = 100, Cá Hiếm = 10")]
    [Min(1)] public int rarityWeight = 50;
    private void OnValidate()
    {
        itemType = ItemType.Fish;
        staminaCost = 0;
    }
}