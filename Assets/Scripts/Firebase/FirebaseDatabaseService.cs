using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq; 
using System;

public class FirebaseDatabaseService
{
    private ApiConfig apiConfig;

    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string Scene { get; set; }

        public Position(float x, float y, string scene)
        {
            X = x;
            Y = y;
            Scene = scene;
        }
    }

    public class Time 
    {
        public string CurrentTime { get; set; }
        public int Day { get; set; }

        public Time(string currentTime, int day)
        {
            CurrentTime = currentTime;
            Day = day;
        }
    }

    public FirebaseDatabaseService(ApiConfig apiConfig)
    {
        this.apiConfig = apiConfig;
    }

    public IEnumerator CreateDocument(string collectionId, string documentId, JObject fields, Action<bool, string> callback)
    {
        string url = apiConfig.Database + $"projects/{apiConfig.ProjectId}/databases/(default)/documents/{collectionId}";
        if (!String.IsNullOrWhiteSpace(documentId))
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

    public static JObject CreatePlayerObject(string name, int exp, int gold, int stamina)
    {
        return new JObject
        {
            ["name"] = new JObject { ["stringValue"] = name },
            ["exp"] = new JObject { ["integerValue"] = $"{exp}" },
            ["gold"] = new JObject { ["integerValue"] = $"{gold}" },
            ["stamina"] = new JObject { ["integerValue"] = $"{stamina}" },
        };
    }
    
    public static JObject CreatePlayerDataObject(string playerId, Time time , Position position)
    {
        return new JObject
        {
            ["player_id"] = new JObject { ["stringValue"] = playerId },
            ["time"] = new JObject
            {
                ["mapValue"] = new JObject
                {
                    ["fields"] = new JObject
                    {
                        ["current_time"] = new JObject { ["stringValue"] = time.CurrentTime },
                        ["day"] = new JObject { ["integerValue"] = time.Day }
                    }
                }
            },
            ["position"] = new JObject
            {
                ["mapValue"] = new JObject
                {
                    ["fields"] = new JObject
                    {
                        ["position_x"] = new JObject { ["doubleValue"] = position.X },
                        ["position_y"] = new JObject { ["doubleValue"] = position.Y },
                        ["scene"] = new JObject { ["stringValue"] = position.Scene }
                    }
                }
            },
        };
    }
}