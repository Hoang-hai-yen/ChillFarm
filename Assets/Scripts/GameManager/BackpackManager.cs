using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class BackpackManager : MonoBehaviour
{
    [Header("UI Visuals")]
    [Tooltip("Kéo thả các Image nền (Background) của Slot vào đây")]
    public Image[] slotBackgroundImages; 

    [Tooltip("Kéo thả các Image hiển thị vật phẩm (Icon) vào đây")]
    public Image[] slotItemIcons;

    [Header("Quantity UI")]
    [Tooltip("Kéo thả các TextMeshPro hiển thị số lượng vào đây (theo thứ tự slot)")]
    public TextMeshProUGUI[] slotQuantityTexts;

    [Header("Settings")]
    public Color normalColor = Color.white; 

    [Header("Setup Helper")]
    [Tooltip("Kéo object BackpackPanel vào đây để code tự đánh số thứ tự cho các Slot")]
    public Transform backpackPanel; 

    void Start()
    {
        SetupSlotIndices();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateBackpackVisuals;
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

            slotUI.slotIndex = index;
            slotUI.isHotbarSlot = false; 

            index++;
            if (InventoryManager.Instance != null && index >= InventoryManager.Instance.backpackSlots.Length) 
                break;
        }
    }

    public void UpdateBackpackVisuals()
    {
        if (InventoryManager.Instance == null) return;

        InventorySlot[] backpackData = InventoryManager.Instance.backpackSlots;

        for (int i = 0; i < slotItemIcons.Length; i++)
        {
            if (i >= backpackData.Length) break;

            InventorySlot slotData = backpackData[i];
            
            if (i < slotBackgroundImages.Length && slotBackgroundImages[i] != null)
            {
                slotBackgroundImages[i].color = normalColor;
            }

            if (slotItemIcons[i] != null)
            {
                if (slotData.itemData != null)
                {
                    slotItemIcons[i].sprite = slotData.itemData.itemIcon;
                    slotItemIcons[i].color = Color.white;
                    slotItemIcons[i].raycastTarget = true; 
                    slotItemIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    slotItemIcons[i].sprite = null;
                    slotItemIcons[i].color = new Color(1, 1, 1, 0); 
                    slotItemIcons[i].raycastTarget = false; 
                }
            }

            if (slotQuantityTexts != null && i < slotQuantityTexts.Length && slotQuantityTexts[i] != null)
            {
                if (slotData.itemData != null && slotData.quantity > 1)
                {
                    slotQuantityTexts[i].text = slotData.quantity.ToString();
                    slotQuantityTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    slotQuantityTexts[i].gameObject.SetActive(false);
                }
            }
        }
    }
}