using UnityEngine;

public class DeleteBall3 : MonoBehaviour
{
    private Rigidbody rig;
    private ShootBall3 shooter;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<ShootBall3>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bat")
        {
            rig.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }

        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Bat")
        {
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
                Debug.LogWarning("ShootBall が見つかりません。");
            }
        }
    }
}
