using UnityEngine;
using UnityEngine.UI;

public class DynamicGridContent : MonoBehaviour
{
    public GridLayoutGroup grid;
    public RectTransform content;

    void Update()
    {
        int childCount = grid.transform.childCount;

        // Số cột dựa vào constraint
        int columns = grid.constraintCount;
        int rows = Mathf.CeilToInt((float)childCount / columns);

        float totalHeight = rows * (grid.cellSize.y + grid.spacing.y) - grid.spacing.y;

        // Cập nhật chiều cao content
        var size = content.sizeDelta;
        size.y = totalHeight;
        content.sizeDelta = size;
    }
}
