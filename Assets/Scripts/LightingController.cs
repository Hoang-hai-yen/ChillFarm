using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class LightingController : MonoBehaviour
{
    [Header("References")]
    public TimeController timeController;
    public Light2D globalLight; 

    [Header("Lighting Gradients")]
    [Tooltip("Độ sáng toàn cảnh theo thời gian (0.0=0h, 0.5=12h, 1.0=24h)")]
    public Gradient lightColor;
    
    [Tooltip("Cường độ ánh sáng toàn cảnh theo thời gian (0.0=0h, 0.5=12h, 1.0=24h)")]
    public AnimationCurve lightIntensity; 

    void Update()
    {
        if (timeController == null || globalLight == null) return;

        float normalizedTime = timeController.GetTimeNormalized();

        UpdateLighting(normalizedTime);
    }

    private void UpdateLighting(float normalizedTime)
    {
        globalLight.color = lightColor.Evaluate(normalizedTime);

        globalLight.intensity = lightIntensity.Evaluate(normalizedTime);

    }
}