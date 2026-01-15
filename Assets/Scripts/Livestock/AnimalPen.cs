using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    [SerializeField] private bool isUnlocked = false;
    public bool IsUnlocked => isUnlocked;

    [Header("Cài đặt Chuồng")]
    public AnimalType allowedAnimal; 
    
    public BarnStructure linkedStructure;

    public DoorController doorController; 

    private Collider2D penBounds;

    private List<FarmAnimal> animalsInPen = new List<FarmAnimal>();

    void Awake()
    {
        penBounds = GetComponent<Collider2D>();
        if (penBounds == null) Debug.LogError("AnimalPen: Thiếu Collider2D!");
    }

    void Start()
    {
        
    }

    void OnValidate()
    {
        if(System.String.IsNullOrEmpty(id))
            id = "FarmAnimal_" + System.Guid.NewGuid().ToString();
    }

    public void MarkDataDirty()
    {
        GameDataManager.instance.MarkDirty(GameDataManager.DataType.animals);
    }

    public void LoadFromSchema(Assets.Scripts.Cloud.Schemas.Pen penSchema)
    {
        if(penSchema != null)
        {
            linkedStructure.currentLevelIndex = penSchema.CurrentLevel;
            isUnlocked = penSchema.IsUnlocked;
            if(doorController != null)
            {
                doorController.isLocked = !isUnlocked;
            }
            animalsInPen = AnimalFarmManager.Instance.LoadAnimalsInPen(penSchema.Animals, this);
        }
    }

    public Assets.Scripts.Cloud.Schemas.Pen SaveToSchema()
    {
        Assets.Scripts.Cloud.Schemas.Pen penSchema = new Assets.Scripts.Cloud.Schemas.Pen
        {
            Id = this.Id,
            IsUnlocked = this.isUnlocked,
            CurrentLevel = linkedStructure != null ? linkedStructure.currentLevelIndex : 0,
            Animals = new List<Assets.Scripts.Cloud.Schemas.Animal>()
        };

        foreach(var animal in animalsInPen)
        {
            penSchema.Animals.Add(animal.ToSchema());
        }

        return penSchema;
    }

    public Collider2D GetBounds()
    {
        return penBounds;
    }

    public bool Contains(Vector3 position)
    {
        return penBounds.OverlapPoint(position);
    }

    public bool IsFull()
    {
        int capacity = 2; 
        if (linkedStructure != null)
        {
            capacity = linkedStructure.GetCurrentCapacity();
        }

        return animalsInPen.Count >= capacity;
    }
    
    public void AddAniamal(FarmAnimal animal)
    {
        
        if (allowedAnimal == animal.GetAnimalType())
        {
            animalsInPen.Add(animal);
        }

        MarkDataDirty();
    }
}