using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance; 

    [Header("Time")]
    public float minWaitTime = 30f;
    public float maxWaitTime = 90f;
    public float minRainDuration = 5f;
    public float maxRainDuration = 12f;

    [Header("Effects")]
    public ParticleSystem globalRainVFX;
    public Image darkOverlay;

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
            // 1. Chờ đến đợt mưa tiếp theo
            float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Bắt đầu mưa đồng bộ
            yield return StartCoroutine(StartRainProcess());
        }
    }

    IEnumerator StartRainProcess()
{
    // 1. Giảm độ tối: Sử dụng 0.25f để màn hình bớt tối hơn
    float rainDarkness = 0.25f; 

    // Làm tối màn hình trước khi mưa
    if (darkOverlay != null) yield return StartCoroutine(FadeScreen(rainDarkness));

    // Bật mưa
    if (globalRainVFX != null) globalRainVFX.Play();

    // Phát tín hiệu mọc nấm
    OnRainStarted?.Invoke();

    // Duy trì cơn mưa
    float duration = UnityEngine.Random.Range(minRainDuration, maxRainDuration);
    yield return new WaitForSeconds(duration);

    // 2. KẾT THÚC ĐỒNG BỘ:
    // Gọi Stop() cho Rain và StartCoroutine cho FadeScreen cùng lúc (không dùng yield return cho Fade)
    if (globalRainVFX != null) globalRainVFX.Stop();
    OnRainStopped?.Invoke();

    // Chạy làm sáng màn hình song song với việc các hạt mưa cuối cùng đang rơi nốt
    yield return StartCoroutine(FadeScreen(0f)); 
}

    IEnumerator FadeScreen(float targetAlpha)
    {
        if (darkOverlay == null) yield break;
        float speed = 1f;
        Color color = darkOverlay.color;
        while (!Mathf.Approximately(color.a, targetAlpha))
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, speed * Time.deltaTime);
            darkOverlay.color = color;
            yield return null;
        }
    }
}