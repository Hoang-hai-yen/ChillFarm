using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class FaintUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup blackScreenGroup; 
    public TextMeshProUGUI messageText; 

    [Header("Settings")]
    public float fadeDuration = 1.5f;
    public float textFadeDuration = 1.5f;
    public float displayDuration = 2.0f;

    private void Awake()
    {
        if (blackScreenGroup != null) blackScreenGroup.gameObject.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    public IEnumerator PlayFaintSequence(Action onDayResetAction)
    {
        if (blackScreenGroup != null)
        {
            blackScreenGroup.gameObject.SetActive(true);
            blackScreenGroup.alpha = 0f; 
            blackScreenGroup.blocksRaycasts = true;
        }

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Mình lại làm việc quá sức rồi...";
            messageText.alpha = 0f;
        }

        yield return StartCoroutine(FadeCanvasGroup(blackScreenGroup, 0f, 1f, fadeDuration));

        if (messageText != null)
        {
            yield return StartCoroutine(FadeTextAlpha(messageText, 0f, 1f, textFadeDuration));
        }

        yield return new WaitForSeconds(displayDuration);

        onDayResetAction?.Invoke();

        if (messageText != null) messageText.alpha = 0f;

        yield return StartCoroutine(FadeCanvasGroup(blackScreenGroup, 1f, 0f, fadeDuration));

        if (blackScreenGroup != null)
        {
            blackScreenGroup.blocksRaycasts = false;
            blackScreenGroup.gameObject.SetActive(false); 
        }
        
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false); 
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        if (cg == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI txt, float start, float end, float duration)
    {
        if (txt == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            txt.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        txt.alpha = end;
    }
}