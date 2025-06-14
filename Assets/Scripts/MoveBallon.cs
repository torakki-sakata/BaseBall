using UnityEngine;

public class MoveBalloon : MonoBehaviour
{
    private bool movingRight;
    private float moveSpeed;
    private float movedDistance = 0.0f;
    private float moveRange = 3.0f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        movingRight = Random.Range(0, 2) == 0;
        moveSpeed = Random.Range(0.05f, 0.1f);
    }

    void Update()
    {
        float delta = 15 * moveSpeed * Time.deltaTime;

        if (movingRight)
        {
            movedDistance += delta;
            if (movedDistance >= moveRange)
            {
                movedDistance = moveRange;
                movingRight = false;
            }
        }
        else
        {
            movedDistance -= delta;
            if (movedDistance <= -moveRange)
            {
                movedDistance = -moveRange;
                movingRight = true;
            }
        }

        transform.position = new Vector3(startPosition.x + movedDistance, startPosition.y, startPosition.z);
    }
}
