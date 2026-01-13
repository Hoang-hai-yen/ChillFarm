using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HotbarManager : MonoBehaviour
{
    [Header("UI Visuals")]
    public Image[] slotBackgroundImages;
    public Image[] slotItemIcons;
    
    [Header("Quantity UI")]
    public TextMeshProUGUI[] slotQuantityTexts; 

    [Header("Colors & Sprites")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Sprite handIconSprite;

    void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("HotbarManager: Không tìm thấy InventoryManager!");
            return;
        }

        InventoryManager.Instance.OnInventoryChanged += UpdateHotbarVisuals;
        
        // Chọn mặc định ô đầu tiên
        InventoryManager.Instance.SelectSlot(0);
        UpdateHotbarVisuals();
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
        if (InventoryManager.Instance == null) return;
        
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
        int newIndex = currentIndex;

        if (scroll > 0f)
        {
            newIndex--;
            if (newIndex < 0) newIndex = slotBackgroundImages.Length - 1;
        }
        else if (scroll < 0f)
        {
            newIndex++;
            if (newIndex >= slotBackgroundImages.Length) newIndex = 0;
        }

        if (newIndex != currentIndex)
        {
            InventoryManager.Instance.SelectSlot(newIndex);
        }
    }

    void UpdateHotbarVisuals()
    {
        if (InventoryManager.Instance == null) return;
        
        int selectedIndexFromInventory = InventoryManager.Instance.SelectedHotbarSlot; 

        for (int i = 0; i < slotBackgroundImages.Length; i++)
        {
            if (slotBackgroundImages[i] != null)
            {
                slotBackgroundImages[i].color = (i == selectedIndexFromInventory) ? selectedColor : normalColor;
            }

            InventorySlot slot = InventoryManager.Instance.hotbarSlots[i];

            if (slotItemIcons[i] != null)
            {
                if (i == 0) 
                {
                    slotItemIcons[i].sprite = handIconSprite;
                    slotItemIcons[i].color = Color.white;
                }
                else if (slot.itemData != null)
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

            if (slotQuantityTexts != null && i < slotQuantityTexts.Length && slotQuantityTexts[i] != null)
            {
                if (i == 0 || slot.itemData == null || slot.quantity <= 1)
                {
                    slotQuantityTexts[i].gameObject.SetActive(false);
                }
                else
                {
                    slotQuantityTexts[i].text = slot.quantity.ToString();
                    slotQuantityTexts[i].gameObject.SetActive(true);
                }
            }
        }
    }
}