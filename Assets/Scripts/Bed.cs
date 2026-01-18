using System.Collections;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private StaminaController staminaController;
    private FaintUIManager faintUIManager;
    private AudioSource audioSource;

    [Header("Audio")]
    public AudioClip yawnClip;

    private void Start()
    {
        staminaController = FindFirstObjectByType<StaminaController>();
        faintUIManager = FindFirstObjectByType<FaintUIManager>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public bool Sleep()
    {
        if (staminaController != null && faintUIManager != null)
        {
            StartCoroutine(SleepRoutine());
            return true;
        }
        
        if (staminaController != null)
        {
            staminaController.EndDayAndRecover();
            return true;
        }

        return false;
    }

    private IEnumerator SleepRoutine()
    {
        yield return faintUIManager.PlaySleepSequence(() => 
        {
            staminaController.EndDayAndRecover();
        });

        if (yawnClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(yawnClip);
        }
        
        Debug.Log("Đã thức dậy và ngáp một cái!");
    }
}