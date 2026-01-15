using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Databases/ItemDatabase")]
public class GameSODatabase : ScriptableObject {
    public List<GameSOData> allItems;
    private Dictionary<string, GameSOData> _itemDict;

    // Chuyển List sang Dictionary để tìm kiếm nhanh (O(1))
    public void Initialize() {
        _itemDict = new Dictionary<string, GameSOData>();
        foreach (var item in allItems) {
            if (!_itemDict.ContainsKey(item.Id))
                _itemDict.Add(item.Id, item);
        }
    }

    public GameSOData GetItemById(string id) {
        if (_itemDict == null) Initialize();
        _itemDict.TryGetValue(id, out var item);
        return item;
    }
}