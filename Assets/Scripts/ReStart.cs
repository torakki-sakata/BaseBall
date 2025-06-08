using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ReStart : MonoBehaviour
{
    public void OnClick()
    {
        // スコアリセット
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.HideAllFeedbackTexts();
        }

        // Ballの発射系もリセット
        ShootBall shooter = FindObjectOfType<ShootBall>();
        if (shooter != null)
        {
            shooter.ResetGame();
        }

        // シーン再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}