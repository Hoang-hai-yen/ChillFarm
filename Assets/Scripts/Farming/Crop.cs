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

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        progressBarCanvas = GetComponentInChildren<Canvas>();
        growthSlider = GetComponentInChildren<Slider>();
        if (progressBarCanvas != null)
            progressBarCanvas.gameObject.SetActive(false);
    }

    public void Initialize(CropData data)
    {
        cropData = data;
        isWatered = true;
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

    public void Harvest()
    {
        if(isHarvestable) 
        {
            Debug.Log($"Thu hoạch {cropData.cropName}!");
            
            if(cropData.harvestItemPrefab != null)
            {
                for(int i = 0; i < cropData.harvestYield; i++) 
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
}