using UnityEngine;
using System;

public class StaminaController : MonoBehaviour, IDataPersistence
{
    [Header("Settings")]
    public float maxStamina = 100f;
    public float eveningHourStart = 22f; 
    
    [Header("Sleep Logic")]
    public float lateSleepHour = 23f;        
    public float lateSleepMaxStamina = 80f;  

    private float currentStamina;
    private float staminaToRecover;
    private bool isFainted = false;

    private TimeController timeController;

    public event Action<float> OnStaminaChange;
    public event Action OnPlayerFaint;
    public event Action OnPlayerWakeUp;

    void Awake()
    {
        currentStamina = maxStamina;
        staminaToRecover = maxStamina; 
        
        timeController = FindFirstObjectByType<TimeController>();
        if (timeController == null)
        {
            Debug.LogError("TimeController not found in the scene.");
        }
    }

    void OnEnable()
    {
        if (timeController != null)
        {
            timeController.OnNewDayStart += Recover;
        }
    }

    void OnDisable()
    {
        if (timeController != null)
        {
            timeController.OnNewDayStart -= Recover;
        }
    }

    public void LoadData(GameData data)
    {
        currentStamina = (float) data.PlayerDataData.Stamina;
        maxStamina = (float) data.PlayerDataData.MaxStamina;
        OnStaminaChange?.Invoke(currentStamina);    
    }
    public void SaveData(GameData data)
    {
        data.PlayerDataData.Stamina = currentStamina; 
        data.PlayerDataData.MaxStamina = maxStamina;
    }
    void Update()
    {
        if (currentStamina <= 0 && !isFainted)
        {
            Faint();
        }
    }

    public bool ConsumeStamina(float amount)
    {
        if (isFainted) return false;

        if (currentStamina > 0)
        {
            currentStamina = Mathf.Max(0, currentStamina - amount);
            OnStaminaChange?.Invoke(currentStamina);
            return true;
        }
        return false;
    }
    
    public void RestoreStamina(float amount)
    {
        if (isFainted) return;
        
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        OnStaminaChange?.Invoke(currentStamina);
    }
    
    private void Faint()
    {
        isFainted = true;
        Debug.Log("Player fainted! Stamina hết.");
        
        CalculateRecoveryAmount();

        OnPlayerFaint?.Invoke(); 
    }

    public void EndDayAndRecover()
    {
        if (timeController != null)
        {
            CalculateRecoveryAmount();

            timeController.SkipToNextDayStart();
        }
    }
    
    private void CalculateRecoveryAmount()
    {
        if (timeController == null) return;

        float currentHour = timeController.GetCurrentHour();
        staminaToRecover = lateSleepMaxStamina; 
        Debug.Log($"Đã ngất/Ngủ muộn. Mai chỉ hồi {lateSleepMaxStamina}.");
    }
    
    public void Recover()
    {
        currentStamina = staminaToRecover;
        isFainted = false; 
        
        staminaToRecover = maxStamina;

        OnStaminaChange?.Invoke(currentStamina);
        OnPlayerWakeUp?.Invoke();
        Debug.Log("Player recovered and started a new day.");
    }
    
    public float GetCurrentStamina() => currentStamina;
    public bool IsFainted() => isFainted;

    public bool IsEvening()
    {
        return timeController != null && timeController.GetCurrentHour() >= eveningHourStart;
    }
}