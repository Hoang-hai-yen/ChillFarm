using System;
using UnityEngine;

//public enum GameActionType { PLANT, HARVEST, FISH }
public class EventManager 
{
    public static event Action<string, int> OnGameAction;

    public static void TriggerAction(string itemId, int amount = 1)
    {
        OnGameAction?.Invoke(itemId, amount);
    }
}
