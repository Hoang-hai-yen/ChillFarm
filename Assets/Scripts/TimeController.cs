using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Độ dài một ngày (24 giờ) trong game, tính bằng giây thực.")]
    public float dayDurationInSeconds = 120f; 
    
    [Tooltip("Giờ bắt đầu của game (0-24).")]
    [Range(0, 24)]
    public float startHour = 6f; 

    public event Action<int> OnDayChange;

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

        if (currentTime.Hour == 0 && currentTime.Minute < 1)
        {
            if (currentDay != (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1)
            {
                currentDay = (currentTime.DayOfYear - DateTime.Today.DayOfYear) + 1;
                // Kích hoạt sự kiện ngày mới
                OnDayChange?.Invoke(currentDay);
            }
        }
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
        return currentDay;
    }
}