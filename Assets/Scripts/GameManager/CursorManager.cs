using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Textures")]
    [Tooltip("Hình chuột mặc định")]
    public Texture2D defaultCursor;
    
    [Tooltip("Hình chuột khi nhấn Click (Optional)")]
    public Texture2D clickCursor;

    [Header("Settings")]
    public Vector2 hotSpot = Vector2.zero; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetDefaultCursor();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (clickCursor != null) 
                Cursor.SetCursor(clickCursor, hotSpot, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetDefaultCursor();
        }
    }

    public void SetDefaultCursor()
    {
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
        }
    }

    public void SetCustomCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}