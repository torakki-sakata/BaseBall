using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class Score : NetworkBehaviour
{
    [Networked] public int TotalScore { get; set; }
    [Networked] public int PitchCount { get; set; }
    [Networked] public int Score1 { get; set; }

    [Networked] public int InningNumber { get; set; } = 1;
    [Networked] public bool IsTopInning { get; set; } = true;

    private Text scoreText;
    private Text pitchCountText;
    private Text feedbackText;
    private Text changeText;
    private Text inningText;

    public event System.Action OnPitchLimitReached;

    public override void Spawned()
    {
        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        pitchCountText = GameObject.Find("PitchCountText")?.GetComponent<Text>();
        feedbackText = GameObject.Find("FeedbackText")?.GetComponent<Text>();
        changeText = GameObject.Find("ChangeText")?.GetComponent<Text>();
        inningText = GameObject.Find("InningText")?.GetComponent<Text>();

        UpdateUIAll();
    }

    public void RequestAddScore(int points)
    {
        if (Object.HasStateAuthority)
        {
            RPC_AddScore(points);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AddPitch()
    {
        PitchCount++;
        UpdatePitchCountUI();

        if (PitchCount >= 10)
        {
            FinalizeHalfInning();
            OnPitchLimitReached?.Invoke();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AddScore(int points)
    {
        TotalScore += points;
        UpdateScoreUI();
        ShowFeedback(points);
    }

    private void FinalizeHalfInning()
    {
        Score1 += TotalScore;
        TotalScore = 0;
        PitchCount = 0;

        UpdateScoreUI();
        UpdatePitchCountUI();

        // 表裏切替
        if (IsTopInning)
        {
            IsTopInning = false;
        }
        else
        {
            IsTopInning = true;
            InningNumber++;
        }

        UpdateInningUI();
        ShowChangeText("攻守交替！");
        OnInningChanged?.Invoke(IsTopInning);
    }

    public event System.Action<bool> OnInningChanged;

    private void UpdateUIAll()
    {
        UpdateScoreUI();
        UpdatePitchCountUI();
        UpdateInningUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {TotalScore}";
        }
    }

    private void UpdatePitchCountUI()
    {
        if (pitchCountText != null)
        {
            pitchCountText.text = $"投球数: {PitchCount}/10";
        }
    }

    private void UpdateInningUI()
    {
        if (inningText != null)
        {
            string half = IsTopInning ? "表" : "裏";
            inningText.text = $"{InningNumber}回{half}";
        }
    }

    private void ShowFeedback(int points)
    {
        if (feedbackText != null)
        {
            feedbackText.text = GetFeedback(points);
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    private string GetFeedback(int points)
    {
        return points switch
        {
            500 => "ホームラン！",
            300 => "スリーベースヒット！",
            200 => "ツーベースヒット！",
            100 => "ヒット！",
            10 => "ファウル",
            0 => "ストライク",
            -50 => "アウト",
            -100 => "ダブルアウト",
            _ => ""
        };
    }

    private void ShowChangeText(string message)
    {
        if (changeText != null)
        {
            changeText.text = message;
            changeText.enabled = true;
            CancelInvoke(nameof(HideChangeText));
            Invoke(nameof(HideChangeText), 2f);
        }
    }

    private void HideChangeText()
    {
        if (changeText != null)
        {
            changeText.enabled = false;
        }
    }

    public void RequestAddPitch()
    {
        if (Object.HasStateAuthority)
        {
            RPC_AddPitch();
        }
    }
}
