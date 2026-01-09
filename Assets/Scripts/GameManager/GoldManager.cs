using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;
    public int currentGold = 1000; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log("Đã tiêu: " + amount + " Gold. Còn lại: " + currentGold);
            return true;
        }
        return false;
    }
}