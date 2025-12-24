using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [Header("Cài đặt Chuồng")]
    public AnimalType allowedAnimal; 
    
    private Collider2D penBounds;

    void Awake()
    {
        penBounds = GetComponent<Collider2D>();
        if (penBounds == null) Debug.LogError("AnimalPen: Thiếu Collider2D để định vị chuồng!");
    }

    public Collider2D GetBounds()
    {
        return penBounds;
    }

    public bool Contains(Vector3 position)
    {
        return penBounds.OverlapPoint(position);
    }
}