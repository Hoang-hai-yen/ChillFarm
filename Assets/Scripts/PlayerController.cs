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
            PlayerInput();
        }
        else
        {
            movement = Vector2.zero;
        }
    }
    
    private void FixedUpdate()
    {
        if (staminaController != null && !staminaController.IsFainted())
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
}