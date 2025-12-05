using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;

    private NPC_girlPatrol patrol;

    void Awake()
    {
        patrol = GetComponent<NPC_girlPatrol>();
    }
    
    public void Interact()
    {
        if (patrol != null)
            patrol.StopNPC(true);

        StartCoroutine(HandleDialog());
    }

    private IEnumerator HandleDialog()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        if (patrol != null)
            patrol.StopNPC(false);
    }
}
