using UnityEngine;

public class AddScore2 : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hit"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(100);
            }
        }

        if (collision.gameObject.CompareTag("2B"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(200);
            }
        }

        if (collision.gameObject.CompareTag("3B"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(300);
            }
        }

        if (collision.gameObject.CompareTag("HR"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(500);
            }
        }

        if (collision.gameObject.CompareTag("Straik"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(0);
            }
        }

        if (collision.gameObject.CompareTag("Faul"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(10);
            }
        }

        if (collision.gameObject.CompareTag("Out"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(-50);
            }
        }

        if (collision.gameObject.CompareTag("DoubleOut"))
        {
            if (ScoreManager2.Instance != null)
            {
                ScoreManager2.Instance.AddScore(-100);
            }
        }
    }
}
