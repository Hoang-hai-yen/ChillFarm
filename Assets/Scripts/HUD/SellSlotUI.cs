using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public TMP_Text quantityText;
    public TMP_Text sellPriceText;
    public Button sellButton;

    private ItemData currentItem;

    void Start()
    {
        if (sellButton == null) sellButton = GetComponent<Button>();
        sellButton.onClick.AddListener(OnSellClick);
    }

    public void SetSlotData(InventorySlot slot)
    {
        if (slot != null && slot.itemData != null)
        {
            currentItem = slot.itemData;
            
            iconImage.sprite = currentItem.itemIcon;
            iconImage.gameObject.SetActive(true);
            
            quantityText.text = "x" + slot.quantity.ToString();
            
            sellPriceText.text = "+" + currentItem.sellPrice.ToString() + " G";
            
            sellButton.interactable = true;
        }
        else
        {
            ClearSlot();
        }
    }

    void ClearSlot()
    {
        currentItem = null;
        iconImage.gameObject.SetActive(false);
        quantityText.text = "";
        sellPriceText.text = "";
        sellButton.interactable = false;
    }

    void OnSellClick()
    {
        if (currentItem != null)
        {
            InventoryManager.Instance.SellItem(currentItem, 1);
            
        }
    }
}