using System.Collections;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
   public MushroomData data;
    private SpriteRenderer sr;
    
    [Header("Animation")]
    public float growSpeed = 5f; // Tốc độ mọc

    public void Init(MushroomData newData)
    {
        data = newData;
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.mushroomSprite;

        // Bắt đầu với scale bằng 0
        transform.localScale = Vector3.zero;
        
        // Chạy animation mọc
        StartCoroutine(GrowRoutine());
    }

// IEnumerator GrowRoutine()
// {
//     float duration = 0.5f; // Thời gian mọc
//     float elapsed = 0f;
    
//     // Animation Curve giúp tạo hiệu ứng nảy (bạn có thể chỉnh trong Inspector nếu dùng biến public)
//     AnimationCurve growCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

//     while (elapsed < duration)
//     {
//         elapsed += Time.deltaTime;
//         float percent = elapsed / duration;
        
//         // Công thức tạo hiệu ứng nảy nhẹ bằng toán học hoặc Curve
//         float curveValue = growCurve.Evaluate(percent);
        
//         // Bạn có thể nhân thêm hiệu ứng Sin để nó nảy lên
//         float scale = Mathf.Lerp(0, 1.2f, percent); // Mọc lên 1.2
//         if(percent > 0.8f) scale = Mathf.Lerp(1.2f, 1f, (percent - 0.8f) * 5); // Co lại về 1

//         transform.localScale = new Vector3(scale, scale, 1);
//         yield return null;
//     }
//     transform.localScale = Vector3.one;
// }

IEnumerator GrowRoutine()
{
    float duration = 0.5f; // Tổng thời gian mọc
    float elapsed = 0f;
    
    // Đảm bảo bắt đầu từ 0
    transform.localScale = Vector3.zero;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float percent = elapsed / duration;

        float scaleValue;

        // Giai đoạn 1: Mọc nhanh và vượt ngưỡng (Overshoot)
        if (percent < 0.8f) 
        {
            // Lerp từ 0 lên 1.1 (vượt ngưỡng 1.0 một chút)
            // Dùng t = percent / 0.8f để chuẩn hóa thời gian về đoạn [0, 1]
            scaleValue = Mathf.Lerp(0f, 1.15f, percent / 0.8f);
        }
        // Giai đoạn 2: Co lại về kích thước chuẩn
        else 
        {
            // Lerp từ 1.15 về 1.0
            scaleValue = Mathf.Lerp(1.15f, 1.0f, (percent - 0.8f) / 0.2f);
        }

        // Áp dụng scale (đảm bảo Pivot của Sprite đã đặt là Bottom để mọc từ đất lên)
        transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
        
        yield return null;
    }

    // Kết thúc chính xác ở 1
    transform.localScale = Vector3.one;
}
    public void Collected()
    {
        Debug.Log("Picking " + data.itemName);
        Destroy(gameObject);
        InventoryManager.Instance.AddItem(data);
    }
}