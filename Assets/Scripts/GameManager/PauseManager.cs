using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject pausePanel; 
    [SerializeField] private Button resumeButton;   
    [SerializeField] private Button quitButton;    

    [Header("Settings")]
    [Tooltip("Nếu đang mở Shop hoặc UI khác thì có cho phép mở Pause không?")]
    [SerializeField] private bool allowPauseOverUI = false;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

        if (resumeButton != null) 
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (quitButton != null) 
            quitButton.onClick.AddListener(OnQuitClick);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        
        Time.timeScale = 1f;
    }

    private void OnQuitClick()
    {
        Time.timeScale = 1f; 
        StartCoroutine(QuitSequence());
    }

    private IEnumerator QuitSequence()
    {
        if (quitButton != null) quitButton.interactable = false;

        if (GameDataManager.instance != null)
        {
            Debug.Log("Đang lưu dữ liệu trước khi thoát...");
            yield return StartCoroutine(GameDataManager.instance.ForceSaveAll());
        }

        yield return null;

        Debug.Log("Game đã lưu. Đang thoát...");

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false; 
        #else
            Application.Quit(); 
        #endif
    }
}