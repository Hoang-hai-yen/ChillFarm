using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Dialogue
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    GameState state;

    private void Start()
    {
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialogue;
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (state == GameState.Dialogue)
            {
                state = GameState.FreeRoam;
            }    
                
        };
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        } 
        else if (state == GameState.Dialogue)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {

        }
    }
}
