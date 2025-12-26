using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IDropHandler
{
    public int slotIndex; 
    public bool isHotbarSlot; 

    public void OnDrop(PointerEventData eventData)
    {
        if (isHotbarSlot && slotIndex == 0) 
        {
            Debug.Log("Không thể bỏ đồ vào ô Bàn tay!");
            return;
        }

        GameObject droppedObj = eventData.pointerDrag;
        InventoryDragItem draggableItem = droppedObj.GetComponent<InventoryDragItem>();

        if (draggableItem != null)
        {
            InventorySlotUI oldSlotUI = draggableItem.parentAfterDrag.GetComponent<InventorySlotUI>();

            if (oldSlotUI != null)
            {
                if (oldSlotUI.isHotbarSlot && oldSlotUI.slotIndex == 0) return;

                InventoryManager.Instance.SwapItems(
                    oldSlotUI.slotIndex, oldSlotUI.isHotbarSlot, 
                    this.slotIndex, this.isHotbarSlot           
                );
            }
        }
    }
}