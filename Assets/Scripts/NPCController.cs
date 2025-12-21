using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;

    private NPC_girlPatrol girlPatrol;
    private NPC_IanPatrol ianPatrol;

    private void Awake()
    {
        girlPatrol = GetComponent<NPC_girlPatrol>();
        ianPatrol = GetComponent<NPC_IanPatrol>();
    }

    private void OnEnable()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnShowDialog += PauseNPC;
            DialogManager.Instance.OnHideDialog += ResumeNPC;
        }
    }

    private void OnDisable()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnShowDialog -= PauseNPC;
            DialogManager.Instance.OnHideDialog -= ResumeNPC;
        }
    }

    public void Interact()
    {
        if (DialogManager.Instance != null)
            StartCoroutine(HandleDialog());
    }

    private IEnumerator HandleDialog()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private void PauseNPC()
    {
        if (girlPatrol != null) girlPatrol.StopNPC(true);
        if (ianPatrol != null) ianPatrol.StopNPC(true);
    }

    private void ResumeNPC()
    {
        if (girlPatrol != null) girlPatrol.StopNPC(false);
        if (ianPatrol != null) ianPatrol.StopNPC(false);
    }
}
