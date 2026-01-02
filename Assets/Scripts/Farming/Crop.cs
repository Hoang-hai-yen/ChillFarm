using UnityEngine;
using UnityEngine.UI;

public class Crop : MonoBehaviour
{
    [HideInInspector] public CropData cropData;

    private SpriteRenderer sr;
    private Canvas progressBarCanvas;
    private Slider growthSlider;

    private float currentGrowth = 0;
    private int currentStage = 0;
    private bool isWatered = false;
    private bool isHarvestable = false;
    private float yieldMultiplier = 1f; 
    private bool hasBeenFertilized = false; 

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        progressBarCanvas = GetComponentInChildren<Canvas>();
        growthSlider = GetComponentInChildren<Slider>();
        if (progressBarCanvas != null)
            progressBarCanvas.gameObject.SetActive(false);
    }

    public void Initialize(CropData data, bool isSoilWet) 
    {
        cropData = data;
        
        isWatered = isSoilWet; 
        hasBeenFertilized = false;
        yieldMultiplier = 1f;
        
        sr.sprite = cropData.growthSprites[0];
        UpdateGrowthUI();
    }

    public void LoadState(CropData data, float growth, bool watered)
    {
        cropData = data;
        currentGrowth = growth;
        isWatered = watered;

        CalculateStage();
        sr.sprite = cropData.growthSprites[currentStage];

        if (isHarvestable)
            progressBarCanvas.gameObject.SetActive(false);

        UpdateGrowthUI();
    }

    public void Water()
    {
        isWatered = true;
    }

    public void Grow()
    {
        if (isHarvestable) return;

        if (isWatered)
        {
            currentGrowth += 1; 
            isWatered = false; 

            CalculateStage();
            sr.sprite = cropData.growthSprites[currentStage];
            UpdateGrowthUI();
        }
    }

    private void CalculateStage()
    {
        if (currentGrowth >= cropData.daysToGrow)
        {
            isHarvestable = true;
            currentStage = cropData.growthSprites.Length - 1;
        }
        else
        {
            isHarvestable = false;
            float progress = currentGrowth / cropData.daysToGrow;
            currentStage = Mathf.FloorToInt(progress * (cropData.growthSprites.Length - 1));
        }
    }

    // public void Harvest()
    // {
    //     if(isHarvestable) 
    //     {
    //         Debug.Log($"Thu hoạch {cropData.cropName}!");
            
    //         if(cropData.harvestItemPrefab != null)
    //         {
    //             int finalYield = Mathf.RoundToInt(cropData.harvestYield * yieldMultiplier);

    //             if (finalYield < 1) finalYield = 1;

    //             Debug.Log($"Thu hoạch được: {finalYield} (Gốc: {cropData.harvestYield} x Hệ số: {yieldMultiplier:F2})");

    //             for(int i = 0; i < finalYield; i++) 
    //             {
    //                 Vector3 spawnOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
    //                 Instantiate(cropData.harvestItemPrefab, transform.position + spawnOffset, Quaternion.identity);
    //             }
    //         }

    //         Destroy(gameObject); 
    //     }
    // }

    public void Harvest()
{
    if (isHarvestable) 
    {
        Debug.Log($"Thu hoạch {cropData.cropName}!");
        
        if (cropData.harvestItemPrefab != null)
        {
            // 1. Tính toán sản lượng gốc dựa trên dữ liệu và hệ số vùng (yieldMultiplier)
            int baseYield = cropData.harvestYield;
            
            // 2. Tính toán Bonus từ kỹ năng
            int bonusItems = 0;
            if (SkillManager.Instance != null)
            {
                float harvestBonusChance = SkillManager.Instance.GetHarvestBonus();
                
                // Sử dụng tỷ lệ ngẫu nhiên để quyết định có cộng thêm sản phẩm hay không
                // Ví dụ: Level 5 có 100% cơ hội (1.0), chắc chắn cộng thêm sản phẩm
                if (Random.value < harvestBonusChance)
                {
                    bonusItems = 1; 
                    Debug.Log("<color=cyan>Kỹ năng Thu Hoạch kích hoạt! Nhận thêm sản phẩm.</color>");
                }
            }

            // 3. Tổng hợp sản lượng cuối cùng
            int finalYield = Mathf.RoundToInt((baseYield + bonusItems) * yieldMultiplier);

            // Đảm bảo luôn có ít nhất 1 sản phẩm
            if (finalYield < 1) finalYield = 1;

            Debug.Log($"Tổng thu hoạch: {finalYield} (Gốc: {baseYield} + Bonus: {bonusItems}) x Hệ số: {yieldMultiplier:F2}");

            // 4. Sinh ra (Spawn) các item vật phẩm
            for (int i = 0; i < finalYield; i++) 
            {
                Vector3 spawnOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                Instantiate(cropData.harvestItemPrefab, transform.position + spawnOffset, Quaternion.identity);
            }
        }

        Destroy(gameObject); 
    }
}

    private void UpdateGrowthUI()
    {
        if(growthSlider != null)
            growthSlider.value = currentGrowth / cropData.daysToGrow;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isHarvestable && progressBarCanvas != null)
        {
            progressBarCanvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && progressBarCanvas != null)
        {
            progressBarCanvas.gameObject.SetActive(false);
        }
    }

    public float GetCurrentGrowth() => currentGrowth;
    public bool IsWatered() => isWatered;
    public bool IsHarvestable() => isHarvestable;

    public bool ApplyFertilizer(float minMult, float maxMult)
{
    if (hasBeenFertilized) 
    {
        Debug.Log("Cây này đã được bón phân rồi!");
        return false; 
    }

    hasBeenFertilized = true;
    
    yieldMultiplier = Random.Range(minMult, maxMult);
    
    Debug.Log($"Đã bón phân! Sản lượng sẽ nhân lên: {yieldMultiplier:F2} lần");
    return true; 
}
}