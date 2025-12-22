using UnityEngine;

public class BreedingManager : MonoBehaviour
{
    // Singleton
    public static BreedingManager Instance;
    
    [Header("Config")]
    public GameObject cowPrefab;
    public GameObject chickenPrefab;

    // Các biến Data mẫu để gán khi sinh ra con mới (Cần assign trong Inspector)
    public AnimalData chickenNormal, chickenMedium, chickenHigh;
    public AnimalData cowNormal, cowMedium, cowHigh;

    void Awake() { Instance = this; }

    public void TryBreedAnimals(FarmAnimal[] animalsInPen)
    {
        FarmAnimal parent1 = null;
        FarmAnimal parent2 = null;

        foreach(var animal in animalsInPen)
        {
            if (animal.IsAdult())
            {
                if (parent1 == null) parent1 = animal;
                else if (parent2 == null && animal.GetAnimalType() == parent1.GetAnimalType())
                {
                    parent2 = animal;
                    break; 
                }
            }
        }

        if (parent1 != null && parent2 != null)
        {
            if (Random.value < 0.3f) 
            {
                SpawnBaby(parent1, parent2);
            }
        }
    }

    private void SpawnBaby(FarmAnimal p1, FarmAnimal p2)
    {
        AnimalTier babyTier = CalculateBabyTier(p1.GetTier(), p2.GetTier());
        AnimalType type = p1.GetAnimalType();

        AnimalData babyData = GetDataByTier(type, babyTier);
        GameObject prefabToSpawn = (type == AnimalType.Chicken) ? chickenPrefab : cowPrefab;

        GameObject babyObj = Instantiate(prefabToSpawn, p1.transform.position, Quaternion.identity);
        FarmAnimal babyScript = babyObj.GetComponent<FarmAnimal>();
        babyScript.data = babyData; 
        
        Debug.Log($"Một bé {type} loại {babyTier} đã chào đời!");
    }

    private AnimalTier CalculateBabyTier(AnimalTier t1, AnimalTier t2)
    {
        int score = (int)t1 + (int)t2; 
        
        float rand = Random.value;

        if (score == 0) return (rand < 0.9f) ? AnimalTier.Normal : AnimalTier.Medium;
        
        if (score >= 4) return (rand < 0.5f) ? AnimalTier.Medium : AnimalTier.High;

        if (rand < 0.5f) return AnimalTier.Normal;
        if (rand < 0.8f) return AnimalTier.Medium;
        return AnimalTier.High;
    }

    private AnimalData GetDataByTier(AnimalType type, AnimalTier tier)
    {
        if (type == AnimalType.Chicken)
        {
            if (tier == AnimalTier.High) return chickenHigh;
            if (tier == AnimalTier.Medium) return chickenMedium;
            return chickenNormal;
        }
        else
        {
            if (tier == AnimalTier.High) return cowHigh;
            if (tier == AnimalTier.Medium) return cowMedium;
            return cowNormal;
        }
    }
}