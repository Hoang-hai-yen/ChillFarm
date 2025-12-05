using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_girlPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private Rigidbody2D rd;
    private Animator animator;
    private Transform currentPoint;

    [SerializeField] private float speed = 2f;

    void Start()
    {
        rd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentPoint = pointB; 
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        float directionY = currentPoint.position.y > transform.position.y ? 1f : -1f;
        rd.linearVelocity = new Vector2(0, directionY * speed);

        float distanceY = Mathf.Abs(transform.position.y - currentPoint.position.y);

        if (distanceY < 0.1f)
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
