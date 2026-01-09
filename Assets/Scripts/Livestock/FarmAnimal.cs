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
    
    [SerializeField] private int daysWithoutFood = 0;
    [SerializeField] private int maxStarvationDays = 3;
    private Vector3 targetPosition;
    private bool isDead = false; 

    [Header("Movement AI")]
    private Collider2D currentPen;
    private float moveSpeed = 0.5f;
    private bool isMoving = false;

    private SpriteRenderer sr;
    private Animator anim;
    private TimeController timeController;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        timeController = FindFirstObjectByType<TimeController>();
        if (timeController != null) timeController.OnNewDayStart += OnNewDay;

        UpdateVisuals();
    }

    void OnDisable()
    {
        if (timeController != null) timeController.OnNewDayStart -= OnNewDay;
    }


    public bool Feed()
    {
        if (isDead) return false;
        if (isFedToday) return false;

        isFedToday = true;
        daysWithoutFood = 0; 
        
        TriggerHappy();
        UpdateStateAnimation(); 
        
        Debug.Log($"{data.animalName} đã ăn và hết buồn!");
        return true;
    }

    public void Play()
    {
        if (isDead) return;
        if (hasPlayedToday) return;

        hasPlayedToday = true;
        affection = Mathf.Clamp(affection + 10f, 0, 100);
        
        TriggerHappy();
    }

    public void CleanupCorpse()
    {
        if (isDead)
        {
            Debug.Log("Đã dọn dẹp xác vật nuôi.");
            Destroy(gameObject);
        }
    }

    private void TriggerHappy()
    {
        if (anim != null) anim.SetTrigger("Happy");
    }


    private void OnNewDay()
    {
        if (isDead) return; 

        if (!isFedToday)
        {
            daysWithoutFood++;
            if (daysWithoutFood >= maxStarvationDays)
            {
                Die(); 
                return;
            }
        }
        else
        {
            daysWithoutFood = 0;
        }

        if (!hasPlayedToday) affection = Mathf.Clamp(affection - 5f, 0, 100);

        if (!isAdult && daysWithoutFood == 0)
        {
            currentAge++;
            if (currentAge >= data.daysToGrow) isAdult = true;
        }

        if (isAdult && affection >= data.minAffectionToProduce && daysWithoutFood == 0)
        {
            ProduceProduct();
        }

        isFedToday = false;
        hasPlayedToday = false;

        UpdateVisuals(); 
    }

    private void Die()
    {
        isDead = true;
        isMoving = false;
        
        if (anim != null)
        {
            anim.SetBool("IsDead", true);
            anim.SetBool("isMoving", false);
        }
        
        Debug.Log($"{data.animalName} đã chết. Cần dọn dẹp.");
    }

    private void UpdateStateAnimation()
    {
        if (isDead) return;

        bool isSad = !isFedToday; 

        if (anim != null)
        {
            anim.SetBool("IsAdult", isAdult);
            anim.SetBool("IsSad", isSad);
        }
    }

    private void UpdateVisuals()
    {
        UpdateStateAnimation();
    }


    public void SetHome(Collider2D penCollider)
    {
        currentPen = penCollider;
        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)  
        {

            if (isDead || (!isFedToday))
            {
                isMoving = false;
                if (anim != null) anim.SetBool("isMoving", false);
                
                yield return new WaitForSeconds(1f); 
                continue; 
            }

            
            isMoving = false;
            if (anim != null) anim.SetBool("isMoving", false);
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            if (currentPen != null && !isDead && isFedToday) 
            {
                Vector3 potentialTarget = transform.position;
                bool foundPoint = false;
                
                 for (int i = 0; i < 10; i++)
                {
                    Vector3 randPoint = GetRandomPointInBounds(currentPen.bounds);
                    if (currentPen.OverlapPoint(randPoint))
                    {
                        targetPosition = randPoint;
                        foundPoint = true;
                        break;
                    }
                }

                if (foundPoint)
                {
                    if (targetPosition.x < transform.position.x) sr.flipX = true;
                    else sr.flipX = false;

                    isMoving = true;
                    if (anim != null) anim.SetBool("isMoving", true);

                    float timeLimit = 5f;
                    float timer = 0;
                    
                    while (Vector3.Distance(transform.position, targetPosition) > 0.1f && timer < timeLimit && !isDead)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }
    }
    
    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(randomX, randomY, 0);
    }

    private void ProduceProduct()
{
    if (data.productPrefab != null)
    {
        int amountToProduce = 1; // Số lượng mặc định hàng ngày

        // 1. Kiểm tra Bonus từ kỹ năng chăn nuôi
        if (SkillManager.Instance != null)
        {
            float bonusChance = SkillManager.Instance.GetAnimalProductBonus(); // Trả về 0.1 -> 0.5 (10% - 50%)
            
            // Sử dụng tỉ lệ ngẫu nhiên để quyết định có rơi thêm sản phẩm hay không
            if (Random.value < bonusChance)
            {
                amountToProduce++; 
                Debug.Log($"<color=cyan>Kỹ năng Chăn Nuôi kích hoạt! {data.animalName} tạo thêm sản phẩm bonus.</color>");
            }
        }

        // 2. Sinh ra (Spawn) các sản phẩm dựa trên số lượng đã tính
        for (int i = 0; i < amountToProduce; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(data.productPrefab, spawnPos, Quaternion.identity);
        }
    }
}

    public bool IsDead() => isDead;
    public AnimalType GetAnimalType() 
    {
        return data.type;
    }

    public AnimalTier GetTier() 
    {
        return data.tier;
    }

    public bool IsAdult() 
    {
        return isAdult;
    }
}