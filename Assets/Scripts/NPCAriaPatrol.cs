using UnityEngine;

public class NPCAriaPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;

    private Rigidbody2D rd;
    private Animator animator;
    private Transform targetPoint;

    private enum MoveState { Up, Right, Left, Down }
    private MoveState currentState;

    [SerializeField] private float speed = 1f;
    private bool isStopped = false;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        transform.position = pointA.position;
        targetPoint = pointB;
        currentState = MoveState.Up;

        rd.gravityScale = 0;
        rd.freezeRotation = true;

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isStopped || pointA == null || pointB == null || pointC == null)
        {
            rd.linearVelocity = Vector2.zero;
            return;
        }

        MoveNPC();
        CheckArrival();
    }

    void MoveNPC()
    {
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rd.linearVelocity = direction * speed;
    }

    void CheckArrival()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
            SwitchNextTarget();
    }

    void SwitchNextTarget()
    {
        if (isStopped) return;

        switch (currentState)
        {
            case MoveState.Up:
                targetPoint = pointC; currentState = MoveState.Right; break;
            case MoveState.Right:
                targetPoint = pointB; currentState = MoveState.Left; break;
            case MoveState.Left:
                targetPoint = pointA; currentState = MoveState.Down; break;
            case MoveState.Down:
                targetPoint = pointB; currentState = MoveState.Up; break;
        }
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null || isStopped) return;
        animator.SetInteger("MovementState", (int)currentState + 1);
    }

    public void StopNPC(bool stop)
    {
        isStopped = stop;
        rd.linearVelocity = Vector2.zero;

        if (stop)
        {
            rd.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetInteger("MovementState", 0);
        }
        else
        {
            rd.constraints = RigidbodyConstraints2D.FreezeRotation;
            UpdateAnimation();
        }
    }
}
