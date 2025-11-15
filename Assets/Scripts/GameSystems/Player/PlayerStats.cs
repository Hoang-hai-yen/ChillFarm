using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int gold;
    public int XP;
    public int level = 1;
    public int XPToNextLevel = 100;

    public void GainXP(int amount)
    {
        XP += amount;
        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        XPToNextLevel = Mathf.RoundToInt(XPToNextLevel * 1.25f);
    }
}
