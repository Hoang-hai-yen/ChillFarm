using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Cần thiết cho Coroutine
using System.Collections.Generic;
using Assets.Scripts.Cloud.Schemas;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private Button backgroundButton; 
    [SerializeField] private Button skipButton;       
    [SerializeField] private List<GameObject> tutorialPages;

    [Header("UI Animation")]
    [SerializeField] private float fadeDuration = 0.5f; // Thời gian fade

    [Header("Audio")]
    [SerializeField] private AudioClip tutorialBgm;   // Nhạc nền riêng cho Tutorial
    [SerializeField] private AudioClip clickClip;     
    private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private string firstQuestId = "quest_01_welcome";

    private int currentPageIndex = 0;
    private PlayerController playerController;
    private bool isTransitioning = false; // Ngăn người dùng click liên tục khi đang fade

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (GameDataManager.instance != null)
        {
            if (GameDataManager.instance.IsDataLoaded)
                CheckAndStartTutorial();
            else
                GameDataManager.instance.OnDataLoaded += CheckAndStartTutorial;
        }

        if (backgroundButton != null)
        {
            backgroundButton.onClick.AddListener(OnScreenClick);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipClick);
        }
    }

    void OnDestroy()
    {
        if (GameDataManager.instance != null)
            GameDataManager.instance.OnDataLoaded -= CheckAndStartTutorial;
    }

    private void CheckAndStartTutorial()
    {
        var gameData = GameDataManager.instance.GetGameData();
        if (gameData == null || gameData.PlayerDataData == null) return;

        if (gameData.PlayerDataData.IsFirstLogin)
        {
            StartTutorialSequence();
        }
        else
        {
            if(tutorialCanvas != null) tutorialCanvas.SetActive(false);
            
            // Nếu không phải newbie, bật nhạc nền game ngay
            if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM();
        }
    }

    private void StartTutorialSequence()
    {
        if(tutorialCanvas != null) tutorialCanvas.SetActive(true);
        if (playerController != null) playerController.enabled = false;

        // --- XỬ LÝ ÂM THANH ---
        // 1. Tắt nhạc nền game chính
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();

        // 2. Bật nhạc tutorial (nếu có)
        if (audioSource != null && tutorialBgm != null)
        {
            audioSource.clip = tutorialBgm;
            audioSource.loop = true;
            audioSource.Play();
        }
        // ----------------------

        // Ẩn tất cả các trang trước khi bắt đầu
        foreach (var page in tutorialPages)
        {
            if (page != null) page.SetActive(false);
        }

        currentPageIndex = 0;
        // Bắt đầu hiện trang đầu tiên
        StartCoroutine(FadePageRoutine(null, tutorialPages[currentPageIndex]));
    }

    private void OnScreenClick()
    {
        // Nếu đang chuyển cảnh thì không nhận click
        if (isTransitioning) return;

        if (clickClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickClip);
        }

        int nextIndex = currentPageIndex + 1;

        if (nextIndex < tutorialPages.Count)
        {
            StartCoroutine(FadePageRoutine(tutorialPages[currentPageIndex], tutorialPages[nextIndex]));
            currentPageIndex = nextIndex;
        }
        else
        {
            EndTutorial();
        }
    }

    private void OnSkipClick()
    {
        if (isTransitioning) return;
        if (clickClip != null && audioSource != null) audioSource.PlayOneShot(clickClip);
        
        EndTutorial();
    }

    private void EndTutorial()
    {
        // Dừng nhạc tutorial
        if (audioSource != null) audioSource.Stop();

        if(tutorialCanvas != null) tutorialCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = true;

        var playerData = GameDataManager.instance.GetGameData().PlayerDataData;
        if (playerData != null)
        {
            playerData.IsFirstLogin = false;
            StartCoroutine(GameDataManager.instance.ForceSaveAll());
        }

        // --- BẬT LẠI NHẠC NỀN GAME ---
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM();

        Debug.Log("Tutorial Finished! Trigger Quest here.");
    }

    // --- LOGIC FADE IN/OUT ---
    private IEnumerator FadePageRoutine(GameObject pageOut, GameObject pageIn)
    {
        isTransitioning = true;

        // 1. Fade Out trang cũ (nếu có)
        if (pageOut != null)
        {
            CanvasGroup cgOut = GetCanvasGroup(pageOut);
            float time = 0;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                cgOut.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
                yield return null;
            }
            cgOut.alpha = 0;
            pageOut.SetActive(false);
        }

        // 2. Fade In trang mới (nếu có)
        if (pageIn != null)
        {
            pageIn.SetActive(true);
            CanvasGroup cgIn = GetCanvasGroup(pageIn);
            cgIn.alpha = 0; // Đảm bảo bắt đầu từ 0

            float time = 0;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                cgIn.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
                yield return null;
            }
            cgIn.alpha = 1;
        }

        isTransitioning = false;
    }

    // Helper: Lấy hoặc thêm CanvasGroup tự động để chỉnh Alpha
    private CanvasGroup GetCanvasGroup(GameObject obj)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = obj.AddComponent<CanvasGroup>();
        }
        return cg;
    }
}