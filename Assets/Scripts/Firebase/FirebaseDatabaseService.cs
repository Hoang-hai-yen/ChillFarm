using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using System;

public class FirebaseDatabaseService
{
    private ApiConfig apiConfig;

    public FirebaseDatabaseService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
    }

    public IEnumerator CreateDocument(string collectionId, string documentId, JObject fields, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";
        if(!String.IsNullOrWhiteSpace(documentId))
        {
            url += $"?documentId={documentId}";
        }

        JObject body = new JObject
        {
            ["fields"] = fields
        };

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(body.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + FirebaseManager.Instance.Auth.IdToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true, "Create document successful");
        }
        else
        {
            callback?.Invoke(false, request.error);
        }
    }
}