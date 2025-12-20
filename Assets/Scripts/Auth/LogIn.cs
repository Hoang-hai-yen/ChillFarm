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

    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Vui lòng nhập Email và Mật khẩu!";
            return;
        }

        // Gọi đăng nhập Firebase
        StartCoroutine(CloudManager.Instance.Auth.Login(email, password, (success, message) =>
        {
            if (success)
            {
                messageText.text = "Đăng nhập thành công! Đang vào trang trại...";
                StartCoroutine(DelayToScene("Test", 3f)); // Delay 2 giây
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
        // yield return GameDataManager.instance.TryLoadData();
        SceneManager.LoadScene(sceneName);
    }
}
