using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text messageText;
    [Header("Loading System")]
    [SerializeField] private LoadingController loadingScreen;

    void Start()
    {
        // if(CloudManager.Instance.Auth.IsLogin)
        // {
        //     messageText.text = "Đã đăng nhập! Đang vào trang trại...";
        //     StartCoroutine(DelayToScene("Test", 2f)); // Delay 2 giây
        // }
    }
    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Vui lòng nhập Email và Mật khẩu!";
            return;
        }

        StartCoroutine(CloudManager.Instance.Auth.Login(email, password, (success, message) =>
        {
            if (success)
            {
                messageText.text = "Đăng nhập thành công! Đang chuẩn bị dữ liệu...";

                if (GameDataManager.instance != null)
                {
                    GameDataManager.instance.LoadGame();
                }
                
                if (loadingScreen != null)
                {
                    loadingScreen.LoadGame(); 
                }
                else
                {
                    Debug.LogWarning("Chưa gắn LoadingScreen vào Inspector!");
                    StartCoroutine(DelayToScene("Test", 3f)); 
                }
            }
            else
            {
                messageText.text = "Đăng nhập thất bại: " + message;
            }

            Debug.Log("Login: " + success + " | " + message);
        }));
    }

    private IEnumerator DelayToScene(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }
}
