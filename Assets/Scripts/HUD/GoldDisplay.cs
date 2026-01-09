using UnityEngine;
using TMPro; 

public class GoldDisplay : MonoBehaviour
{
    [Tooltip("Kéo cái TextMeshPro dùng để hiện tiền vào đây")]
    public TMP_Text goldText;

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            UpdateGoldUI(InventoryManager.Instance.currentGold);

            InventoryManager.Instance.OnGoldChanged += UpdateGoldUI;
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnGoldChanged -= UpdateGoldUI;
        }
    }

    private void UpdateGoldUI(int currentGold)
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString() + " G"; 
        }
    }
}