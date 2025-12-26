using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    [Header("UI Visuals")]
    [Tooltip("Kéo thả các Image nền (Background) của Slot vào đây")]
    public Image[] slotBackgroundImages; 

    [Tooltip("Kéo thả các Image hiển thị vật phẩm (Icon) vào đây")]
    public Image[] slotItemIcons;

    [Header("Settings")]
    public Color normalColor = Color.white; // Màu nền mặc định

    [Header("Setup Helper")]
    [Tooltip("Kéo object BackpackPanel vào đây để code tự đánh số thứ tự cho các Slot")]
    public Transform backpackPanel; 

    void Start()
    {
        // 1. Tự động điền số thứ tự (Index) cho các ô Slot cha
        SetupSlotIndices();

        // 2. Đăng ký sự kiện
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateBackpackVisuals;
            
            // Cập nhật ngay lần đầu
            UpdateBackpackVisuals();
        }
        else
        {
            Debug.LogError("BackpackManager: Không tìm thấy InventoryManager!");
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateBackpackVisuals;
        }
    }

    void SetupSlotIndices()
    {
        if (backpackPanel == null) return;

        int index = 0;
        foreach (Transform slotTransform in backpackPanel)
        {
            InventorySlotUI slotUI = slotTransform.GetComponent<InventorySlotUI>();
            if (slotUI == null) slotUI = slotTransform.gameObject.AddComponent<InventorySlotUI>();

            // Cài đặt thông số
            slotUI.slotIndex = index;
            slotUI.isHotbarSlot = false; // Đây là Backpack

            index++;
            if (InventoryManager.Instance != null && index >= InventoryManager.Instance.backpackSlots.Length) 
                break;
        }
    }

// Trong file BackpackManager.cs
    public void UpdateBackpackVisuals()
    {
        if (InventoryManager.Instance == null) return;

        InventorySlot[] backpackData = InventoryManager.Instance.backpackSlots;

        for (int i = 0; i < slotItemIcons.Length; i++)
        {
            if (i >= backpackData.Length) break;

            InventorySlot slotData = backpackData[i];
            
            // 1. XỬ LÝ BACKGROUND
            if (i < slotBackgroundImages.Length && slotBackgroundImages[i] != null)
            {
                slotBackgroundImages[i].color = normalColor;
            }

            // 2. XỬ LÝ ICON
            if (slotItemIcons[i] != null)
            {
                if (slotData.itemData != null)
                {
                    slotItemIcons[i].sprite = slotData.itemData.itemIcon;
                    slotItemIcons[i].color = Color.white;
                    
                    // --- CHỈ SỬA NHỎ Ở ĐÂY ---
                    // Nếu Image đang bị tắt raycast (do script Drag đang chạy), thì đừng bật lại vội
                    // Chỉ bật nếu nó đang không kéo (InventoryDragItem tự quản lý lúc kéo)
                    // Tuy nhiên, cách đơn giản nhất là cứ để TRUE như bạn làm,
                    // vì OnEndDrag sẽ chạy sau và sửa lại mọi thứ.
                    slotItemIcons[i].raycastTarget = true; 
                    
                    slotItemIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    slotItemIcons[i].sprite = null;
                    slotItemIcons[i].color = new Color(1, 1, 1, 0); 
                    // Khi ẩn đi thì tắt raycast để không bấm nhầm vào ô trống
                    slotItemIcons[i].raycastTarget = false; 
                }
            }
        }
    }
}