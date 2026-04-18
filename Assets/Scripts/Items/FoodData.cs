using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Inventory/Food Item")]
public class FoodData : ItemData
{
    [Header("Food Settings")]
    [Tooltip("Lượng Stamina hồi phục khi ăn")]
    public float staminaRecover = 20f;

    [Tooltip("Âm thanh khi ăn (tiếng nhai, uống...)")]
    public AudioClip eatSound;
}