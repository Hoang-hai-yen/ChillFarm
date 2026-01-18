using System;
using UnityEngine;

// [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Generic Item")] 
// --------------------
public class GameSOData : ScriptableObject
{
    public string Id;
    public string Name;
    protected virtual void OnValidate()
    {
        if(String.IsNullOrEmpty(Id))
        {
            Id = Name + "_" + Guid.NewGuid().ToString();
        }
    }


}
