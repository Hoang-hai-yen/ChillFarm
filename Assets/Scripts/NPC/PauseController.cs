using NUnit.Framework;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused {get; private set;} = false;

    public static void setPause(bool pause)
    {
        IsGamePaused = pause;
    }
}