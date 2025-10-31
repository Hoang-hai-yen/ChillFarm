using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("UI Visuals")]
    [Tooltip("Gán 9 ô Image NỀN (để đổi màu)")]
    public Image[] slotBackgroundImages; //
    
    [Tooltip("Gán 9 ô Image ICON (để hiện item)")]
    public Image[] slotItemIcons;      
    
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateHotbarVisuals;
            UpdateHotbarVisuals(); 
        }
        else
        {
            Debug.LogError("HotbarManager: Không tìm thấy InventoryManager!");
        }
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateHotbarVisuals;
        }
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseScroll();
    }

    void HandleKeyboardInput()
    {
        for (int i = 0; i < slotBackgroundImages.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                InventoryManager.Instance.SelectSlot(i);
            }
        }
    }

    void HandleMouseScroll()
    {
        if (InventoryManager.Instance == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        int currentIndex = InventoryManager.Instance.SelectedHotbarSlot;

        if (scroll > 0f)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = slotBackgroundImages.Length - 1;
        }
        else if (scroll < 0f)
        {
            currentIndex++;
            if (currentIndex >= slotBackgroundImages.Length) currentIndex = 0;
        }
        
        InventoryManager.Instance.SelectSlot(currentIndex);
    }

    void UpdateHotbarVisuals()
    {
        int selectedIndex = InventoryManager.Instance.SelectedHotbarSlot;

        for (int i = 0; i < slotBackgroundImages.Length; i++)
        {
            slotBackgroundImages[i].color = (i == selectedIndex) ? selectedColor : normalColor;

            InventorySlot slot = InventoryManager.Instance.hotbarSlots[i];
            if (slot.itemData != null)
            {
                slotItemIcons[i].sprite = slot.itemData.itemIcon;
                slotItemIcons[i].color = Color.white;
            }
            else
            {
                slotItemIcons[i].sprite = null;
                slotItemIcons[i].color = new Color(1, 1, 1, 0); 
            }
        }
    }
}