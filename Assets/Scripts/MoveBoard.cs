using UnityEngine;

public class MoveBoard : MonoBehaviour
{
    private bool movingUp;
    private float moveSpeed;
    private float movedDistance = 0.0f;
    private float moveRange = 1.0f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        movingUp = Random.Range(0, 2) == 0;
        moveSpeed = Random.Range(0.05f, 0.1f);
    }

    void Update()
    {
        float delta = 15 * moveSpeed * Time.deltaTime;
        if (movingUp)
        {
            movedDistance += delta;
            if (movedDistance >= moveRange)
            {
                movedDistance = moveRange;
                movingUp = false;
            }
        }
        else
        {
            movedDistance -= delta;
            if (movedDistance <= -moveRange)
            {
                movedDistance = -moveRange;
                movingUp = true;
            }
        }
        transform.position = new Vector3(startPosition.x, startPosition.y + movedDistance, startPosition.z);
    }
}
