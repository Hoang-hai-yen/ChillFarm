using UnityEngine;

[CreateAssetMenu(fileName = "New CropData", menuName = "Farming/Crop Data")]
public class CropData : GameSOData
{
    public string cropName;
    [Tooltip("Các hình ảnh từ mầm -> lớn. Hình cuối cùng là hình thu hoạch.")]
    public Sprite[] growthSprites;
    [Tooltip("Tổng số ngày để lớn (không tính ngày gieo hạt).")]
    public float daysToGrow;
    [Tooltip("Vật phẩm Prefab sẽ rớt ra khi thu hoạch.")]
    public GameObject harvestItemPrefab;
    [Tooltip("Số lượng vật phẩm rớt ra.")]
    public int harvestYield = 1;
}