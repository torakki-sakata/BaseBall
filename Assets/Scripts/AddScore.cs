using UnityEngine;

public class AddScore : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hit"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(100);
            }
        }

        if (collision.gameObject.CompareTag("2B"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(200);
            }
        }

        if (collision.gameObject.CompareTag("3B"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(300);
            }
        }

        if (collision.gameObject.CompareTag("HR"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(500);
            }
        }

        if (collision.gameObject.CompareTag("Straik"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(0);
            }
        }

        if (collision.gameObject.CompareTag("Faul"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(10);
            }
        }
        if (collision.gameObject.CompareTag("Out"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(-50);
            }
        }

        if (collision.gameObject.CompareTag("DoubleOut"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(-100);
            }
        }
    }
}
