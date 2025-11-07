using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RegisterManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text messageText;

    public void OnRegisterButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email hoặc Password trống!");
            messageText.text = "Vui lòng nhập Email và Mật khẩu!";
            return;
        }
         
        StartCoroutine(CloudManager.Instance.Auth.Register(email, password, "Player", (success, message) =>
        {
            if (success)
            {
                Debug.Log("Đăng ký thành công!");
                messageText.text = "Đăng ký thành công! Chuyển về đăng nhập...";
                StartCoroutine(DelayToScene("Lobby", 2f));
            }
            else
            {
                Debug.LogError("Đăng ký thất bại: " + message);
                messageText.text = "Đăng ký thất bại: " + message;
            }

            Debug.Log("Create acc: " + success + " | " + message);
        }));
    }

    private IEnumerator DelayToScene(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }
}
