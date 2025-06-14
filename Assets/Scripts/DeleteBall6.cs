using UnityEngine;

public class DeleteBall6 : MonoBehaviour
{
    private Rigidbody rig;
    private PitchMini shooter;
    private CameraController cameraController;
    public MoveDefender[] defenders;
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<PitchMini>();
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
                Debug.LogWarning("PitchMini が見つかりません。");
            }

            foreach (MoveDefender defender in defenders)
            {
                defender?.OnBallReset();
            }
        }
    }
}
