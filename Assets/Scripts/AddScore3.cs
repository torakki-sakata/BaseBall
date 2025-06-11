using UnityEngine;
using Fusion;

public class AddScore3 : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"【AddScore3】衝突検出: {collision.gameObject.name} with tag: {collision.gameObject.tag}");

        if (ScoreNetwork.Instance == null)
        {
            Debug.LogWarning("【AddScore3】ScoreNetwork.Instance is null → スコア処理スキップ");
            return;
        }

        PlayerRef localPlayer = ScoreNetwork.Instance.Runner.LocalPlayer;

        int score = collision.gameObject.tag switch
        {
            "Hit" => 100,
            "2B" => 200,
            "3B" => 300,
            "HR" => 500,
            "Faul" => 10,
            "Out" => -50,
            "DoubleOut" => -100,
            "Straik" => 0,
            _ => 0
        };
        ScoreNetwork.Instance.AddScore(localPlayer, score);
        GameLauncher.Instance?.AddLocalScoreFeedback(score, collision.gameObject.tag);
        GameLauncher.Instance?.AddPitchCountAndCheckEnd();
    }
}
