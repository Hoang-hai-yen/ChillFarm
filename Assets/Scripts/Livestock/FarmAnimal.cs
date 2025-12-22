using UnityEngine;
using System.Collections;

public class FarmAnimal : MonoBehaviour
{
    [Header("Data")]
    public AnimalData data;

    [Header("Status (Read Only)")]
    [SerializeField] private bool isAdult = false;
    [SerializeField] private int currentAge = 0;
    [SerializeField] private float affection = 0f; 
    [SerializeField] private bool isFedToday = false;
    [SerializeField] private bool hasPlayedToday = false;
    [SerializeField] private bool isSick = false; 

    [Header("Components")]
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private GameObject heartParticlePrefab; 
    [SerializeField] private GameObject sadIconPrefab; 

    private TimeController timeController;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        timeController = FindFirstObjectByType<TimeController>();
        if (timeController != null) timeController.OnNewDayStart += OnNewDay;

        UpdateVisuals();
    }

    private void OnDisable()
    {
        if (timeController != null) timeController.OnNewDayStart -= OnNewDay;
    }


    public bool Feed()
    {
        if (isFedToday) return false;
        
        isFedToday = true;
        ShowEmote(heartParticlePrefab);
        Debug.Log($"{data.animalName} đã ăn ngon miệng!");
        return true;
    }

    public void Play()
    {
        if (hasPlayedToday) return;

        hasPlayedToday = true;
        affection = Mathf.Clamp(affection + 10f, 0, 100);
        
        if(anim != null) anim.SetTrigger("Happy");
        ShowEmote(heartParticlePrefab);
        
        Debug.Log($"Đã chơi với {data.animalName}. Tình cảm: {affection}");
    }


    private void OnNewDay()
    {
        if (!isFedToday)
        {
            Die();
            return; 
        }

        if (!hasPlayedToday)
        {
            affection = Mathf.Clamp(affection - 5f, 0, 100);
        }

        if (!isAdult)
        {
            currentAge++;
            if (currentAge >= data.daysToGrow)
            {
                isAdult = true;
                Debug.Log($"{data.animalName} đã trưởng thành!");
            }
        }

        if (isAdult && affection >= data.minAffectionToProduce)
        {
            ProduceProduct();
        }

        UpdateStateAnimation();

        isFedToday = false;
        hasPlayedToday = false;
        
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Đổi sprite lớn/nhỏ
        sr.sprite = isAdult ? data.adultSprite : data.babySprite;
    }

    private void UpdateStateAnimation()
    {
        bool isSad = affection < 10; 
        
        if (anim != null)
        {
            anim.SetBool("IsAdult", isAdult);
            anim.SetBool("IsSad", isSad); 
        }

        if (isSad && sadIconPrefab != null)
        {
        }
    }

    private void ProduceProduct()   
    {
        if (data.productPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(data.productPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"{data.animalName} đã đẻ trứng/cho sữa!");
        }
    }

    private void Die()
    {
        Debug.Log($"{data.animalName} đã chết vì đói...");
        Destroy(gameObject);
    }

    private void ShowEmote(GameObject prefab)
    {
        if (prefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    public AnimalType GetAnimalType() => data.type;
    public AnimalTier GetTier() => data.tier;
    public bool IsAdult() => isAdult;
}