using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;

    private bool isOpen = false;
    private Animator animator;
    private BoxCollider2D doorCollider;

    void Start()
    {
        animator = door.GetComponent<Animator>();
        doorCollider = door.GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;

            if (animator != null)
            {
                animator.SetBool("isOpen", isOpen);
            }

            doorCollider.enabled = !isOpen;
        }
    }
}
