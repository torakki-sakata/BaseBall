using UnityEngine;

public class DeleteBall4 : MonoBehaviour
{
    private Rigidbody rig;
    private PitchBall shooter;
    private CameraController cameraController;

    public MoveDefender[] defenders; // ← 複数のDefenderをInspectorで登録可能に

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<PitchBall>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Bat")
        {
            rig.constraints &= ~RigidbodyConstraints.FreezePositionY;

            // カメラ追尾開始
            cameraController?.StartFollowingBall();

            // 全Defenderに「打たれた」通知
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

            // ボールリセット処理
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
                Debug.LogWarning("PitchBall が見つかりません。");
            }

            // 全Defenderに「リセット」通知
            foreach (MoveDefender defender in defenders)
            {
                defender?.OnBallReset();
            }
        }
    }
}
