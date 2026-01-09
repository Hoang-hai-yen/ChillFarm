using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;

    private bool isOpen = false;
    private bool playerInRange = false;

    private Animator animator;
    private BoxCollider2D doorCollider;

    void Start()
    {
        animator = door.GetComponent<Animator>();
        doorCollider = door.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;

        if (animator != null)
        {
            animator.SetBool("isOpen", isOpen);
        }

        doorCollider.enabled = !isOpen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
