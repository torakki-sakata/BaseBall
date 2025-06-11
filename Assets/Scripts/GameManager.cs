using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Text inningText;
    public Text inningHalfText;
    public Text endText;
    public Text winnerText;
    public Text gameText;
    public Text changeText;
    public Text firstAttackText;
    public Text secondAttackText;
    public Text ballCountText;
    public Button restartButton;
    public Button titleButton;

    private int inning = 1;
    private bool isTop = true;
    private int ballCount = 0;
    private int score1 = 0;
    private int score2 = 0;

    private PitchBall pitchBall;

    void Start()
    {
        pitchBall = FindObjectOfType<PitchBall>();

        UpdateInningDisplay();
        UpdateScoreDisplays();

        endText.gameObject.SetActive(false);
        winnerText.gameObject.SetActive(false);
        gameText.gameObject.SetActive(false);
        changeText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        titleButton.gameObject.SetActive(false);
        ballCountText.text = "投球数: 0";
    }

    public void BallThrown()
    {
        ballCount++;
        ballCountText.text = "投球数: " + ballCount.ToString();

        if (ballCount >= 10 )
        {
            // 10球目の得点を先に反映
            int inningScore = ScoreManager2.Instance.GetScore();
            if (isTop)
                score1 += inningScore;
            else
                score2 += inningScore;

            ScoreManager2.Instance.ResetScore();
            UpdateScoreDisplays();

            // 判定チェック（試合終了の可能性）
            bool isGameEnd = (inning == 3 && isTop && score1 < score2) || (inning == 3 && !isTop);

            if (isGameEnd)
            {
                StartCoroutine(DelayedGameEnd());
            }
            else
            {
                StartCoroutine(ShowChangeThenProceed());
            }
        }
    }


    private IEnumerator ShowChangeThenProceed()
    {
        yield return new WaitForSeconds(0.25f);
        changeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        changeText.gameObject.SetActive(false);
        ProcessEndOfHalfInning();
    }

    private IEnumerator DelayedGameEnd()
    {
        yield return new WaitForSeconds(1.0f);  // 3秒の待機
        ProcessEndOfHalfInning();
        EndGame();
    }



    private void ProcessEndOfHalfInning()
    {
        ballCount = 0;
        ballCountText.text = "投球数: 0";

        if (isTop)
        {
            isTop = false;
        }
        else
        {
            isTop = true;
            inning++;
        }

        UpdateInningDisplay();
        pitchBall.ResetGame();  // ゲーム終了で呼ばれないように制御済み
    }



    private void UpdateInningDisplay()
    {
        inningText.text = inning.ToString();
        inningHalfText.text = isTop ? "回表" : "回裏";
    }

    private void UpdateScoreDisplays()
    {
        firstAttackText.text = score1.ToString();
        secondAttackText.text = score2.ToString();
    }

    private void EndGame()
    {
        endText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);

        if (score1 > score2)
        {
            winnerText.text = "先攻が勝利‼";
            winnerText.gameObject.SetActive(true);
        }
        else if (score1 < score2)
        {
            winnerText.text = "後攻が勝利‼";
            winnerText.gameObject.SetActive(true);
        }
        else
        {
            gameText.text = "引き分け";
            gameText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && endText.gameObject.activeSelf)
        {
            SceneManager.LoadScene("Start");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Start");
    }
}
