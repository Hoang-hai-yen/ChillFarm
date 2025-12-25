using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private Transform startParent; // Thêm biến này để nhớ nhà cũ
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

        // Lưu lại chính xác cha ban đầu
        startParent = transform.parent;
        parentAfterDrag = transform.parent; // Dữ liệu fallback
        
        transform.SetParent(canvas.transform); 
        transform.SetAsLastSibling();

        image.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // --- SỬA LỖI MẤT HÌNH TẠI ĐÂY ---
        
        // Thay vì dùng parentAfterDrag (có thể bị thay đổi bởi các script khác),
        // Ta bắt buộc nó quay về startParent (nhà cũ).
        transform.SetParent(startParent);

        // Reset vị trí về tâm
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        
        transform.localScale = Vector3.one; 
        transform.localRotation = Quaternion.identity;

        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        
        // Gọi update lại lần nữa cho chắc (đề phòng UI chưa kịp refresh)
        if (InventoryManager.Instance != null) InventoryManager.Instance.ForceSetSlot(InventoryManager.Instance.SelectedHotbarSlot);
    }
}