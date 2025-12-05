using System.Collections;
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
        if (isStopped)
        {
            rd.linearVelocity = Vector2.zero;
            return;
        }

        if (pointA == null || pointB == null) return;

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
        if (animator == null) return;

        if (currentPoint == pointB)
            animator.SetBool("isRunning", false);
        else
            animator.SetBool("isRunning", true);
    }

    public void StopNPC(bool stop)
    {
        isStopped = stop;
        rd.linearVelocity = Vector2.zero;

        if (stop)
            animator.SetBool("isRunning", false);
        else
            UpdateAnimation();
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
