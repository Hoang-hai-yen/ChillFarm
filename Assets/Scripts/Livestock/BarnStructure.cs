using UnityEngine;
using System.Collections.Generic;

public class BarnStructure : MonoBehaviour
{
    [Header("Info")]
    public string structureName = "Chuồng Gà";

    [Header("Upgrade Data")]
    public List<BarnLevelData> levels; 
    public int currentLevelIndex = 0; 
    
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateStructureVisual();
    }

    public bool TryUpgrade()
    {
        if (currentLevelIndex >= levels.Count - 1)
        {
            BarnLevelData currentData = levels[currentLevelIndex];
            
            string maxMsg = $"<b>{structureName}</b>\n" +
                            $"<color=red>Bạn đã đạt cấp độ tối đa!</color>\n" +
                            $"- Cấp độ: {currentData.levelIndex}\n" +
                            $"- Sức chứa: {currentData.maxCapacity} con";

            ConfirmationUI.Instance.ShowNotification(maxMsg);
            
            return false;
        }

        int nextLevelIndex = currentLevelIndex + 1;
        BarnLevelData currentLvl = levels[currentLevelIndex];
        BarnLevelData nextLvl = levels[nextLevelIndex];
        int cost = nextLvl.upgradeCost;
        
        string msg = $"<b>NÂNG CẤP {structureName.ToUpper()}?</b>\n\n" +
                     $"- Cấp độ hiện tại: {currentLvl.levelIndex}\n" +
                     $"- Số lượng chứa hiện tại: {currentLvl.maxCapacity}\n" +
                     $"- Sức chứa sau khi tăng: <color=blue><b>{nextLvl.maxCapacity}</b></color>\n\n" +
                     $"Bạn có muốn nâng cấp không?";

        ConfirmationUI.Instance.ShowQuestion(msg, cost, () => 
        {
            if (InventoryManager.Instance.TrySpendGold(cost))
            {
                currentLevelIndex = nextLevelIndex;
                UpdateStructureVisual();
                Debug.Log("Nâng cấp thành công!");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
            }
        });

        return true;
    }

    public int GetCurrentCapacity()
    {
        if (levels != null && levels.Count > currentLevelIndex) 
            return levels[currentLevelIndex].maxCapacity;
        return 2;
    }

    private void UpdateStructureVisual()
    {
        if (levels != null && levels.Count > currentLevelIndex && sr != null) 
            sr.sprite = levels[currentLevelIndex].houseSprite;
    }
}