using UnityEngine;

public class DeleteBall : MonoBehaviour
{
    private Rigidbody rig;
    private ShootBall shooter;
    private CameraController cameraController;
    public MoveDefender[] defenders;
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<ShootBall>();
        cameraController = Camera.main.GetComponent<CameraController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
                if (hitSound != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;
            transform.position = new Vector3(20.95f, 0.2f, -0.07f);
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
            
            foreach (MoveDefender defender in defenders)
            {
                defender?.OnBallReset();
            }
        }
    }
}
