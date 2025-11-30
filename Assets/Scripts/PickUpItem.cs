using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Tooltip("Data ScriptableObject của vật phẩm này (ví dụ: Carrot Item Data).")]
    public ItemData itemData;

    [Tooltip("Số lượng vật phẩm sẽ được nhặt (thường là 1).")]
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePickup();
        }
    }

    private void HandlePickup()
    {
        if (itemData == null)
        {
            Debug.LogError("PickupItem: ItemData chưa được gán! Không thể nhặt.");
            return;
        }

        if (InventoryManager.Instance != null)
        {
            bool wasPickedUp = InventoryManager.Instance.AddItem(itemData, quantity);

            if (wasPickedUp)
            {
                Debug.Log($"Player nhặt {quantity} x {itemData.itemName}.");
                
                Destroy(gameObject); 
            }
            else
            {
                Debug.Log("Inventory đầy, không thể nhặt vật phẩm.");
            }
        }
        else
        {
            Debug.LogError("PickupItem: InventoryManager không tìm thấy!");
        }
    }
}