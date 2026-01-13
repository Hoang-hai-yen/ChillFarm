using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class StaminaHUD : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo component Image trên Canvas vào đây")]
    [SerializeField] private Image staminaImage;

    [Tooltip("Kéo component TextMeshProUGUI vào đây để hiện số")]
    [SerializeField] private TextMeshProUGUI staminaText; 

    [Header("Sprite Configuration")]
    [Tooltip("Kéo các sprite vào theo thứ tự: 0 (Rỗng) -> 1 -> 2 -> 3 -> 4 (Đầy)")]
    [SerializeField] private Sprite[] staminaSprites; 

    [Header("Logic References")]
    [SerializeField] private StaminaController staminaController;

    void Awake()
    {
        if (staminaController == null)
            staminaController = FindFirstObjectByType<StaminaController>();

        if (staminaImage == null)
            staminaImage = GetComponent<Image>();
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

    private void UpdateStaminaDisplay(float currentStamina)
    {
        if (staminaSprites.Length > 0 && staminaImage != null)
        {
            float maxStamina = staminaController.maxStamina;
            float percentage = Mathf.Clamp01(currentStamina / maxStamina);
            int index = Mathf.FloorToInt(percentage * (staminaSprites.Length - 1));

            if (currentStamina > 0 && index == 0) index = 1;
            staminaImage.sprite = staminaSprites[index];
        }

        if (staminaText != null)
        {
            int current = Mathf.CeilToInt(currentStamina);
            int max = (int)staminaController.maxStamina;
            
            staminaText.text = $"{current} / {max}";
        }
    }

    private void OnPlayerWakeUp()
    {
        UpdateStaminaDisplay(staminaController.GetCurrentStamina());
    }
}