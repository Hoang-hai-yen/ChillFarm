using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Cài đặt")]
    public string doorName = "Chuồng Gà"; 
    public bool isLocked = false; 
    public int unlockCost = 500; 

    [Header("Components")]
    public Transform doorVisuals; 
    
    private bool isOpen = false;
    private Animator animator;
    private BoxCollider2D doorCollider;

    void Start()
    {
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
                    Debug.Log("Mở khóa thành công!");
                    SetDoorState(true); 
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