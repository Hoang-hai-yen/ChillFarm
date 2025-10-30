using UnityEngine;

public class testapi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CloudManager.Instance.Auth.Login("test1234@gmail.com", "fdffdfdfdfdd", (success, message) =>
        {
            Debug.Log("Login: " + success + " | " + message);
            StartCoroutine(CloudManager.Instance.Auth.RefreshIdToken((success, message) =>
            {
                Debug.Log("Refresh: " + success + " | " + message);
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
