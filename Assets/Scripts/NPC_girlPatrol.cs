using UnityEngine;

public class NPC_girlPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private Rigidbody2D rd;
    private Animator animator;
    private Transform currentPoint;

    [SerializeField] private float speed = 1f;
    private bool isStopped = false;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentPoint = pointB;
        UpdateAnimation();

        rd.gravityScale = 0;
        rd.freezeRotation = true;
        rd.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (isStopped || pointA == null || pointB == null)
        {
            rd.linearVelocity = Vector2.zero;
            return;
        }

        float directionY = currentPoint.position.y > transform.position.y ? 1f : -1f;
        rd.linearVelocity = new Vector2(0, directionY * speed);

        float distanceY = Mathf.Abs(transform.position.y - currentPoint.position.y);
        if (distanceY < 0.05f)
        {
            currentPoint = (currentPoint == pointB) ? pointA : pointB;
            UpdateAnimation();
        }
    }

    void UpdateAnimation()
    {
        if (animator == null || isStopped) return;
        animator.SetBool("isRunning", currentPoint != pointB);
    }

    public void StopNPC(bool stop)
    {
        isStopped = stop;
        rd.linearVelocity = Vector2.zero;

        if (stop)
        {
            rd.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetBool("isRunning", false);
        }
        else
        {
            rd.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            UpdateAnimation();
        }
    }
}
