using UnityEngine;

public class Mine : MonoBehaviour
{
    public int penaltyValue = -50;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(penaltyValue);
            }
        }
    }
}
