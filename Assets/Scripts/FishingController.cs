using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    [Header("Cài đặt chung")]
    // [Tooltip("Nút bấm để bắt đầu câu")]
    // public KeyCode fishKey = KeyCode.Space;
    [Tooltip("Vị trí hiển thị kết quả trên đầu Player")]
    public SpriteRenderer resultDisplaySprite;
    [Tooltip("Thời gian hiển thị kết quả trước khi biến mất")]
    public float resultDisplayDuration = 2f;

    [Header("Thời gian chờ cá cắn câu")]
    public float minWaitTime = 3f;
    public float maxWaitTime = 7f;

    [Header("Tỉ lệ và Danh sách Cá")]
    [Tooltip("Tỉ lệ không câu được gì (0.1 = 10%)")]
    [Range(0f, 1f)] public float missChance = 0.1f;
    [Tooltip("Sprite hiển thị khi không câu được gì (Rác/Hụt)")]
    public Sprite missSprite;
    [Tooltip("Kéo các FishData bạn đã tạo vào danh sách này")]
    public List<FishData> fishPool = new List<FishData>();

    [Header("Cấu hình 4 hướng")]
    public float fishingDistance = 1.0f; // Khoảng cách quăng cần ra xa nhân vật
    public float checkRadius = 0.3f;    // Độ rộng của điểm kiểm tra
    public LayerMask waterLayer;

    private Animator animator;

    private bool isFishing = false;
    private Coroutine currentFishingRoutine;
    public Vector2 lastFacingDirection = Vector2.down;

    void Start()
    {
        if (resultDisplaySprite != null)
            resultDisplaySprite.gameObject.SetActive(false);
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        if (animator.GetBool("isFishing") && !isFishing)
        {
            
                StartFishing();
           
        }
    }

    void StartFishing()
    {
        if (fishPool.Count == 0)
        {
            Debug.LogError("Chưa gán danh sách cá vào Fish Pool!");
            return;
        }

        isFishing = true;
        Debug.Log("Bắt đầu thả câu... Đang chờ...");
        
        currentFishingRoutine = StartCoroutine(WaitForFishCoroutine());
    }

    IEnumerator WaitForFishCoroutine()
    {
        // Random thời gian chờ
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // Hết thời gian chờ, tiến hành kéo cá
        if(animator.GetBool("isFishing"))
        {
            yield return FinishFishing();

        }
        else
        {
            isFishing = false;
        }
    }

    IEnumerator FinishFishing()
    {
        isFishing = false;
        Debug.Log("Đã kéo cần!");
        animator.SetBool("isFishing", false);
        yield return new WaitForSeconds(0.5f);

        // 1. Kiểm tra xem có bị hụt không (Miss chance)
        if (Random.value < missChance)
        {
            
            ShowResult(missSprite, "Không câu được gì cả!");
        }
        else
        {
            // 2. Nếu không hụt, chọn random cá dựa trên độ hiếm
            FishData caughtFish = GetRandomFishWeighted();
            if (caughtFish != null)
            {
               
                ShowResult(caughtFish.itemIcon, $"Bạn đã câu được: {caughtFish.itemName}!");

            }
        }

    }

    private FishData GetRandomFishWeighted()
    {
        int totalWeight = 0;
        foreach (var fish in fishPool)
        {
            totalWeight += fish.rarityWeight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (var fish in fishPool)
        {
            currentWeightSum += fish.rarityWeight;
            if (randomValue < currentWeightSum)
            {
                return fish;
            }
        }
        return fishPool[0];
    }

    private void ShowResult(Sprite spriteToShow, string debugMessage)
    {
        Debug.Log(debugMessage);

        if (resultDisplaySprite != null)
        {
            resultDisplaySprite.sprite = spriteToShow;
            resultDisplaySprite.gameObject.SetActive(true);
            StartCoroutine(HideResultAfterDelay());
        }
    }

    IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayDuration);
        if (resultDisplaySprite != null)
        {
            resultDisplaySprite.gameObject.SetActive(false);
        }
    }


    public bool CanFishInDirection()
    {
        // Tính toán vị trí kiểm tra: Vị trí hiện tại + (Hướng nhìn * Khoảng cách)
        Vector2 checkPosition = (Vector2)transform.position + (lastFacingDirection * fishingDistance);
        
        // Kiểm tra xem tại vị trí đó có Layer Water không
        Collider2D hit = Physics2D.OverlapCircle(checkPosition, checkRadius, waterLayer);
        Debug.Log(hit != null ? "Water" : "Not water");
        return hit != null;
    }

    // Vẽ để bạn dễ căn chỉnh trong cửa sổ Scene
    void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + (lastFacingDirection * fishingDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPosition, checkRadius);
    }
}