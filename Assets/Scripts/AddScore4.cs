using UnityEngine;

public class AddScore4 : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (ScoreManager3.Instance == null) return;

        switch (collision.gameObject.tag)
        {
            case "Hit":
                ScoreManager3.Instance.AddScore(100);
                break;
            case "2B":
                ScoreManager3.Instance.AddScore(200);
                break;
            case "3B":
                ScoreManager3.Instance.AddScore(300);
                break;
            case "HR":
                ScoreManager3.Instance.AddScore(500);
                break;
            case "Straik":
                ScoreManager3.Instance.AddScore(0);
                break;
            case "Faul":
                ScoreManager3.Instance.AddScore(10);
                break;
            case "Out":
                ScoreManager3.Instance.AddScore(-50);
                break;
            case "DoubleOut":
                ScoreManager3.Instance.AddScore(-100);
                break;
            default:
                break;
        }
    }
}
