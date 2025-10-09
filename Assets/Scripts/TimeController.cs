using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{
    public float dayDurationInSeconds = 120f; 
    [Range(0, 24)]
    public float startHour = 6f; 

    public event Action<int> OnDayChange;
    public event Action OnNewDayStart; 

    private float timeMultiplier;
    private DateTime currentTime;
    private int currentDay = 1;

    void Start()
    {
        currentTime = DateTime.Today.AddHours(startHour);
        timeMultiplier = 86400f / dayDurationInSeconds;
    }

    void Update()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        
        int newDay = (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
        
        if (newDay > currentDay)
        {
            currentDay = newDay;
            OnDayChange?.Invoke(currentDay);
            OnNewDayStart?.Invoke();
        }
    }
    
    public void SkipToNextDayStart()
    {
        currentTime = currentTime.AddDays(1).Date.AddHours(startHour); 
        
        currentDay = (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
        OnDayChange?.Invoke(currentDay);
        OnNewDayStart?.Invoke(); 
        
        Debug.Log($"Skipped to the start of Day {currentDay} ({startHour}:00).");
    }

    public float GetCurrentHour()
    {
        return (float)currentTime.Hour + (float)currentTime.Minute / 60f + (float)currentTime.Second / 3600f;
    }

    public float GetTimeNormalized()
    {
        return (float)currentTime.TimeOfDay.TotalSeconds / 86400f;
    }

    public string GetFormattedTime()
    {
        return currentTime.ToString("HH:mm");
    }

    public int GetCurrentDay()
    {
        return (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
    }
}