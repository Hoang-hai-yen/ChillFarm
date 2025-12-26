using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private Transform startParent; 
    private Image image;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); 
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image.sprite == null || image.color.a < 0.1f)
        {
            eventData.pointerDrag = null;
            return; 
        }

        // Lưu cha cũ
        startParent = transform.parent;
        parentAfterDrag = transform.parent; 
        
        // Đưa lên Canvas để kéo không bị che
        transform.SetParent(canvas.transform); 
        transform.SetAsLastSibling();

        image.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Khi kéo, dùng position toàn cục
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. Trả về cha cũ (nếu drop thành công thì InventorySlotUI đã xử lý parentAfterDrag rồi,
        // nhưng dòng này đảm bảo nếu drop ra ngoài thì nó quay về chỗ cũ)
        transform.SetParent(startParent);

        // 2. --- SỬA LỖI MẤT HÌNH Ở ĐÂY ---
        // Reset toàn bộ thông số vị trí, đặc biệt là localPosition để tránh bị lọt xuống dưới background
        transform.localPosition = Vector3.zero; 
        rectTransform.anchoredPosition = Vector2.zero;
        transform.localScale = Vector3.one; 
        transform.localRotation = Quaternion.identity;

        // 3. Bật lại raycast
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        
        // 4. Cập nhật UI lần cuối
        if (InventoryManager.Instance != null) 
            InventoryManager.Instance.ForceSetSlot(InventoryManager.Instance.SelectedHotbarSlot);
    }
}