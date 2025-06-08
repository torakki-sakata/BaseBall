using UnityEngine;
using Fusion;
using System.Linq;

public class DeleteBall5 : MonoBehaviour
{
    private Ball ball;
    private static readonly Vector3 resetPosition = new Vector3(20.95001f, 0.1999969f, -0.07f);

    void Start()
    {
        ball = GetComponent<Ball>();

        if (ball == null) Debug.LogError("Ball コンポーネントが見つかりません！");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ball == null || !ball.Object.HasStateAuthority) return;

        string tag = collision.gameObject.tag;
        int points = tag switch
        {
            "Hit" => 100,
            "2B" => 200,
            "3B" => 300,
            "HR" => 500,
            "Foul" or "Faul" => 10,
            "Strike" or "Straik" => 0,
            "Out" => -50,
            "DoubleOut" => -100,
            _ => int.MinValue
        };

        if (points != int.MinValue)
        {
            var score = FindObjectsOfType<Score>().FirstOrDefault(s => s.Object.HasStateAuthority);
            if (score != null)
            {
                score.RequestAddScore(points);
                score.RequestAddPitch(); // 投球数も加算（上記のタグなら全て対象）
            }

            ball.Rb.velocity = Vector3.zero;
            ball.Rb.angularVelocity = Vector3.zero;
            ball.transform.position = resetPosition;
            ball.transform.rotation = Quaternion.identity;
            ball.Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }
}
