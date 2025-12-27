using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeBuyImageButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Diamonds (set in order left → right)")]
    [SerializeField] private GameObject[] diamonds;

    [Header("Max upgrade level")]
    [SerializeField] private int maxLevel = 5;

    private int currentLevel = 0;

    void Start()
    {
        // Đảm bảo tất cả kim cương đều tắt lúc đầu
        for (int i = 0; i < diamonds.Length; i++)
        {
            if (diamonds[i] != null)
                diamonds[i].SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuyUpgrade();
        AudioManager.Instance.PlayUpgrade();
    }

    private void BuyUpgrade()
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Đã nâng cấp tối đa");
            return;
        }

        if (currentLevel < diamonds.Length)
        {
            diamonds[currentLevel].SetActive(true);
        }

        currentLevel++;

        Debug.Log("Upgrade level: " + currentLevel);
    }
}
