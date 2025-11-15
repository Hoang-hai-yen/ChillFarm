using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;

public class StaminaController : MonoBehaviour
{
    public float maxStamina = 100f;
    public float eveningHourStart = 22f; 

    private float currentStamina;
    private bool isFainted = false;

    private TimeController timeController;

    public event Action<float> OnStaminaChange;
    public event Action OnPlayerFaint;
    public event Action OnPlayerWakeUp;

    void Awake()
    {
        currentStamina = maxStamina;
        
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
        
        StartCoroutine(SaveStaminaData());
        
        OnPlayerFaint?.Invoke(); 
        
        EndDayAndRecover();
    }

    public void EndDayAndRecover()
    {
        if (timeController != null)
        {
            timeController.SkipToNextDayStart();
        }
    }
    
    public void Recover()
    {
        currentStamina = maxStamina;
        isFainted = false; 
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
    private IEnumerator SaveStaminaData()
    {
        string userId = FirebaseManager.Instance.Auth.LocalId;
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Save Stamina: Không thể lưu vì người dùng chưa đăng nhập!");
            yield break;
        }

        int staminaToSave = (int)currentStamina;

        JObject staminaField = FirebaseDatabaseService.CreateStaminaUpdateObject(staminaToSave);
        
        string collectionName = "players"; 
        
        var database = FirebaseManager.Instance.Database;
        yield return database.UpdateDocument(collectionName, userId, staminaField, (success, error) => {
            if (success) {
                Debug.Log("Stamina đã được lưu lên Firebase!");
            } else {
                Debug.LogError("Lỗi lưu stamina: " + error);
            }
        });
    }
}