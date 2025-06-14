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

    public AudioClip[] musicTracksSet1;
    public AudioClip[] musicTracksSet2;

    private AudioClip[] currentMusicTracks;
    private int currentTrackIndex = 0;
    private int currentSetIndex = 0; // 0: Set1, 1: Set2

    public AudioClip endTrack;
    private AudioSource audioSource;
    private bool gameEnded = false;

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

        audioSource = GetComponent<AudioSource>();
        currentMusicTracks = musicTracksSet1;

        if (audioSource != null && currentMusicTracks.Length > 0)
        {
            audioSource.clip = currentMusicTracks[currentTrackIndex];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && endText.gameObject.activeSelf)
        {
            SceneManager.LoadScene("Start");
        }

        if (!gameEnded && audioSource != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentTrackIndex = (currentTrackIndex + 1) % currentMusicTracks.Length;
                audioSource.clip = currentMusicTracks[currentTrackIndex];
                audioSource.Play();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentTrackIndex = (currentTrackIndex - 1 + currentMusicTracks.Length) % currentMusicTracks.Length;
                audioSource.clip = currentMusicTracks[currentTrackIndex];
                audioSource.Play();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // 曲リスト切り替え
                currentSetIndex = (currentSetIndex == 0) ? 1 : 0;
                currentMusicTracks = (currentSetIndex == 0) ? musicTracksSet1 : musicTracksSet2;
                currentTrackIndex = 0;

                if (currentMusicTracks.Length > 0)
                {
                    audioSource.clip = currentMusicTracks[currentTrackIndex];
                    audioSource.Play();
                }
            }
        }
    }

    public void BallThrown()
    {
        ballCount++;
        ballCountText.text = "投球数: " + ballCount.ToString();

        if (ballCount >= 10)
        {
            int inningScore = ScoreManager2.Instance.GetScore();
            if (isTop)
                score1 += inningScore;
            else
                score2 += inningScore;

            ScoreManager2.Instance.ResetScore();
            UpdateScoreDisplays();

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
        yield return new WaitForSeconds(1.0f);
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
        pitchBall.ResetGame();
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
        gameEnded = true;

        endText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        titleButton.gameObject.SetActive(true);

        if (audioSource != null && endTrack != null)
        {
            audioSource.clip = endTrack;
            audioSource.loop = true;
            audioSource.Play();
        }

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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Start");
    }
}
