using System;
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
    
    [Header("Bed Spawn Point")]
    [Tooltip("Vị trí nhân vật sẽ xuất hiện khi Ngủ/Ngất xỉu.")]
    public Transform bedSpawnPoint;

    private StaminaController staminaController; 
    
    private float lastMoveX;
    private float lastMoveY;

    [Header("Farming")]
    [SerializeField] private float interactionDistance = 0.5f;
    
    [Header("Farming Highlight")]
    [Tooltip("Kéo Prefab ô vuông highlight vào đây")]
    public GameObject tileHighlightPrefab;
    private GameObject currentHighlight; 

    [Header("UI Panels")]
    public GameObject backpackPanel;
    
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
        {
            Debug.LogError("StaminaController not found in the scene.");
        }
        
        farmlandManager = FindFirstObjectByType<FarmlandManager>();
        grid = FindFirstObjectByType<Grid>();
        if (farmlandManager == null) Debug.LogError("Player: Không tìm thấy FarmlandManager");
        if (grid == null) Debug.LogError("Player: Không tìm thấy Grid");

        if (backpackPanel != null)
        {
            backpackPanel.SetActive(false);
        }
        
        if (tileHighlightPrefab != null)
        {
            currentHighlight = Instantiate(tileHighlightPrefab, Vector3.zero, Quaternion.identity);
            currentHighlight.SetActive(false);
        }
        
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
    }

    private void OnDisable()
    {
        playerControls.Disable();
        
        if (staminaController != null)
        {
            staminaController.OnPlayerFaint -= HandlePlayerFaint;
            staminaController.OnPlayerWakeUp -= HandlePlayerWakeUp;
        }
    }
    
    private void Update()
    {
        if (staminaController != null && !staminaController.IsFainted())

        {
            if (!isInteracting && !isBackpackOpen)
            {
                PlayerInput();
                HandleHighlighting();
            }
        }
        else
        {
            movement = Vector2.zero;
            if (currentHighlight != null) currentHighlight.SetActive(false);
        }
    }
    
    private void FixedUpdate()
    {
        if (staminaController != null && !staminaController.IsFainted() && !isInteracting && !isBackpackOpen)
        {
            Move();
        }
        else
        {
            // Tạm dừng di chuyển (sử dụng velocity thay vì linearVelocity)
            rb.linearVelocity = Vector2.zero; 
        }
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        
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
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void HandlePlayerFaint()
    {
        playerControls.Disable();
        movement = Vector2.zero; 
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isFainted", true);
        
        if (currentHighlight != null) currentHighlight.SetActive(false);
        
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
        
        Debug.Log("PlayerController: Input enabled, starting new day.");
    }
    
    public void UseTool(float staminaCost)
    {
        if (staminaController != null && !staminaController.IsFainted())
        {
            staminaController.ConsumeStamina(staminaCost);
            Debug.Log($"Used tool. Stamina remaining: {staminaController.GetCurrentStamina()}");
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInteracting) return; 
        if (staminaController != null && staminaController.IsFainted()) return;
        
        if (currentHighlight != null && currentHighlight.activeSelf)
        {
            StartCoroutine(HandleInteraction());
        }
    }

    private void HandleHighlighting()
    {
        if (farmlandManager == null || grid == null || currentHighlight == null)
        {
            if (currentHighlight != null) currentHighlight.SetActive(false);
            return;
        }

        ItemData currentItem = GetCurrentHeldItem();
        Vector3 playerDirection = new Vector3(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"), 0).normalized;
        if (playerDirection == Vector3.zero) playerDirection = Vector3.down; 
        
        Vector3Int targetCellPos = grid.WorldToCell(transform.position + playerDirection * interactionDistance);

        bool canInteract = farmlandManager.CanInteract(targetCellPos, currentItem);

        if (canInteract)
        {
            Vector3 cellWorldPos = grid.GetCellCenterWorld(targetCellPos);
            currentHighlight.transform.position = cellWorldPos;
            currentHighlight.SetActive(true);
        }
        else
        {
            currentHighlight.SetActive(false);
        }
    }

    private IEnumerator HandleInteraction()
    {
        if (currentHighlight != null) currentHighlight.SetActive(false); 

        isInteracting = true;

        ItemData currentItem = GetCurrentHeldItem();
        Vector3 playerDirection = new Vector3(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"), 0).normalized;
        if (playerDirection == Vector3.zero) playerDirection = Vector3.down;
        Vector3Int targetCellPos = grid.WorldToCell(transform.position + playerDirection * interactionDistance);

        float staminaCost = (currentItem == null) ? 2f : currentItem.staminaCost;

        if (staminaController.GetCurrentStamina() < staminaCost)
        {
            Debug.Log("Không đủ Stamina!");
            isInteracting = false;
            yield break; 
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

        if (farmlandManager != null)
        {
            bool actionSucceeded = farmlandManager.Interact(targetCellPos, currentItem);

            if (actionSucceeded)
            {
                UseTool(staminaCost);
            }
        }

        isInteracting = false;
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
            
            if (currentHighlight != null) currentHighlight.SetActive(false);
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
