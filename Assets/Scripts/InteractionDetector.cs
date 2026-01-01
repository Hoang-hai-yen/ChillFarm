using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    void Start()
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interaction input received");
        if(context.performed && interactableInRange != null && interactableInRange.CanInteract())
        {   Debug.Log("Interacting with " + interactableInRange.ToString());
            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null)
        {
            interactableInRange = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }
}