using UnityEngine;

public class MoveDefender : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float moveRadius = 2.0f;
    public Transform ball;

    private Vector3 initialPosition;
    private bool isMoving = false;
    private bool returningToStart = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isMoving && ball != null)
        {
            FollowBallWithinRadius();
        }
        else if (returningToStart)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
            {
                transform.position = initialPosition;
                returningToStart = false;
            }
        }
    }

    private void FollowBallWithinRadius()
    {
        Vector3 directionToBall = ball.position - initialPosition;
        Vector3 clampedOffset = Vector3.ClampMagnitude(directionToBall, moveRadius);
        Vector3 targetPosition = initialPosition + clampedOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void OnBallHitBat()
    {
        isMoving = true;
        returningToStart = false;
    }

    public void OnBallReset()
    {
        isMoving = false;
        returningToStart = true;
    }
}
