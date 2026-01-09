using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class Upgrade : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SkillType skillType;
    [SerializeField] private GameObject[] diamonds;
    [SerializeField] private int[] levelCosts = {100, 200, 400, 800, 1600};
    public TMP_Text costText;

    private int currentLevel = 0; 

    void Start()
    {
        UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuyUpgrade();
    }

   private void BuyUpgrade()
{
    // 1. Kiểm tra Singleton
    if (InventoryManager.Instance == null) {
        Debug.LogError("Chưa có InventoryManager trong Scene!");
        return;
    }

    if (currentLevel >= 5) return;

    // 2. Kiểm tra mảng levelCosts
    if (levelCosts == null || levelCosts.Length <= currentLevel) {
        Debug.LogError("Mảng levelCosts chưa được thiết lập đủ 5 phần tử!");
        return;
    }

    int cost = levelCosts[currentLevel];

    if (InventoryManager.Instance.TrySpendGold(cost))
    {
        currentLevel++;
        UpdateSkillLevel();
        UpdateUI();
    }
    else
    {
        Debug.Log("Không đủ Gold!");
    }
}

    private void UpdateSkillLevel()
    {
        switch (skillType)
        {
            case SkillType.Stamina: SkillManager.Instance.staminaLevel = currentLevel; break;
            case SkillType.Fishing: SkillManager.Instance.fishingLevel = currentLevel; break;
            case SkillType.Animal: SkillManager.Instance.animalLevel = currentLevel; break;
            case SkillType.Harvest: SkillManager.Instance.harvestLevel = currentLevel; break;
        }
    }

    private void UpdateUI()
{
    if (diamonds == null) return;

    costText.text = levelCosts[currentLevel].ToString();
    for (int i = 0; i < diamonds.Length; i++)
    {
        if (diamonds[i] != null)
        {
            diamonds[i].SetActive(i < currentLevel);
        }
        else {
            Debug.LogWarning($"Phần tử thứ {i} trong mảng diamonds bị thiếu (Null)!");
        }
    }
}
}