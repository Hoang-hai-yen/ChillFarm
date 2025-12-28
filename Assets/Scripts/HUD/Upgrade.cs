using UnityEngine;
using UnityEngine.EventSystems;

public class Upgrade : MonoBehaviour, IPointerClickHandler
{
    [Header("Diamonds (left → right)")]
    [SerializeField] private GameObject[] diamonds;

    [Header("Max upgrade level")]
    [SerializeField] private int maxLevel = 5;

    private int currentLevel = 0;

    void Start()
    {
        // Auto fix: tắt toàn bộ diamond khi bắt đầu
        for (int i = 0; i < diamonds.Length; i++)
        {
            if (diamonds[i] != null)
            {
                diamonds[i].SetActive(false);
            }
            else
            {
                Debug.LogWarning("Diamond element " + i + " đang NULL");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuyUpgrade();
    }

    private void BuyUpgrade()
    {
        // Đã max
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Đã nâng cấp tối đa");
            return;
        }

        // Mảng thiếu phần tử
        if (currentLevel >= diamonds.Length)
        {
            Debug.LogError("Diamonds array KHÔNG đủ phần tử");
            return;
        }

        // Element bị null
        if (diamonds[currentLevel] == null)
        {
            Debug.LogError("Diamond element " + currentLevel + " bị NULL");
            return;
        }

        // Hiện diamond
        diamonds[currentLevel].SetActive(true);
        currentLevel++;

        Debug.Log("Upgrade level: " + currentLevel);
    }
}
