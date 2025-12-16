using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;

    private NPC_girlPatrol girlPatrol;
    private NPC_IanPatrol ianPatrol;

    void Awake()
    {
        girlPatrol = GetComponent<NPC_girlPatrol>();
        ianPatrol = GetComponent<NPC_IanPatrol>();
    }

    public void Interact()
    {
        if (girlPatrol != null)
            girlPatrol.StopNPC(true);

        if (ianPatrol != null)
            ianPatrol.StopNPC(true);

        StartCoroutine(HandleDialog());
    }

    private IEnumerator HandleDialog()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        if (girlPatrol != null)
            girlPatrol.StopNPC(false);

        if (ianPatrol != null)
            ianPatrol.StopNPC(false);
    }
}
