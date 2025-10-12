using UnityEngine;
using TMPro;

public class ResetPasswordManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_Text messageText;

    public void OnResetPasswordButtonClicked()
    {
        string email = emailInput.text;

        if (string.IsNullOrEmpty(email))
        {
            messageText.text = "Vui lòng nhập Email!";
            return;
        }

        // Gửi yêu cầu reset password
        StartCoroutine(FirebaseManager.Instance.Auth.SendPassResetEmail(email, (success, message) =>
        {
            if (success)
            {
                messageText.text = "Email khôi phục đã được gửi! Vui lòng kiểm tra hộp thư.";
            }
            else
            {
                messageText.text = "Gửi thất bại: " + message;
            }

            Debug.Log("Reset Password: " + success + " | " + message);
        }));
    }
}
