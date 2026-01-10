using UnityEngine;

[CreateAssetMenu(fileName = "New Barn Level", menuName = "Farming/Barn Level Data")]
public class BarnLevelData : ScriptableObject
{
    public int levelIndex;          
    public string levelName;        
    public int upgradeCost;         
    public int maxCapacity;        
    public Sprite houseSprite;     
}