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

        startParent = transform.parent;
        parentAfterDrag = transform.parent; 
        
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
        transform.SetParent(startParent);

        transform.localPosition = Vector3.zero; 
        rectTransform.anchoredPosition = Vector2.zero;
        transform.localScale = Vector3.one; 
        transform.localRotation = Quaternion.identity;

        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        
        if (InventoryManager.Instance != null) 
            InventoryManager.Instance.ForceSetSlot(InventoryManager.Instance.SelectedHotbarSlot);
    }
}