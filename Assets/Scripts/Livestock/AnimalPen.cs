using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [Header("Cài đặt Chuồng")]
    public AnimalType allowedAnimal; 
    
    public BarnStructure linkedStructure; 

    private Collider2D penBounds;

    void Awake()
    {
        penBounds = GetComponent<Collider2D>();
        if (penBounds == null) Debug.LogError("AnimalPen: Thiếu Collider2D!");
    }

    public Collider2D GetBounds()
    {
        return penBounds;
    }

    public bool Contains(Vector3 position)
    {
        return penBounds.OverlapPoint(position);
    }

    public bool IsFull(int currentAnimalCount)
    {
        int capacity = 2; 
        if (linkedStructure != null)
        {
            capacity = linkedStructure.GetCurrentCapacity();
        }
        
        return currentAnimalCount >= capacity;
    }
}