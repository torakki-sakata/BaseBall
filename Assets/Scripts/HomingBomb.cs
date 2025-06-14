using UnityEngine;

public class HomingBomb : MonoBehaviour
{
    public float speed = 5f;
    public float followRadius = 3.0f;
    public int penaltyValue = -100;
    public Transform ball;

    private Vector3 initialPosition;
    private bool isTracking = false;
    private bool returningToStart = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isTracking && ball != null)
        {
            FollowBallWithinRadius();
        }
        else if (returningToStart)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
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
        Vector3 clampedOffset = Vector3.ClampMagnitude(directionToBall, followRadius);
        Vector3 targetPosition = initialPosition + clampedOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(penaltyValue);
            }
            Destroy(gameObject);
        }
    }

    public void OnBallHit()
    {
        isTracking = true;
        returningToStart = false;
    }

    public void OnBallReset()
    {
        isTracking = false;
        returningToStart = true;
    }
}
