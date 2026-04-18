using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;

    private NPC_girlPatrol girlPatrol;
    private NPC_IanPatrol ianPatrol;
    private NPCAriaPatrol ariaPatrol;
    private NPCSharkPatrol sharkPatrol;

    private bool isInteracting = false;

    private void Awake()
    {
        girlPatrol = GetComponent<NPC_girlPatrol>();
        ianPatrol = GetComponent<NPC_IanPatrol>();
        ariaPatrol = GetComponent<NPCAriaPatrol>();
        sharkPatrol = GetComponent<NPCSharkPatrol>();
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
        if (isInteracting) return;

        if (DialogManager.Instance != null)
            StartCoroutine(HandleDialog());
    }

    public void PauseDirect()
    {
        PauseNPC();
    }


    private IEnumerator HandleDialog()
    {
        isInteracting = true;
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        isInteracting = false;
    }

    private void PauseNPC()
    {
        if (girlPatrol != null) girlPatrol.StopNPC(true);
        if (ianPatrol != null) ianPatrol.StopNPC(true);
        if (ariaPatrol != null) ariaPatrol.StopNPC(true);
        if (sharkPatrol != null) sharkPatrol.StopNPC(true);
    }

    private void ResumeNPC()
    {
        if (girlPatrol != null) girlPatrol.StopNPC(false);
        if (ianPatrol != null) ianPatrol.StopNPC(false);
        if (ariaPatrol != null) ariaPatrol.StopNPC(false);
        if (sharkPatrol != null) sharkPatrol.StopNPC(false);
    }
}
