using System.Collections;
using UnityEngine;
using System;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance; 

    [Header("References")]
    public LightingController lightingController; // Thay darkOverlay bằng cái này
    public ParticleSystem globalRainVFX;

    [Header("Settings")]
    public float minWaitTime = 30f;
    public float maxWaitTime = 90f;
    public float minRainDuration = 5f;
    public float maxRainDuration = 12f;
    
    [Range(0f, 1f)]
    public float rainBrightnessMultiplier = 0.6f; // Khi mưa ánh sáng còn 60%
    public float fadeSpeed = 0.5f;

    public static event Action OnRainStarted;
    public static event Action OnRainStopped;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        StartCoroutine(GlobalWeatherCycle());
    }

    IEnumerator GlobalWeatherCycle()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(StartRainProcess());
        }
    }

    IEnumerator StartRainProcess()
    {
        // 1. Làm tối bằng LightingController
        if (lightingController != null)
        {
            lightingController.FadeWeatherIntensity(rainBrightnessMultiplier, fadeSpeed);
        }

        if (globalRainVFX != null) globalRainVFX.Play();
        OnRainStarted?.Invoke();

        float duration = UnityEngine.Random.Range(minRainDuration, maxRainDuration);
        yield return new WaitForSeconds(duration);

        // 2. Tắt mưa và làm sáng lại đồng bộ
         if (lightingController != null)
        {
            lightingController.FadeWeatherIntensity(1f, fadeSpeed);
        }
        
        if (globalRainVFX != null) globalRainVFX.Stop();
        OnRainStopped?.Invoke();

       
        
        // Chờ màn hình sáng hẳn mới kết thúc chu kỳ
        yield return new WaitForSeconds(1f / fadeSpeed); 
    }
}