using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

public class FirebaseAuthService 
{
    public string IdToken { get; private set; }
    public string LocalId { get; private set; }
    public string RefreshToken { get; private set; }
    

    public IEnumerator CreateAccount(string email, string password, System.Action<bool, string> callback)
    {
        string url = ApiConfig.Instance.Register + ApiConfig.Instance.ApiKey;
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = JObject.Parse(request.downloadHandler.text);
            IdToken = data["idToken"].ToString();
            LocalId = data["localId"].ToString();
            RefreshToken = data["refreshToken"].ToString();
            callback?.Invoke(true, "Account created successfully!");
        }
        else
        {
            callback?.Invoke(false, request.downloadHandler.text);
        }
    }
    
    
     public IEnumerator Login(string email, string password, System.Action<bool, string> callback)
    {
        string url = ApiConfig.Instance.Login + ApiConfig.Instance.ApiKey;
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = JObject.Parse(request.downloadHandler.text);
            IdToken = data["idToken"].ToString();
            LocalId = data["localId"].ToString();
            RefreshToken = data["refreshToken"].ToString();
            callback?.Invoke(true, "Login successful!");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }

    public IEnumerator RefreshIdToken(System.Action<bool, string> callback)
    {
        string url = ApiConfig.Instance.Refresh + ApiConfig.Instance.ApiKey; ;

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

    public IEnumerator SendPassResetEmail(string email, System.Action<bool, string> callback)
    {
        string url = ApiConfig.Instance.SendOobCode + ApiConfig.Instance.ApiKey;
        string json = $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{email}\"}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true, "Password reset email sent successfully!");
        }
        else
        {
            callback?.Invoke(false, request.downloadHandler.text);
        }
    }
}