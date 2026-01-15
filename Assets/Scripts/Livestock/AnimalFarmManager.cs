
using System.Collections.Generic;
using UnityEngine;

public class AnimalFarmManager : MonoBehaviour, IDataPersistence
{
    public static AnimalFarmManager Instance;

    public List<AnimalPen> animalPens = new List<AnimalPen>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadData(GameData data)
    {
        foreach(var pen in data.AnimalFarmData.Pens)
        {
            var animalPen = animalPens.Find(p => p.Id == pen.Id);
            animalPen.LoadFromSchema(pen);
        }
    }

    public void SaveData(GameData data)
    {
        data.AnimalFarmData.Pens = new List<Assets.Scripts.Cloud.Schemas.Pen>();
        foreach(var pen in animalPens)
        {
            data.AnimalFarmData.Pens.Add(pen.SaveToSchema());
        }
    }

    public List<FarmAnimal> LoadAnimalsInPen(List<Assets.Scripts.Cloud.Schemas.Animal> animalSchemas, AnimalPen pen)
    {
        List<FarmAnimal> loadedAnimals = new List<FarmAnimal>();
        
        foreach(var animalSchema in animalSchemas)
        {
            string liveStockItemId = (GameDataManager.instance.gameSODatabase.GetItemById(animalSchema.AnimalDataId) as AnimalData).liveStockItemId;
            GameObject animalPrefab = (GameDataManager.instance.gameSODatabase.GetItemById(liveStockItemId) as LivestockItemData).animalPrefab;
            GameObject animalObj = Instantiate(animalPrefab, pen.GetBounds().bounds.center, Quaternion.identity);
            FarmAnimal animalInstance = animalObj.GetComponent<FarmAnimal>();
            animalInstance.SetHome(pen.GetBounds());
            animalInstance.LoadFromSchema(animalSchema);
            loadedAnimals.Add(animalInstance);
        }
        return loadedAnimals;
    }
}