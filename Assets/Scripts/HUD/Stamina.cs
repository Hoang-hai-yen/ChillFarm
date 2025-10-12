using UnityEngine;
using UnityEngine.UI;

public class StaminaHUD : MonoBehaviour
{
    [Header("Stamina Bar References")]
    [SerializeField] private GameObject emptyBar; // Thanh trống (Stamina-0)
    [SerializeField] private GameObject[] staminaBars; // 4 ô stamina (Stamina-1, 2, 3, 4)

    [Header("Animation Settings")]
    [SerializeField] private float transitionSpeed = 5f; // Tốc độ chuyển đổi giữa các trạng thái

    [Header("References")]
    [SerializeField] private StaminaController staminaController;

    private int currentActiveBar = 4; // Số ô đang hiện (0-4)
    private int targetActiveBar = 4;

    void Awake()
    {
        if (staminaController == null)
        {
            staminaController = FindFirstObjectByType<StaminaController>();
        }

        if (staminaController == null)
        {
            Debug.LogError("StaminaController not found!");
            return;
        }
    }

    void OnEnable()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaChange += UpdateStaminaDisplay;
            staminaController.OnPlayerWakeUp += OnPlayerWakeUp;
        }
    }

    void OnDisable()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaChange -= UpdateStaminaDisplay;
            staminaController.OnPlayerWakeUp -= OnPlayerWakeUp;
        }
    }

    void Start()
    {
        UpdateStaminaDisplay(staminaController.GetCurrentStamina());
    }

    void Update()
    {
        // Smooth transition giữa các trạng thái
        if (currentActiveBar != targetActiveBar)
        {
            float transition = Mathf.MoveTowards(currentActiveBar, targetActiveBar, transitionSpeed * Time.deltaTime);
            currentActiveBar = Mathf.RoundToInt(transition);
            UpdateBarVisuals();
        }
    }

    private void UpdateStaminaDisplay(float currentStamina)
    {
        float maxStamina = staminaController.maxStamina;
        float staminaPercent = currentStamina / maxStamina;

        // Tính số ô cần hiển thị (0-4)
        // 0% = 0 ô, 25% = 1 ô, 50% = 2 ô, 75% = 3 ô, 100% = 4 ô
        targetActiveBar = Mathf.RoundToInt(staminaPercent * staminaBars.Length);

        UpdateBarVisuals();
    }

    private void UpdateBarVisuals()
    {
        // Hiển thị thanh trống khi không có ô nào active
        if (emptyBar != null)
        {
            emptyBar.SetActive(currentActiveBar == 0);
        }

        // Hiển thị các ô stamina
        for (int i = 0; i < staminaBars.Length; i++)
        {
            if (staminaBars[i] != null)
            {
                staminaBars[i].SetActive(i < currentActiveBar);
            }
        }
    }

    private void OnPlayerWakeUp()
    {
        Debug.Log("Player woke up! Restoring stamina display.");
        UpdateStaminaDisplay(staminaController.GetCurrentStamina());
    }
}