using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public LayerMask interactableLayer;

    [Header("Bed Spawn Point")]
    [Tooltip("Vị trí nhân vật sẽ xuất hiện khi Ngủ/Ngất xỉu.")]
    public Transform bedSpawnPoint;

    private StaminaController staminaController;

    private float lastMoveX;
    private float lastMoveY;

    [Header("Farming")]
    [SerializeField] private float interactionDistance = 0.5f;

    [Header("UI Panels")]
    public GameObject backpackPanel;
    [Header("Highlight")]
    [Tooltip("Đối tượng SpriteRenderer dùng để highlight ô đất.")]
    [SerializeField] private SpriteRenderer highlightRenderer;

    private FarmlandManager farmlandManager;
    private Grid grid;
    private bool isBackpackOpen = false;

    private bool isInteracting = false;

    public void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        staminaController = FindFirstObjectByType<StaminaController>();
        if (staminaController == null)
            Debug.LogError("StaminaController not found in the scene.");

        farmlandManager = FindFirstObjectByType<FarmlandManager>();
        grid = FindFirstObjectByType<Grid>();
        if (farmlandManager == null) Debug.LogError("Player: Không tìm thấy FarmlandManager");
        if (grid == null) Debug.LogError("Player: Không tìm thấy Grid");

        if (backpackPanel != null)
            backpackPanel.SetActive(false);

        lastMoveY = -1f;
    }

    private void OnEnable()
    {
        playerControls.Enable();

        if (staminaController != null)
        {
            staminaController.OnPlayerFaint += HandlePlayerFaint;
            staminaController.OnPlayerWakeUp += HandlePlayerWakeUp;
        }

        playerControls.Movement.Interact.performed += OnInteract;
        playerControls.Movement.Interact.Enable();

        playerControls.Movement.Backpack.performed += ToggleBackpack;
        playerControls.Movement.Backpack.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

        if (staminaController != null)
        {
            staminaController.OnPlayerFaint -= HandlePlayerFaint;
            staminaController.OnPlayerWakeUp -= HandlePlayerWakeUp;
        }

        playerControls.Movement.Interact.performed -= OnInteract;
        playerControls.Movement.Interact.Disable();

        playerControls.Movement.Backpack.performed -= ToggleBackpack;
        playerControls.Movement.Backpack.Disable();
    }

    public void HandleUpdate()
    {
        if (staminaController != null && !staminaController.IsFainted() && !isInteracting)
        {
            PlayerInput();
            UpdateInteractionHighlight();

            // Phím Z cho NPC interaction
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                StartCoroutine(HandleNPCInteraction());
            }
        }
        else
        {
            movement = Vector2.zero;
            if (highlightRenderer != null)
                highlightRenderer.gameObject.SetActive(false);
        }
    }

    private IEnumerator HandleInteraction()
    {
        isInteracting = true;

        ItemData currentItem = GetCurrentHeldItem();
        Vector3 playerDirection = new Vector3(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"), 0).normalized;
        if (playerDirection == Vector3.zero) playerDirection = Vector3.down;

        Vector3 interactionWorldPos = transform.position + playerDirection * interactionDistance;
        Vector3Int targetCellPos = grid.WorldToCell(interactionWorldPos);

        float staminaCost = (currentItem == null) ? 2f : currentItem.staminaCost;
        
        if (staminaController == null || staminaController.GetCurrentStamina() < staminaCost)
        {
            Debug.Log("Không đủ Stamina!");
            isInteracting = false;
            yield break;
        }

        bool actionSuccessful = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(interactionWorldPos, 0.5f);
        foreach (var hit in hits)
        {
            FarmAnimal animal = hit.GetComponent<FarmAnimal>();
        if (animal != null)
        {
            if (animal.IsDead())
            {
                float cleanUpCost = 5f; 
                
                if (staminaController.GetCurrentStamina() >= cleanUpCost)
                {
                    animator.SetTrigger("doAction"); 
                    animal.CleanupCorpse();
                    
                    staminaCost = cleanUpCost; 
                    actionSuccessful = true;
                    
                    yield return new WaitForSeconds(0.5f);
                    goto FinalizeInteraction;
                }
                else
                {
                    Debug.Log("Không đủ sức để dọn dẹp!");
                }
            }
            
            else 
            {
                if (currentItem != null && currentItem.itemType == ItemType.AnimalFood)
                {
                    if (animal.Feed()) 
                    {
                        animator.SetTrigger("doAction"); 
                        InventoryManager.Instance.RemoveItem(currentItem, 1); 
                        actionSuccessful = true;
                    }
                }

                else 
                {
                    animal.Play(); 
                    animator.SetTrigger("petAnimal");
                    actionSuccessful = true;
                    staminaCost = 1f; 
                }

                if (actionSuccessful)
                {
                    yield return new WaitForSeconds(0.5f); 
                    goto FinalizeInteraction; 
                }
            }
        }
            AnimalPen pen = hit.GetComponent<AnimalPen>();
            if (pen != null)
            {
                if (currentItem is LivestockItemData livestockItem)
                {
                    if (livestockItem.animalType == pen.allowedAnimal)
                    {
                        GameObject newAnimalObj = Instantiate(livestockItem.animalPrefab, interactionWorldPos, Quaternion.identity);
                        FarmAnimal newAnimalScript = newAnimalObj.GetComponent<FarmAnimal>();
                        
                        newAnimalScript.SetHome(pen.GetBounds());

                        InventoryManager.Instance.RemoveItem(currentItem, 1);
                        
                        animator.SetTrigger("doAction");
                        Debug.Log("Đã thả gà vào chuồng!");
                        
                        actionSuccessful = true;
                        yield return new WaitForSeconds(0.5f);
                        goto FinalizeInteraction;
                    }
                    else
                    {
                        Debug.Log("Sai chuồng rồi! Đây là chuồng " + pen.allowedAnimal);
                    }
                }
            }
        }


        if (currentItem is ToolData tool)
        {
            if (tool.toolType == ToolType.Hoe)
            {
                animator.SetTrigger("useHoe");
                yield return new WaitForSeconds(0.5f); 
            }
            else if (tool.toolType == ToolType.WateringCan)
            {
                animator.SetTrigger("useWaterCan");
                yield return new WaitForSeconds(0.7f); 
            }
        }
        else if (currentItem != null && (currentItem.itemType == ItemType.Seed || currentItem.itemType == ItemType.Fertilizer))
        {
            animator.SetTrigger("doAction"); 
            yield return new WaitForSeconds(0.2f);
        }
        else if (currentItem == null)
        {
            animator.SetTrigger("doAction");
        }

        if (farmlandManager != null)
        {
            actionSuccessful = farmlandManager.Interact(targetCellPos, currentItem);
            
            if (actionSuccessful && currentItem != null)
            {
                if (currentItem.itemType == ItemType.Seed || currentItem.itemType == ItemType.Fertilizer)
                {
                    InventoryManager.Instance.RemoveItem(currentItem, 1);
                }
            }
        }

        FinalizeInteraction:
        
        if (actionSuccessful && staminaCost > 0)
        {
            if (staminaController.ConsumeStamina(staminaCost))
                Debug.Log($"Action SUCCESSFUL. Stamina remaining: {staminaController.GetCurrentStamina()}");
        }
        else if (!actionSuccessful)
        {
            Debug.Log("Action FAILED. No valid target or insufficient conditions.");
        }

        isInteracting = false;
    }

    private IEnumerator HandleNPCInteraction()
    {
        isInteracting = true;

        Vector3 playerDirection = new Vector3(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"), 0).normalized;
        if (playerDirection == Vector3.zero) playerDirection = Vector3.down;

        Vector3 interactionWorldPos = transform.position + playerDirection * interactionDistance;
        float detectionRadius = 0.2f;

        Collider2D npcCollider = Physics2D.OverlapCircle(interactionWorldPos, detectionRadius, interactableLayer);

        if (npcCollider != null)
        {
            Interactable npc = npcCollider.GetComponent<Interactable>();
            if (npc != null)
            {
                npc.Interact(); // Chỉ NPC
                Debug.Log("NPC interaction triggered via Z key.");
            }
        }

        yield return null;
        isInteracting = false;
    }

    private void FixedUpdate()
    {
        if (staminaController != null && !staminaController.IsFainted() && !isInteracting)
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        animator.SetBool("isMoving", movement != Vector2.zero);

        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);

        if (movement != Vector2.zero)
        {
            lastMoveX = movement.x;
            lastMoveY = movement.y;
        }
        animator.SetFloat("lastMoveX", lastMoveX);
        animator.SetFloat("lastMoveY", lastMoveY);
    }

    private void Move()
    {
        Vector3 targetPos = rb.position + movement * speed * Time.fixedDeltaTime;
        if (IsWalkable(targetPos))
        {
            rb.MovePosition(targetPos);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandlePlayerFaint()
    {
        playerControls.Disable();
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isFainted", true);

        if (isBackpackOpen)
        {
            isBackpackOpen = false;
            backpackPanel.SetActive(false);
        }

        Debug.Log("PlayerController: Input disabled, fainted animation playing.");
    }

    private void HandlePlayerWakeUp()
    {
        if (bedSpawnPoint != null)
        {
            transform.position = bedSpawnPoint.position;
            animator.SetFloat("lastMoveX", 0f);
            animator.SetFloat("lastMoveY", -1f);
            Debug.Log("Player teleported to bed spawn point.");
        }
        else
        {
            Debug.LogError("Bed Spawn Point not assigned in PlayerController! Cannot teleport.");
        }

        animator.SetBool("isFainted", false);
        playerControls.Enable();

        playerControls.Movement.Move.Enable();
        playerControls.Movement.Interact.Enable();

        Debug.Log("PlayerController: Input enabled, starting new day.");
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInteracting) return;
        if (staminaController != null && staminaController.IsFainted()) return;

        StartCoroutine(HandleInteraction()); // E key → tool/farmland
    }

    private void ToggleBackpack(InputAction.CallbackContext context)
    {
        if (isInteracting) return;
        if (staminaController != null && staminaController.IsFainted()) return;

        if (backpackPanel == null) return;

        isBackpackOpen = !isBackpackOpen;
        backpackPanel.SetActive(isBackpackOpen);

        if (isBackpackOpen)
        {
            playerControls.Movement.Move.Disable();
            playerControls.Movement.Interact.Disable();
        }
        else
        {
            playerControls.Movement.Move.Enable();
            playerControls.Movement.Interact.Enable();
        }
    }

    private ItemData GetCurrentHeldItem()
    {
        if (InventoryManager.Instance != null)
        {
            return InventoryManager.Instance.GetSelectedItem();
        }

        Debug.LogWarning("Không tìm thấy InventoryManager!");
        return null;
    }

    private void UpdateInteractionHighlight()
    {
        if (highlightRenderer == null || grid == null || farmlandManager == null)
        {
            if (highlightRenderer != null)
                highlightRenderer.gameObject.SetActive(false);
            return;
        }

        Vector3 playerDirection = new Vector3(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"), 0).normalized;
        if (playerDirection == Vector3.zero)
        {
            playerDirection = new Vector3(lastMoveX, lastMoveY, 0).normalized;
            if (playerDirection == Vector3.zero) playerDirection = Vector3.down;
        }

        Vector3 interactionWorldPos = transform.position + playerDirection * interactionDistance;
        Vector3Int targetCellPos = grid.WorldToCell(interactionWorldPos);

        if (farmlandManager.IsTillableArea(targetCellPos))
        {
            Vector3 cellCenterWorld = grid.GetCellCenterWorld(targetCellPos);
            cellCenterWorld.z = 0;
            highlightRenderer.transform.position = cellCenterWorld;
            highlightRenderer.gameObject.SetActive(true);
        }
        else
        {
            highlightRenderer.gameObject.SetActive(false);
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.2f, interactableLayer);

        if (hit != null)
        {
            if (hit.isTrigger) 
            {
                return true;
            }
            
            return false;
        }

        return true;
    }
    
}
