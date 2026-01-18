using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Nhớ dùng TextMeshPro
using System.Collections;

public class LoadingController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject loadingPanel; 
    [SerializeField] private TMP_Text statusText;     

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "Test"; 

    public void LoadGame()
    {
        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);

        Coroutine textEffect = StartCoroutine(AnimateTextRoutine());

        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            bool isDataReady = false;
            if (GameDataManager.instance != null)
            {
                isDataReady = GameDataManager.instance.IsDataLoaded;
            }

            if (operation.progress >= 0.9f && isDataReady)
            {
                StopCoroutine(textEffect);
                if (statusText != null) statusText.text = "Hoàn tất!";
                
                yield return new WaitForSeconds(0.5f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator AnimateTextRoutine()
    {
        string baseText = "Đang đồng bộ dữ liệu";
        
        while (true) 
        {
            if (statusText != null) statusText.text = baseText + "";
            yield return new WaitForSeconds(0.3f);

            if (statusText != null) statusText.text = baseText + ".";
            yield return new WaitForSeconds(0.3f);

            if (statusText != null) statusText.text = baseText + "..";
            yield return new WaitForSeconds(0.3f);

            if (statusText != null) statusText.text = baseText + "...";
            yield return new WaitForSeconds(0.3f);
        }
    }
}