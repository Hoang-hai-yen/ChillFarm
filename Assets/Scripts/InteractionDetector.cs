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
        if(context.performed && interactableInRange != null && interactableInRange.CanInteract())
        {   
            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null)
        {   interactable.OnInteractableRangeEnter();
            interactableInRange = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null && interactable == interactableInRange)
        {   
            interactable.OnInteractableRangeExit();
            interactableInRange = null;
        }
    }
}