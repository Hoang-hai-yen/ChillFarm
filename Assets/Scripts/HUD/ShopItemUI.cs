using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private ItemData currentItem;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClick);
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnGoldChanged += UpdateButtonState;
        }
    }

    void OnDestroy() 
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnGoldChanged -= UpdateButtonState;
        }
    }

    public void SetItemData(ItemData item)
    {
        currentItem = item;
        if (currentItem != null)
        {
            iconImage.sprite = currentItem.itemIcon;
            iconImage.gameObject.SetActive(true);
            nameText.text = currentItem.itemName;
            priceText.text = $"{currentItem.price} G";

            UpdateButtonState(InventoryManager.Instance.currentGold);
        }
    }

    private void UpdateButtonState(int currentGold)
    {
        if (currentItem == null) return;
        buyButton.interactable = (currentGold >= currentItem.price);
    }

    private void OnBuyClick()
    {
        if (currentItem == null) return;
        
        InventoryManager.Instance.BuyItem(currentItem, currentItem.price);
    }
}