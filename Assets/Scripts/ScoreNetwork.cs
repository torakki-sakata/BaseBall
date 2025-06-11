using Fusion;
using UnityEngine;
using System.Linq;

public class ScoreNetwork : NetworkBehaviour
{
    public static ScoreNetwork Instance;

    [Networked] public int Player1Score { get; set; }
    [Networked] public int Player2Score { get; set; }

    public override void Spawned()
    {
        Instance = this;
        Debug.Log("【ScoreNetwork】Spawned & Instanceセット");
    }

    public void AddScore(PlayerRef player, int points)
    {
        Debug.Log($"AddScore called: Player = {player}, Points = {points}");

        if (Runner == null || !Runner.IsRunning)
        {
            Debug.LogWarning("Runner is not ready.");
            return;
        }

        var players = Runner.ActivePlayers.ToArray();
        if (players.Length < 2)
        {
            Debug.LogWarning("Not enough players.");
            return;
        }

        var firstPlayer = players[0];
        var secondPlayer = players[1];

        if (player == firstPlayer)
        {
            Player1Score += points;
        }
        else if (player == secondPlayer)
        {
            Player2Score += points;
        }
        else
        {
            Debug.LogWarning("Player not recognized in current session.");
        }

        CheckWinCondition();
    }

    public void ResetScores()
    {
        Player1Score = 0;
        Player2Score = 0;
    }

    public void CheckWinCondition()
    {
        if (GameLauncher.Instance?.shootBall == null) return;

        int currentBall = GameLauncher.Instance.shootBall.GetBallCount();
        int maxBall = GameLauncher.Instance.shootBall.GetMaxBalls();

        if (currentBall >= maxBall)
        {
            GameLauncher.Instance.ShowResult();
        }
    }
}
