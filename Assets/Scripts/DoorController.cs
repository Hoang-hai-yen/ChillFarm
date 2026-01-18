using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    [Header("Cài đặt Cửa")]
    public string doorName = "Cửa Thường"; 
    public bool isLocked = false;
    public int unlockCost = 500; 

    [Header("Tutorial UI (Chỉ dùng cho Cửa Khóa)")]
    [Tooltip("Kéo Panel hướng dẫn vào đây. Nếu là cửa thường thì để trống cũng được.")]
    [SerializeField] private GameObject guidePanel; 
    [SerializeField] private Button closeButton;    

    [Header("Components")]
    public Transform doorVisuals; 
    
    private bool isOpen = false;
    private Animator animator;
    private BoxCollider2D doorCollider;

    void Start()
    {
        // Setup Component
        if (doorVisuals != null)
        {
            animator = doorVisuals.GetComponent<Animator>();
            doorCollider = doorVisuals.GetComponent<BoxCollider2D>();
        }
        else
        {
            animator = GetComponent<Animator>();
            doorCollider = GetComponent<BoxCollider2D>();
        }
        UpdateVisuals();

        if (closeButton != null && guidePanel != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => guidePanel.SetActive(false));
        }
        
        if (guidePanel != null) guidePanel.SetActive(false);
    }

    public bool InteractWithDoor()
    {
        if (isLocked)
        {
            string msg = $"Bạn có muốn mở khóa {doorName} không?";
            
            ConfirmationUI.Instance.ShowQuestion(msg, unlockCost, () => 
            {
                if (InventoryManager.Instance.TrySpendGold(unlockCost))
                {
                    isLocked = false; 
                    SetDoorState(true);
                    Debug.Log("Đã mở khóa cửa: " + doorName);

                    if (guidePanel != null)
                    {
                        guidePanel.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("Không đủ tiền!");
                }
            });

            return true; 
        }
        else
        {
            ToggleDoor();
            return true;
        }
    }

    void ToggleDoor() { SetDoorState(!isOpen); }
    void SetDoorState(bool openState) { isOpen = openState; UpdateVisuals(); }
    private void UpdateVisuals()
    {
        if (animator != null) animator.SetBool("isOpen", isOpen);
        if (doorCollider != null) doorCollider.enabled = !isOpen;
    }
}