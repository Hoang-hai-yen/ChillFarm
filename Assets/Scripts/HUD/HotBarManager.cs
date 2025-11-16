//using UnityEngine;
//using UnityEngine.UI;

//public class HotbarManager : MonoBehaviour
//{
//    [Header("UI Visuals")]
//    [Tooltip("Gán 9 ô Image NỀN (để đổi màu)")]
//    public Image[] slotBackgroundImages; //

//    [Tooltip("Gán 9 ô Image ICON (để hiện item)")]
//    public Image[] slotItemIcons;

//    public Color normalColor = Color.white;
//    public Color selectedColor = Color.yellow;

//    void Start()
//    {
//        if (InventoryManager.Instance != null)
//        {
//            InventoryManager.Instance.OnInventoryChanged += UpdateHotbarVisuals;
//            UpdateHotbarVisuals();
//        }
//        else
//        {
//            Debug.LogError("HotbarManager: Không tìm thấy InventoryManager!");
//        }
//    }

//    void OnDestroy()
//    {
//        if (InventoryManager.Instance != null)
//        {
//            InventoryManager.Instance.OnInventoryChanged -= UpdateHotbarVisuals;
//        }
//    }

//    void Update()
//    {
//        HandleKeyboardInput();
//        HandleMouseScroll();
//    }

//    void HandleKeyboardInput()
//    {
//        for (int i = 0; i < slotBackgroundImages.Length; i++)
//        {
//            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
//            {
//                InventoryManager.Instance.SelectSlot(i);
//            }
//        }
//    }

//    void HandleMouseScroll()
//    {
//        if (InventoryManager.Instance == null) return;

//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        int currentIndex = InventoryManager.Instance.SelectedHotbarSlot;

//        if (scroll > 0f)
//        {
//            currentIndex--;
//            if (currentIndex < 0) currentIndex = slotBackgroundImages.Length - 1;
//        }
//        else if (scroll < 0f)
//        {
//            currentIndex++;
//            if (currentIndex >= slotBackgroundImages.Length) currentIndex = 0;
//        }

//        InventoryManager.Instance.SelectSlot(currentIndex);
//    }

//    void UpdateHotbarVisuals()
//    {
//        int selectedIndex = InventoryManager.Instance.SelectedHotbarSlot;

//        for (int i = 0; i < slotBackgroundImages.Length; i++)
//        {
//            slotBackgroundImages[i].color = (i == selectedIndex) ? selectedColor : normalColor;

//            InventorySlot slot = InventoryManager.Instance.hotbarSlots[i];
//            if (slot.itemData != null)
//            {
//                slotItemIcons[i].sprite = slot.itemData.itemIcon;
//                slotItemIcons[i].color = Color.white;
//            }
//            else
//            {
//                slotItemIcons[i].sprite = null;
//                slotItemIcons[i].color = new Color(1, 1, 1, 0);
//            }
//        }
//    }
//}

using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("UI Visuals")]
    public Image[] slotBackgroundImages;
    public Image[] slotItemIcons;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;

    void Start()
    {
        InventoryManager.Instance.OnInventoryChanged += UpdateHotbarVisuals;
        InventoryManager.Instance.SelectSlot(selectedIndex);
        UpdateHotbarVisuals();
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
                SelectSlot(i);
            }
        }
    }

    void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = slotBackgroundImages.Length - 1;
        }
        else if (scroll < 0f)
        {
            selectedIndex++;
            if (selectedIndex >= slotBackgroundImages.Length) selectedIndex = 0;
        }

        InventoryManager.Instance.ForceSetSlot(selectedIndex);
    }

    void SelectSlot(int index)
    {
        selectedIndex = index;

        // Chỉ chọn đúng index, KHÔNG kiểm tra item
        InventoryManager.Instance.ForceSetSlot(selectedIndex);
        UpdateHotbarVisuals();
    }

    void UpdateHotbarVisuals()
    {
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

