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
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnShowDialog += EnterDialogueState;
            DialogManager.Instance.OnHideDialog += ExitDialogueState;
        }
    }

    private void OnDestroy()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnShowDialog -= EnterDialogueState;
            DialogManager.Instance.OnHideDialog -= ExitDialogueState;
        }
    }

    private void EnterDialogueState()
    {
        state = GameState.Dialogue;
    }

    private void ExitDialogueState()
    {
        if (state == GameState.Dialogue) state = GameState.FreeRoam;
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
    }
}