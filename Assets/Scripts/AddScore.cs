using UnityEngine;

public class AddScore : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // フェンスに当たった場合
        if (collision.gameObject.CompareTag("Hit"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(100); // 100点加算
            }
        }

        // 2ベースヒットボードに当たった場合
        if (collision.gameObject.CompareTag("2B"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(200); // 200点加算
            }
        }

        // 3ベースヒットボードに当たった場合
        if (collision.gameObject.CompareTag("3B"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(300); // 300点加算
            }
        }

        // ホームランボードに当たった場合
        if (collision.gameObject.CompareTag("HR"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(500); // 500点加算
            }
        }

        // 審判に当たった場合（ストライク）
        if (collision.gameObject.CompareTag("Straik"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(0); // 0点（ストライク扱い）
            }
        }

        // ファウルゾーンに当たった場合
        if (collision.gameObject.CompareTag("Faul"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(10); // 10点（ファウル扱い）
            }
        }

        // アウトボードに当たった場合
        if (collision.gameObject.CompareTag("Out"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(-50); // -50点（アウト扱い）
            }
        }

        // ダブルアウトボードに当たった場合
        if (collision.gameObject.CompareTag("DoubleOut"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(-100); // -100点（ダブルアウト扱い）
            }
        }
    }
}
