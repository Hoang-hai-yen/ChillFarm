using UnityEngine;
using Newtonsoft.Json.Linq;

public class testapi : MonoBehaviour
{
    public string email = "test1211ffffffftt1@gmail.com";
    public string password = "ffffffffffffffff";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FirebaseManager.Instance.Auth.CreateAccount(email, password, (success, message) =>
        {
            Debug.Log("Create acc: " + success + " | " + message);

            JObject fields = new JObject
            {
                ["exp"] = new JObject { ["integerValue"] = "20" },
                ["gold"] = new JObject { ["integerValue"] = "50" },
            };
            StartCoroutine(FirebaseManager.Instance.Auth.SendPassResetEmail(email,(success, message) =>
            {
                Debug.Log("Create doc: " + success + " | " + message);
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
