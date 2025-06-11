using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform ball;
    public Vector3 offset = new Vector3(0, 3, -6);
    private bool followBall = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = new Vector3(35.26f, 1.14f, 0f);
        transform.position = initialPosition;
    }

    void LateUpdate()
    {
        if (followBall && ball != null)
        {
            transform.position = ball.position + offset;
        }
    }

    public void StartFollowingBall()
    {
        followBall = true;
    }

    public void ResetCamera()
    {
        followBall = false;
        transform.position = initialPosition;
    }
}
