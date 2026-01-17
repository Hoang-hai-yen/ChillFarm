using System;
using System.Collections;
using Assets.Scripts.Cloud.Schemas;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDataPersistence
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
    private TimeController timeController; 

    private float lastMoveX;
    private float lastMoveY;

    [Header("Farming")]
    [SerializeField] private float interactionDistance = 0.5f;

    [Header("UI Panels")]
    public GameObject backpackPanel;
    [Header("Highlight")]
    [Tooltip("Đối tượng SpriteRenderer dùng để highlight ô đất.")]
    [SerializeField] private SpriteRenderer highlightRenderer;
    [Header("Audio")]
    [SerializeField] private AudioClip faintClip; 
    private AudioSource audioSource;
    [Header("UI Managers")]
    [SerializeField] private FaintUIManager faintUIManager;


    private FarmlandManager farmlandManager;

    private FishingController fishingController;

    private InteractionDetector interactionDetector;
    private Grid grid;
    private bool isBackpackOpen = false;

    private bool isInteracting = false;

    [Header("Mushroom")]    
    public LayerMask mushroomLayer;
    [SerializeField] private float mushroomPickingRadius = 0.5f;
    public static PlayerController Instance { get; private set; }
    public void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fishingController = GetComponent<FishingController>();
        interactionDetector = GetComponent<InteractionDetector>();
        staminaController = FindFirstObjectByType<StaminaController>();
        timeController = FindFirstObjectByType<TimeController>();
        if (timeController == null) Debug.LogError("TimeController not found!");
        if (staminaController == null)
            Debug.LogError("StaminaController not found in the scene.");

        farmlandManager = FindFirstObjectByType<FarmlandManager>();
        grid = FindFirstObjectByType<Grid>();
        if (farmlandManager == null) Debug.LogError("Player: Không tìm thấy FarmlandManager");
        if (grid == null) Debug.LogError("Player: Không tìm thấy Grid");

        if (backpackPanel != null)
            backpackPanel.SetActive(false);

        lastMoveY = -1f;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
        if (staminaController != null)
        {
            staminaController.OnPlayerFaint += HandleStaminaFaint; 
            staminaController.OnPlayerWakeUp += HandlePlayerWakeUp;
        }

        if (timeController != null)
        {
            timeController.OnPassOutTime += HandleTimePassOut;
        }

        playerControls.Movement.Interact.performed += OnInteract;
        playerControls.Movement.Interact.performed += interactionDetector.OnInteract;
        playerControls.Movement.Interact.Enable();

        playerControls.Movement.Backpack.performed += ToggleBackpack;
        playerControls.Movement.Backpack.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

        if (staminaController != null)
        {
    staminaController.OnPlayerFaint -= HandleStaminaFaint;
            staminaController.OnPlayerWakeUp -= HandlePlayerWakeUp;
        }
        if (timeController != null)
        {
            timeController.OnPassOutTime -= HandleTimePassOut;
        }

        playerControls.Movement.Interact.performed -= OnInteract;
        playerControls.Movement.Interact.performed -= interactionDetector.OnInteract;
        playerControls.Movement.Interact.Disable();

        playerControls.Movement.Backpack.performed -= ToggleBackpack;
        playerControls.Movement.Backpack.Disable();
    }

    public void LoadData(GameData data)
    {
        if (data.PlayerDataData != null)
        {
            // PlayerData.PlayerPosition pos = data.PlayerDataData.Position;
            // transform.position = new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.PlayerDataData == null)
        {
            data.PlayerDataData = new PlayerData();
        }

        data.PlayerDataData.Position = new PlayerData.PlayerPosition()
        {
            X = transform.position.x,
            Y = transform.position.y,
            Z = transform.position.z
        };
    }

    public void HandleUpdate()
    {
        if (staminaController != null && !staminaController.IsFainted() && !isInteracting)
        {
            if(!animator.GetBool("isFishing"))
            {
                PlayerInput();

            }
            UpdateInteractionHighlight();
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

        float baseStaminaCost = (currentItem == null) ? 2f : currentItem.staminaCost;

        float reduction = SkillManager.Instance.GetStaminaReduction();
        float finalStaminaCost = baseStaminaCost * (1f - reduction);

        if (staminaController == null || staminaController.GetCurrentStamina() < finalStaminaCost)
        {
            Debug.Log("Không đủ Stamina!");
            isInteracting = false;
            yield break;
        }

        bool actionSuccessful = false;

        // Quét tất cả vật thể trong phạm vi tương tác
        Collider2D[] hits = Physics2D.OverlapCircleAll(interactionWorldPos, 0.5f);
        
        foreach (var hit in hits)
        {
            // 1. Kiểm tra Vật nuôi (FarmAnimal)
            FarmAnimal animal = hit.GetComponent<FarmAnimal>();
            if (animal != null)
            {
                if (animal.IsDead())
                {
                    float cleanUpCost = 5f * (1f - reduction);

                    if (staminaController.GetCurrentStamina() >= cleanUpCost)
                    {
                        animator.SetTrigger("doAction");
                        animal.CleanupCorpse();

                        finalStaminaCost = cleanUpCost;
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
                        finalStaminaCost = 1f * (1f - reduction);
                    }

                    if (actionSuccessful)
                    {
                        yield return new WaitForSeconds(0.5f);
                        goto FinalizeInteraction;
                    }
                }
            }

            Bed bed = hit.GetComponent<Bed>();
            if (bed != null)
            {
                if (bed.Sleep())
                {
                    Debug.Log("Người chơi đã đi ngủ!");
                    
                    actionSuccessful = true;
                    finalStaminaCost = 0f; 
                    
                    yield return new WaitForSeconds(1f);
                    
                    
                    goto FinalizeInteraction;
                }
            }

            // 2. Kiểm tra Cửa chuồng (BarnDoor)
            DoorController door = hit.GetComponent<DoorController>();
            if (door != null)
            {
                if (door.InteractWithDoor())
                {
                    animator.SetTrigger("doAction"); 
                    finalStaminaCost = 0f; 
                    actionSuccessful = true;
                    yield return new WaitForSeconds(0.5f);
                    goto FinalizeInteraction;
                }
            }

            // 3. Kiểm tra Nhà trong chuồng (BarnStructure)
            BarnStructure barnHouse = hit.GetComponent<BarnStructure>();
            if (barnHouse != null)
            {
                if (barnHouse.TryUpgrade())
                {
                    animator.SetTrigger("doAction"); 
                    AudioManager.Instance.PlayDigPlant();
                    actionSuccessful = true;
                    yield return new WaitForSeconds(0.5f);
                    goto FinalizeInteraction;
                }
            }

            // 4. Kiểm tra Chuồng (AnimalPen) để thả vật nuôi
            AnimalPen pen = hit.GetComponent<AnimalPen>();
            if (pen != null)
            {
                if (currentItem is LivestockItemData livestockItem)
                {
                    if (livestockItem.animalType == pen.allowedAnimal)
                    {
                        // Đếm số lượng vật nuôi hiện tại trong chuồng
                        // Collider2D[] animalsInside = Physics2D.OverlapBoxAll(pen.GetBounds().bounds.center, pen.GetBounds().bounds.size, 0);
                        // int count = 0;
                        // foreach (var col in animalsInside)
                        // {
                        //     FarmAnimal fa = col.GetComponent<FarmAnimal>();
                        //     if (fa != null && fa.GetAnimalType() == pen.allowedAnimal && !fa.IsDead())
                        //     {
                        //         count++;
                        //     }
                        // }

                        // Kiểm tra sức chứa (dựa trên code AnimalPen mới đã thêm IsFull)
                        if (pen.IsFull()) 
                        {
                            Debug.Log("Chuồng đã đầy! Hãy nâng cấp.");
                        }
                        else
                        {
                            GameObject newAnimalObj = Instantiate(livestockItem.animalPrefab, interactionWorldPos, Quaternion.identity);
                            FarmAnimal newAnimalScript = newAnimalObj.GetComponent<FarmAnimal>();

                            newAnimalScript.SetHome(pen.GetBounds());
                            
                            pen.AddAniamal(newAnimalScript);
                            InventoryManager.Instance.RemoveItem(currentItem, 1);

                            animator.SetTrigger("doAction");
                            Debug.Log("Đã thả gà vào chuồng!");

                            actionSuccessful = true;
                            yield return new WaitForSeconds(0.5f);
                            goto FinalizeInteraction;
                        }
                    }
                    else
                    {
                        Debug.Log("Sai chuồng rồi! Đây là chuồng " + pen.allowedAnimal);
                    }
                }
            }
        } // Kết thúc vòng lặp foreach


        // 5. Nếu không tương tác với Entity nào thì dùng Tool hoặc Tương tác đất
        if (currentItem is ToolData tool)
        {
            if (tool.toolType == ToolType.Hand)
            {
                TryPickMushroom();
            }
            else if (tool.toolType == ToolType.Hoe)
            {
                animator.SetTrigger("useHoe");
                AudioManager.Instance.PlayDigPlant();
                yield return new WaitForSeconds(0.5f);
            }
            else if (tool.toolType == ToolType.WateringCan)
            {
                animator.SetTrigger("useWaterCan");
                AudioManager.Instance.PlayFishing();
                yield return new WaitForSeconds(0.7f);
            }
            else if (tool.toolType == ToolType.FishingRod)
            {
                if (!animator.GetBool("isFishing") && fishingController.CanFishInDirection())
                {
                    animator.SetBool("isFishing", true);
                    AudioManager.Instance.PlayFishing();
                    yield return new WaitForSeconds(0.7f);
                }
                else
                {
                    animator.SetBool("isFishing", false);
                    AudioManager.Instance.PlayFishing();
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        else if (currentItem != null && (currentItem.itemType == ItemType.Seed || currentItem.itemType == ItemType.Fertilizer))
        {
            animator.SetTrigger("doAction");
            AudioManager.Instance.PlayDigPlant();
            yield return new WaitForSeconds(0.2f);
        }
        else if (currentItem == null)
        {
            animator.SetTrigger("doAction");
            yield return new WaitForSeconds(0.2f);
        }
        else if (currentItem is FoodData foodItem)
        {
            if (staminaController.GetCurrentStamina() >= staminaController.maxStamina)
            {
                Debug.Log("Bụng no rồi, không ăn nổi nữa!");
            }
            else
            {
                animator.SetTrigger("doAction"); 

                if (foodItem.eatSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(foodItem.eatSound);
                }

                staminaController.RestoreStamina(foodItem.staminaRecover);
                Debug.Log($"Đã ăn {foodItem.itemName}, hồi {foodItem.staminaRecover} sức.");

                InventoryManager.Instance.RemoveItem(currentItem, 1);

                actionSuccessful = true;
                finalStaminaCost = 0f;
                yield return new WaitForSeconds(0.5f); 
            }
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
        if (actionSuccessful && finalStaminaCost > 0)
        {
            if (staminaController.ConsumeStamina(finalStaminaCost))
                Debug.Log($"Tiêu tốn {finalStaminaCost} Stamina.");
        }
        else if (!actionSuccessful)
        {
            Debug.Log("Hành động thất bại hoặc không có mục tiêu hợp lệ.");
        }

        isInteracting = false;
    }

    private IEnumerator HandleNPCInteraction()
    {
        isInteracting = true;

        Vector3 playerDirection = new Vector3(
            animator.GetFloat("lastMoveX"),
            animator.GetFloat("lastMoveY"),
            0
        ).normalized;

        if (playerDirection == Vector3.zero)
            playerDirection = Vector3.down;

        Vector3 interactionWorldPos = transform.position + playerDirection * interactionDistance;

        Collider2D[] hits = Physics2D.OverlapCircleAll(interactionWorldPos, 0.5f);

        foreach (var hit in hits)
        {
            QuestGiver questGiver = hit.GetComponent<QuestGiver>();
            if (questGiver != null && questGiver.CanInteract(transform))
            {
                QuestDialogManager.Instance.Open(questGiver);
                isInteracting = false;
                yield break; 
            }

            Interactable npc = hit.GetComponent<Interactable>();
            if (npc != null)
            {
                NPCController npcController = hit.GetComponent<NPCController>();
                if (npcController != null)
                    npcController.PauseDirect();

                DialogData dialogData = hit.GetComponent<DialogData>();
                if (dialogData != null && DialogManager.Instance != null)
                {
                    Dialog dialog = dialogData.CreateDialog();
                    yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
                }
                else
                {
                    npc.Interact();
                }

                isInteracting = false;
                yield break; 
            }
        }

        isInteracting = false;
        yield return HandleInteraction();
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
            fishingController.lastFacingDirection = movement;

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

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInteracting) return;
        if (staminaController != null && staminaController.IsFainted()) return;

        StartCoroutine(HandleNPCInteraction());
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

    void TryPickMushroom()
    {
        Vector2 checkPosition = (Vector2)transform.position + (new Vector2(lastMoveX, lastMoveY) * interactionDistance);

        Collider2D hit = Physics2D.OverlapCircle(checkPosition, mushroomPickingRadius, mushroomLayer);
        
        if (hit != null)
        {
            Mushroom mush = hit.GetComponent<Mushroom>();
            if (mush != null)
            {
                mush.Collected();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mushroomPickingRadius);
    }
    private void HandleStaminaFaint()
    {
        Debug.Log("Player: Ngất do hết Stamina.");
        StartCoroutine(ProcessFaintSequence());
    }

    private void HandleTimePassOut()
    {
        Debug.Log("Player: Ngất do thức quá 3 ngày.");
        StartCoroutine(ProcessFaintSequence());
    }

    private IEnumerator ProcessFaintSequence()
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

        yield return new WaitForSeconds(1f);

        if (faintClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(faintClip);
        }

        yield return new WaitForSeconds(2f);

        if (faintUIManager != null)
        {
            yield return StartCoroutine(faintUIManager.PlayFaintSequence(() => 
            {
                if (timeController != null)
                {
                    timeController.SkipToNextDayStart();
                }
            }));
        }
        else
        {
            if (timeController != null) timeController.SkipToNextDayStart();
        }
    }

    private void HandlePlayerWakeUp()
    {
        if (bedSpawnPoint != null)
        {
            transform.position = bedSpawnPoint.position;
            animator.SetFloat("lastMoveX", 0f);
            animator.SetFloat("lastMoveY", -1f);
        }
        else
        {
            Debug.LogError("Chưa gán Bed Spawn Point!");
        }

        animator.SetBool("isFainted", false);
        playerControls.Enable();

        playerControls.Movement.Move.Enable();
        playerControls.Movement.Interact.Enable();

        Debug.Log("Player: Đã tỉnh dậy ở giường.");
    }
}
