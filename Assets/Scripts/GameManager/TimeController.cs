using UnityEngine;
using System;

public class TimeController : MonoBehaviour, IDataPersistence
{
    public float dayDurationInSeconds = 120f; 
    [Range(0, 24)]
    public float startHour = 6f; 

    [Header("Settings")]
    public int maxDaysAwake = 3; 

    public event Action<int> OnDayChange;
    public event Action OnNewDayStart; 
    public event Action OnPassOutTime; 

    private float timeMultiplier;
    private DateTime currentTime;
    private int currentDay = 1;
    
    private int currentDaysAwake = 0;

    private bool isTimePaused = false;

    void Start()
    {
        currentTime = DateTime.Today.AddHours(startHour);
        timeMultiplier = 86400f / dayDurationInSeconds;
    }

    void Update()
    {
        if (isTimePaused) return;
        UpdateGameTime();
    }

    public void PauseTime()
    {
        isTimePaused = true;
    }

    public void ResumeTime()
    {
        isTimePaused = false;
    }

    public void LoadData(GameData data)
    {
        currentTime = data.PlayerDataData.CurrentTime;
        currentDay = data.PlayerDataData.CurrentDay;
        currentDaysAwake = data.PlayerDataData.CurrentDaysAwake;
    }

    public void SaveData(GameData data)
    {
        data.PlayerDataData.CurrentTime = currentTime;
        data.PlayerDataData.CurrentDay = currentDay;
        data.PlayerDataData.CurrentDaysAwake = currentDaysAwake;
    }

    private void UpdateGameTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        
        int newDay = (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
        
        if (newDay > currentDay)
        {
            currentDay = newDay;
            
            currentDaysAwake++;
            Debug.Log($"Người chơi đã thức {currentDaysAwake} ngày liên tục.");

            if (currentDaysAwake >= maxDaysAwake)
            {
                Debug.Log("Đã thức quá 3 ngày! Kích hoạt ngất xỉu.");
                OnPassOutTime?.Invoke();
            }

            OnDayChange?.Invoke(currentDay);
        }
    }
    
    public void SkipToNextDayStart()
    {
        currentTime = currentTime.AddDays(1).Date.AddHours(startHour); 
        
        currentDay = (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
        
        currentDaysAwake = 0; 

        OnDayChange?.Invoke(currentDay);
        OnNewDayStart?.Invoke(); 
        
        Debug.Log($"Đã ngủ/qua ngày mới: Day {currentDay} ({startHour}:00). DaysAwake reset về 0.");
    }

    public float GetCurrentHour() => (float)currentTime.Hour + (float)currentTime.Minute / 60f + (float)currentTime.Second / 3600f;
    public float GetTimeNormalized() => (float)currentTime.TimeOfDay.TotalSeconds / 86400f;
    public string GetFormattedTime() => currentTime.ToString("HH:mm");
    public int GetCurrentDay() => (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
}