using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("Hotbar Settings")]
    public Image[] slotImages; // Gán tất cả các slot ở đây (9 ô)
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateSlotVisuals();
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleMouseScroll();
    }

    void HandleKeyboardInput()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            // Nhấn phím số 1–9 để chọn ô tương ứng
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
            if (selectedIndex < 0) selectedIndex = slotImages.Length - 1;
            UpdateSlotVisuals();
        }
        else if (scroll < 0f)
        {
            selectedIndex++;
            if (selectedIndex >= slotImages.Length) selectedIndex = 0;
            UpdateSlotVisuals();
        }
    }

    void SelectSlot(int index)
    {
        selectedIndex = index;
        UpdateSlotVisuals();
    }

    void UpdateSlotVisuals()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
}
