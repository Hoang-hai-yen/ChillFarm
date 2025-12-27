using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private ItemData currentItem;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClick);
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
        }
    }

    private void OnBuyClick()
    {
        if (currentItem == null) return;

        if (InventoryManager.Instance.TrySpendGold(currentItem.price))
        {
            bool added = InventoryManager.Instance.AddItem(currentItem, 1);
            
            if (added)
            {
                Debug.Log($"Đã mua {currentItem.itemName}");
            }
            else
            {
                InventoryManager.Instance.AddGold(currentItem.price);
                Debug.Log("Túi đồ đã đầy!");
            }
        }
    }
}