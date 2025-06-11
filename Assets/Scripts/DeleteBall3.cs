using UnityEngine;

public class DeleteBall3 : MonoBehaviour
{
    private Rigidbody rig;
    private ShootBall3 shooter;
    private CameraController cameraController;
    public MoveDefender[] defenders;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<ShootBall3>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Bat")
        {
            rig.constraints &= ~RigidbodyConstraints.FreezePositionY;
            cameraController?.StartFollowingBall();
            foreach (MoveDefender defender in defenders)
            {
                defender?.OnBallHitBat();
            }
        }

        if (tag != "Ground" && tag != "Bat")
        {
            if (tag == "Hit" || tag == "Faul" || tag == "2B" || tag == "3B" ||
                tag == "HR" || tag == "Straik" || tag == "DoubleOut" || tag == "Out")
            {
                cameraController?.ResetCamera();
            }
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;

            float randomY = Random.Range(0.15f, 0.35f);
            transform.position = new Vector3(20.95f, randomY, -0.07f);

            rig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            if (shooter != null)
            {
                shooter.ResetShoot();
                shooter.RegisterBallFinished();
            }
            else
            {
                Debug.LogWarning("ShootBall3 が見つかりません。");
            }

            foreach (MoveDefender defender in defenders)
            {
                defender?.OnBallReset();
            }
        }
    }
}
