using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyNavigator : MonoBehaviour
{
    public void OnSignInClick()
    {
        Debug.Log("Button Clicked!"); // Kiểm tra xem hàm có được gọi không
        SceneManager.LoadScene("SignIn");  // Tên Scene bạn muốn chuyển
    }

    public void OnLogInClick()
    {
        Debug.Log("Button Clicked!"); // Kiểm tra xem hàm có được gọi không
        SceneManager.LoadScene("Lobby");  // Tên Scene bạn muốn chuyển
    }

    public void OnForgotPasswordClick()
    {
        Debug.Log("Button Clicked!"); // Kiểm tra xem hàm có được gọi không
        SceneManager.LoadScene("ForgotPassword");  // Tên Scene bạn muốn chuyển
    }
}
