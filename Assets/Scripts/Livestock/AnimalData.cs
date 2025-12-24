using UnityEngine;

[CreateAssetMenu(fileName = "New AnimalData", menuName = "Farming/Animal Data")]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public AnimalType type;
    public AnimalTier tier;

    [Header("Growth Settings")]
    public int daysToGrow = 5; 
    
    [Header("Visuals")]
    public Sprite babySprite;
    public Sprite adultSprite;
    public RuntimeAnimatorController animatorController; 

    [Header("Production")]
    public GameObject productPrefab; 
    public int minAffectionToProduce = 30; 

    [Header("Breeding")]
    [Range(0, 100)] public float breedingChance = 20f; 
}