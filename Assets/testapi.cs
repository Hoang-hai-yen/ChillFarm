using UnityEngine;

public class testapi : MonoBehaviour
{
    public string email = "test1234@gmail.com";
    public string password = "fdffdfdfdfdd";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CloudManager.Instance.Auth.Login(email, password, (success, message) =>
        {
            Debug.Log(message);
            StartCoroutine(CloudManager.Instance.Database.GetData(CloudManager.Instance.Auth.LocalId, (success, message, gameData) =>
            {
                PlayerProfile p = (PlayerProfile)gameData["playerProfiles"];
                PlayerData pd = (PlayerData)gameData["playerData"];
                Farmland fd = (Farmland)gameData["farmlandData"];
                AnimalFarm ad = (AnimalFarm)gameData["animalFarmData"];


                Debug.Log(message);
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
