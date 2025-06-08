using UnityEngine;

public class DeleteBall4 : MonoBehaviour
{
    private Rigidbody rig;
    private PitchBall shooter;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        shooter = GetComponent<PitchBall>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bat")
        {
            // Y軸の固定を解除してフライ・ゴロを再現可能にする
            rig.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }

        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Bat")
        {
            // ボールリセット
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;
            float randomY = Random.Range(0.15f, 0.35f);
            transform.position = new Vector3(20.95f, randomY, -0.07f);

            // RigidbodyのFreezeを元に戻しておく（任意）
            rig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            // ShootBall に通知
            if (shooter != null)
            {
                shooter.ResetShoot();              // 発射許可リセット
                shooter.RegisterBallFinished();    // 終了カウント
            }
            else
            {
                Debug.LogWarning("ShootBall が見つかりません。");
            }
        }
    }
}
