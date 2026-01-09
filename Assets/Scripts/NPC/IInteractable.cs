public interface IInteractable
{
    void Interact();
    bool CanInteract();

    void OnInteractableRangeEnter();
    void OnInteractableRangeExit();
    
}