using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Cloud.Schemas;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private Button backgroundButton; 
    [SerializeField] private Button skipButton;       
    [SerializeField] private List<GameObject> tutorialPages;

    [Header("UI Animation")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip tutorialBgm;
    [SerializeField] private AudioClip clickClip;     
    private AudioSource audioSource;

    [Header("Shop Quest Tutorial")]
    [SerializeField] private QuestData buyToolQuest; 
    [SerializeField] private GameObject dialogPanel; 
    [SerializeField] private TMP_Text dialogText;    
    [SerializeField] private GameObject guideArrow;  
    [SerializeField] private Transform shopLocation; 
    [SerializeField] private GameObject toolUsagePanel; 
    [SerializeField] private Button closeUsagePanelBtn; 

    [Header("Dialog Settings")]
    [SerializeField] private AudioClip typingClip; 
    [SerializeField] private float typingSpeed = 0.05f; 

    private QuestUIController questUIController;
    private TimeController timeController;
    private PlayerController playerController;
    
    private int currentPageIndex = 0;
    private bool isTransitioning = false; 
    private bool isWaitingForShopQuest = false;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        timeController = FindFirstObjectByType<TimeController>();
        questUIController = FindFirstObjectByType<QuestUIController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (GameDataManager.instance != null)
        {
            if (GameDataManager.instance.IsDataLoaded) CheckAndStartTutorial();
            else GameDataManager.instance.OnDataLoaded += CheckAndStartTutorial;
        }

        if (backgroundButton != null) backgroundButton.onClick.AddListener(OnScreenClick);
        if (skipButton != null) skipButton.onClick.AddListener(OnSkipClick);
        if (closeUsagePanelBtn != null) closeUsagePanelBtn.onClick.AddListener(CloseUsagePanel);

        if (QuestController.Instance != null)
        {
            QuestController.Instance.OnQuestCompleted += OnQuestCompletedHandler;
        }
    }

    void OnDestroy()
    {
        if (GameDataManager.instance != null)
            GameDataManager.instance.OnDataLoaded -= CheckAndStartTutorial;
            
        if (QuestController.Instance != null)
            QuestController.Instance.OnQuestCompleted -= OnQuestCompletedHandler;
    }

    void Update()
    {
        if (isWaitingForShopQuest)
        {
            UpdateGuideArrow();
        }
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
            
            if (AudioManager.Instance != null) 
            {
                AudioManager.Instance.PlayGameBGM(); 
            }
        }
    }

    private void StartTutorialSequence()
    {
        if(tutorialCanvas != null) tutorialCanvas.SetActive(true);
        if (playerController != null) playerController.enabled = false;
        if (timeController != null) timeController.PauseTime();        

        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
        
        if (audioSource != null && tutorialBgm != null)
        {
            audioSource.clip = tutorialBgm;
            audioSource.loop = true;
            audioSource.Play();
        }

        foreach (var page in tutorialPages) if (page != null) page.SetActive(false);
        currentPageIndex = 0;
        StartCoroutine(FadePageRoutine(null, tutorialPages[currentPageIndex]));
    }

    private void OnScreenClick()
    {
        if (isTransitioning) return;

        if (clickClip != null && audioSource != null) audioSource.PlayOneShot(clickClip);

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
        if(tutorialCanvas != null) tutorialCanvas.SetActive(false);
        
        if (audioSource != null) audioSource.Stop();

        if (playerController != null) playerController.enabled = true;
        if (timeController != null) timeController.ResumeTime();

        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayGameBGM();
        }

        StartCoroutine(StartShopQuestSequence());
    }

    // --- LOGIC QUEST ĐI CHỢ ---
    private IEnumerator StartShopQuestSequence()
    {
        if (dialogPanel != null && dialogText != null)
        {
            dialogPanel.SetActive(true);
            dialogText.text = "Chà, mình cần mua vài dụng cụ làm nông trước đã. Ghé chợ xem sao!";
            yield return new WaitForSeconds(3f); 
            dialogPanel.SetActive(false);
        }

        if (buyToolQuest != null && QuestController.Instance != null)
        {
            QuestController.Instance.AcceptQuest(buyToolQuest);
            Debug.Log("Tutorial: Đã nhận quest mua dụng cụ.");
        }

        if (guideArrow != null)
        {
            guideArrow.SetActive(true);
            isWaitingForShopQuest = true;
        }
    }

    private void UpdateGuideArrow()
    {
        if (guideArrow == null || shopLocation == null || playerController == null) return;

        Vector3 dir = shopLocation.position - playerController.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        guideArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        float distance = Vector3.Distance(playerController.transform.position, shopLocation.position);
        
        guideArrow.SetActive(distance > 3f);
    }

    private void OnQuestCompletedHandler(string questId)
    {
        if (buyToolQuest != null && questId == buyToolQuest.questId)
        {
            isWaitingForShopQuest = false;
            if (guideArrow != null) guideArrow.SetActive(false);

            StartCoroutine(ShowQuestCompletionRoutine(questId));
        }
    }

    private IEnumerator ShowQuestCompletionRoutine(string questId)
    {
        // --- BƯỚC 1: HIỆN QUEST LOG ---
        if (questUIController != null)
        {
            questUIController.gameObject.SetActive(true); 
            QuestController.Instance.CheckInventoryForQuests(); 
        }

        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(1.5f);

        if (questUIController != null) questUIController.gameObject.SetActive(false);

        QuestController.Instance.FinishQuestWithoutRemovingItems(questId);

        if (toolUsagePanel != null)
        {
            toolUsagePanel.SetActive(true);
            
            yield return new WaitUntil(() => !toolUsagePanel.activeSelf);
        }

        if (dialogPanel != null && dialogText != null)
        {
            dialogPanel.SetActive(true);

            yield return StartCoroutine(TypeContent("Đủ đồ dùng rồi, bắt đầu công việc thôi!"));
            yield return new WaitForSecondsRealtime(1.5f);

            yield return StartCoroutine(TypeContent("Mấy đồ còn thiếu mình có thể mua ở cửa hàng khác trong chợ."));
            yield return new WaitForSecondsRealtime(2f);

            yield return StartCoroutine(TypeContent("Nghe nói còn có cửa hàng để bán vật phẩm nữa! Nghe bảo giá rất hời đấy!"));
            yield return new WaitForSecondsRealtime(2.5f);

            dialogPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        
        SaveTutorialComplete();
    }
    private void CloseUsagePanel()
    {
        if (toolUsagePanel != null) 
        {
            toolUsagePanel.SetActive(false);
        }
        
    }

    private void SaveTutorialComplete()
    {
        var playerData = GameDataManager.instance.GetGameData().PlayerDataData;
        if (playerData != null)
        {
            playerData.IsFirstLogin = false;
            StartCoroutine(GameDataManager.instance.ForceSaveAll());
        }
    }

    private IEnumerator FadePageRoutine(GameObject pageOut, GameObject pageIn)
    {
        isTransitioning = true;

        if (pageOut != null)
        {
            CanvasGroup cgOut = GetCanvasGroup(pageOut);
            cgOut.blocksRaycasts = false; 
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

        if (pageIn != null)
        {
            pageIn.SetActive(true);
            CanvasGroup cgIn = GetCanvasGroup(pageIn);
            cgIn.alpha = 0; 
            
            float time = 0;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                cgIn.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
                yield return null;
            }
            cgIn.alpha = 1;
            cgIn.blocksRaycasts = true; 
        }

        isTransitioning = false;
    }

    private CanvasGroup GetCanvasGroup(GameObject obj)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }
    private IEnumerator TypeContent(string content)
    {
        dialogText.text = ""; 
        
        foreach (char letter in content.ToCharArray())
        {
            dialogText.text += letter;
            
            if (audioSource != null && typingClip != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(typingClip);
            }

            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}