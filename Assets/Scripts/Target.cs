using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxRadius = 10f; // スコア範囲の最大半径

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball hit detected on Target");

            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;

            float distance = Vector3.Distance(hitPoint, transform.position);
            int score = GetScoreBasedOnDistance(distance);

            Debug.Log("Distance: " + distance + ", Score: " + score);

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(score);
            }
            else
            {
                Debug.LogError("ScoreManager.Instance is null");
            }
        }
    }

    private int GetScoreBasedOnDistance(float distance)
    {
        if (distance <= 0.1f)
        {
            return 500; // ホームラン
        }
        else if (distance <= 0.2f)
        {
            return 300; // 三塁打
        }
        else if (distance <= 0.3f)
        {
            return 200; // 二塁打
        }
        else if (distance <= 0.4f)
        {
            return 100; // ヒット
        }
        else if (distance <= maxRadius)
        {
            return 10; // ファウル
        }
        else
        {
            return 0; // ストライク（スコアなし）
        }
    }
}
