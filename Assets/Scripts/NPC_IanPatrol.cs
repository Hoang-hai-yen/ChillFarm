using System.Collections;
using UnityEngine;

public class NPC_IanPatrol : MonoBehaviour
{
    public Transform pointB;
    public Transform pointC;

    private Rigidbody2D rd;
    private Animator animator;
    private Transform currentPoint;

    [SerializeField] private float speed = 1f;

    private bool isStopped = false;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentPoint = pointC;
        UpdateAnimation();

        rd.gravityScale = 0;
        rd.freezeRotation = true;
        rd.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (isStopped)
        {
            rd.linearVelocity = Vector2.zero;
            return;
        }

        if (pointB == null || pointC == null) return;

        float directionX = currentPoint.position.x > transform.position.x ? 1f : -1f;
        rd.linearVelocity = new Vector2(directionX * speed, 0);

        float distanceX = Mathf.Abs(transform.position.x - currentPoint.position.x);

        if (distanceX < 0.05f)
        {
            currentPoint = (currentPoint == pointC) ? pointB : pointC;
            UpdateAnimation();
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (currentPoint == pointC)
            animator.SetBool("isRunning", false);
        else
            animator.SetBool("isRunning", true);
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
            rd.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            UpdateAnimation();
        }
    }


    private void OnDrawGizmos()
    {
        if (pointB != null && pointC != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
            Gizmos.DrawWireSphere(pointC.position, 0.3f);
            Gizmos.DrawLine(pointB.position, pointC.position);
        }
    }
}
