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

    public CloudAuthService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
        IsLogin = false;
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
            yield return GameDataManager.instance.TryLoadData();
            callback?.Invoke(true, "Account created successfully!");

        }
        else
        {
            callback?.Invoke(false, request.error);
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
            IsLogin = true;
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

            callback?.Invoke(true, "Token refreshed successfully!");
        }
        else
        {
            callback?.Invoke(false, request.downloadHandler.text);
        }
    }

}