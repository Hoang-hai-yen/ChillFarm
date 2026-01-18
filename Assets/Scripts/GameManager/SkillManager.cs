using UnityEngine;

public enum SkillType { Stamina, Fishing, Animal, Harvest }

public class SkillManager : MonoBehaviour, IDataPersistence
{
    public static SkillManager Instance;

    [Header("Skill Levels (0-5)")]
    public int staminaLevel = 0;
    public int fishingLevel = 0;
    public int animalLevel = 0;
    public int harvestLevel = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadData(GameData data)
    {
        staminaLevel = data.PlayerDataData.SkillLevel;
        fishingLevel = data.PlayerDataData.FishingLevel;
        animalLevel = data.PlayerDataData.AnimalLevel;
        harvestLevel = data.PlayerDataData.FarmingLevel;
    }

    public void SaveData(GameData data)
    {
        data.PlayerDataData.SkillLevel = staminaLevel;
        data.PlayerDataData.FishingLevel = fishingLevel;
        data.PlayerDataData.AnimalLevel = animalLevel;
        data.PlayerDataData.FarmingLevel = harvestLevel;
    }

    // 1. Giảm Stamina (Trả về % giảm: 0.1 = giảm 10%)
    public float GetStaminaReduction() => staminaLevel * 0.05f; // Max 25%

    // 2. Tỉ lệ cá hiếm (Trả về % cộng thêm)
    public float GetRareFishBonus() => fishingLevel * 0.05f; // Max 25%

    // 3. Sản phẩm động vật (Trả về % tỉ lệ cộng thêm)
    public float GetAnimalProductBonus() => animalLevel * 0.1f; // Max 50%

    // 4. Sản lượng thu hoạch (Trả về tỉ lệ x2 sản lượng)
    public float GetHarvestBonus() => harvestLevel * 0.2f; // Max 100% (luôn x2 ở lv 5)

    public int GetSkillLevel(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Stamina => staminaLevel,
            SkillType.Fishing => fishingLevel,
            SkillType.Animal => animalLevel,
            SkillType.Harvest => harvestLevel,
            _ => 0,
        };
    }
}