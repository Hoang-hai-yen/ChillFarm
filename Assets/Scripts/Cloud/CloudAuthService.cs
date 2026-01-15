using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using System;

public class CloudAuthService
{
    public string IdToken { get; private set; }
    public string LocalId { get; private set; }
    public string RefreshToken { get; private set; }
    public string ObbCode { get; private set; }
    public bool IsLogin { get; private set; }
    private ApiConfig apiConfig;

    // Token expiry tracking
    private DateTime tokenExpireTime;
    private const int TOKEN_REFRESH_BUFFER_SECONDS = 300; // Refresh 5 phút trước khi hết hạn

    // Event khi authentication hết hạn
    public event Action OnAuthenticationExpired;

    public CloudAuthService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
        IsLogin = false;
        tokenExpireTime = DateTime.MinValue;
        LoadAuthData();
    }

    /// <summary>
    /// Kiểm tra token đã hết hạn chưa
    /// </summary>
    public bool IsTokenExpired()
    {
        return DateTime.UtcNow >= tokenExpireTime;
    }

    /// <summary>
    /// Kiểm tra có nên refresh token không (trước khi hết hạn 5 phút)
    /// </summary>
    public bool ShouldRefreshToken()
    {
        if (!IsLogin) return false;
        return DateTime.UtcNow >= tokenExpireTime.AddSeconds(-TOKEN_REFRESH_BUFFER_SECONDS);
    }

    /// <summary>
    /// Force logout khi refresh token thất bại
    /// </summary>
    public void ForceLogout()
    {
        IdToken = null;
        RefreshToken = null;
        LocalId = null;
        IsLogin = false;
        tokenExpireTime = DateTime.MinValue;
        Debug.Log("[Auth] Session expired. User logged out.");
        OnAuthenticationExpired?.Invoke();
    }

    private void SaveAuthData()
    {
        PlayerPrefs.SetString("IdToken", IdToken);
        PlayerPrefs.SetString("RefreshToken", RefreshToken);
        PlayerPrefs.SetString("LocalId", LocalId);
        PlayerPrefs.Save();
    }

    private void LoadAuthData()
    {
        IdToken = PlayerPrefs.GetString("IdToken", null);
        RefreshToken = PlayerPrefs.GetString("RefreshToken", null);
        LocalId = PlayerPrefs.GetString("LocalId", null);
        IsLogin = !string.IsNullOrEmpty(IdToken) && !string.IsNullOrEmpty(RefreshToken) && !string.IsNullOrEmpty(LocalId);
    }


    public IEnumerator Register(string email, string password, string name, Action<bool, string> callback)
    {
        string url = apiConfig.Register + apiConfig.ApiKey;
        string json = $@"{{
                            ""email"":""{email}"",
                            ""password"":""{password}"",
                            ""returnSecureToken"":true}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
       
        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = JObject.Parse(request.downloadHandler.text);
            IdToken = data["idToken"].ToString();
            LocalId = data["localId"].ToString();
            RefreshToken = data["refreshToken"].ToString();
            // Firebase token có thời hạn 3600 giây (1 giờ)
            tokenExpireTime = DateTime.UtcNow.AddSeconds(3600);
            SaveAuthData();
            yield return CloudManager.Instance.Database.CreateInitialData(LocalId, email, "player", "avatar_01", (success, message) => {
                if(success)
                {
                    IsLogin = true;
                    Debug.Log("Create initial data successful!");
                }
                else
                {
                    callback?.Invoke(false, message);

                }

            });
            // yield return GameDataManager.instance.TryLoadData();
            callback?.Invoke(true, "Account created successfully!");

        }
        else
        {
            callback?.Invoke(false, request.downloadHandler.text);
        }
    }
    
    
     public IEnumerator Login(string email, string password, Action<bool, string> callback)
    {
        string url = apiConfig.Login + apiConfig.ApiKey;
        string json = $@"{{
                            ""email"":""{email}"",
                            ""password"":""{password}"",
                            ""returnSecureToken"":true}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = JObject.Parse(request.downloadHandler.text);
            IdToken = data["idToken"].ToString();
            LocalId = data["localId"].ToString();
            RefreshToken = data["refreshToken"].ToString();
            // Firebase token có thời hạn 3600 giây (1 giờ)
            tokenExpireTime = DateTime.UtcNow.AddSeconds(3600);
            IsLogin = true;
            SaveAuthData();
            callback?.Invoke(true, "Login successful!");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }

    public IEnumerator SendPassResetEmail(string email, Action<bool, string> callback)
    {
        string url = apiConfig.SendPassResetEmail + apiConfig.ApiKey;
        string json = $@"{{
                            ""requestType"":""PASSWORD_RESET"",
                            ""email"":""{email}""
                            }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Conten-Type", "aplication/json");

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true, "Send password reset email successful");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }

    }

    public IEnumerator VerifyPassResetCode(string obbCode, Action<bool, string> callback)
    {
        string url = apiConfig.VerifyPassResetCode + apiConfig.ApiKey;
        string json = $@"{{
                            ""obbCode"":""{obbCode}""
                            }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            ObbCode = obbCode;
            callback?.Invoke(true, "Verify password reset code successful");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }

    public IEnumerator ConfirmPassReset(string newPassword, Action<bool, string> callback)
    {
        string url = apiConfig.ConfirmPassReset + apiConfig.ApiKey;
        string json = $@"{{
                            ""obbCode"":""{ObbCode}"",
                            ""newPassword"":""{newPassword}""
                            }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true, "Confirm password reset successful");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }

    public IEnumerator RefreshIdToken(Action<bool, string> callback)
    {
        string url = apiConfig.Refresh + apiConfig.ApiKey; ;

        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", RefreshToken); 

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = JObject.Parse(request.downloadHandler.text);

            IdToken = data["id_token"].ToString();
            RefreshToken = data["refresh_token"].ToString();
            LocalId = data["user_id"].ToString();
            // Cập nhật thời gian hết hạn từ response
            tokenExpireTime = DateTime.UtcNow.AddSeconds(int.Parse(data["expires_in"].ToString()));
            Debug.Log("[Auth] Token refreshed successfully. Expires at: " + tokenExpireTime.ToLocalTime());

            callback?.Invoke(true, "Token refreshed successfully!");
        }
        else
        {
            callback?.Invoke(false, request.downloadHandler.text);
        }
    }

    
}