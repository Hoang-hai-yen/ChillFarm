using UnityEngine;

public class BackpackToggle : MonoBehaviour
{
    public GameObject backpackPanel; // Gán panel chính ở đây
    private bool isOpen = false;

    void Start()
    {
        backpackPanel.SetActive(false); // Ẩn ban đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            backpackPanel.SetActive(isOpen);
        }
    }
}
