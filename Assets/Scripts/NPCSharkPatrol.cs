using UnityEngine;

public class NPCSharkPatrol : MonoBehaviour
{
    public Transform pointA, pointB, pointC, pointD, pointE;

    private Rigidbody2D rd;
    private Animator animator;
    private Transform targetPoint;

    [SerializeField] private float speed = 1.5f;
    private int currentStep = 0;
    private bool isStopped = false;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        transform.position = pointA.position;
        targetPoint = pointB;

        rd.gravityScale = 0;
        rd.freezeRotation = true;

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isStopped)
        {
            rd.linearVelocity = Vector2.zero;
            return;
        }

        MoveNPC();
        CheckArrival();
    }

    void MoveNPC()
    {
        Vector2 dir = (targetPoint.position - transform.position).normalized;
        rd.linearVelocity = dir * speed;
    }

    void CheckArrival()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
            SwitchNextTarget();
    }

    void SwitchNextTarget()
    {
        if (isStopped) return;

        currentStep = (currentStep + 1) % 8;
        targetPoint = currentStep switch
        {
            0 => pointB,
            1 => pointC,
            2 => pointD,
            3 => pointE,
            4 => pointD,
            5 => pointC,
            6 => pointB,
            _ => pointA
        };

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null || isStopped) return;
        animator.SetInteger("MovementStep", currentStep);
    }

    public void StopNPC(bool stop)
    {
        isStopped = stop;
        rd.linearVelocity = Vector2.zero;

        if (stop)
            rd.constraints = RigidbodyConstraints2D.FreezeAll;
        else
            rd.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
