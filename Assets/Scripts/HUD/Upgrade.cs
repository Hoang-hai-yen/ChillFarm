using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class Upgrade : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SkillType skillType;
    [SerializeField] private GameObject[] diamonds;
    [SerializeField] private int[] levelCosts = {100, 200, 400, 800, 1600};
    public TMP_Text costText;

    // private int currentLevel = 0; 

    void Start()
    {
        UpdateUI();
    }

    void OnEnable()
    {
        GameDataManager.instance.OnDataLoaded += UpdateUI;
    }

    void OnDisable()
    {
        GameDataManager.instance.OnDataLoaded -= UpdateUI;
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

    if (SkillManager.Instance.GetSkillLevel(skillType) >= 5) return;

    // 2. Kiểm tra mảng levelCosts
    if (levelCosts == null || levelCosts.Length <= SkillManager.Instance.GetSkillLevel(skillType)) {
        Debug.LogError("Mảng levelCosts chưa được thiết lập đủ 5 phần tử!");
        return;
    }

    int cost = levelCosts[SkillManager.Instance.GetSkillLevel(skillType)];

    if (InventoryManager.Instance.TrySpendGold(cost))
    {
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
            case SkillType.Stamina: SkillManager.Instance.staminaLevel++; break;
            case SkillType.Fishing: SkillManager.Instance.fishingLevel++; break;
            case SkillType.Animal: SkillManager.Instance.animalLevel++; break;
            case SkillType.Harvest: SkillManager.Instance.harvestLevel++; break;
        }
    }

    private void UpdateUI()
{
    if (diamonds == null) return;

    costText.text = levelCosts[SkillManager.Instance.GetSkillLevel(skillType)].ToString();
    for (int i = 0; i < diamonds.Length; i++)
    {
        if (diamonds[i] != null)
        {
            diamonds[i].SetActive(i < SkillManager.Instance.GetSkillLevel(skillType));
        }
        else {
            Debug.LogWarning($"Phần tử thứ {i} trong mảng diamonds bị thiếu (Null)!");
        }
    }
}
}