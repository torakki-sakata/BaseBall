using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager3 : MonoBehaviour
{
    public static ScoreManager3 Instance;

    private int score = 0;
    private int highScore = 0;

    [Header("Score UI")]
    public Text scoreText;
    public Text highScoreText;

    [Header("Feedback Texts")]
    public Text hitText;
    public Text outText;
    public Text twoBaseHitText;
    public Text threeBaseHitText;
    public Text homeRunText;
    public Text faulText;
    public Text straikText;
    public Text doubleOutText;

    private float hitDisplayTime = 1.5f;
    private float outDisplayTime = 1.5f;
    private float twoBaseHitDisplayTime = 1.5f;
    private float threeBaseHitDisplayTime = 1.5f;
    private float homeRunDisplayTime = 1.5f;
    private float faulDisplayTime = 1.5f;
    private float straikDisplayTime = 1.5f;
    private float doubleOutTime = 1.5f;

    private float hitTimer = 0f;
    private float outTimer = 0f;
    private float twoBaseHitTimer = 0f;
    private float threeBaseHitTimer = 0f;
    private float homeRunTimer = 0f;
    private float faulTimer = 0f;
    private float straikTimer = 0f;
    private float doubleOutTimer = 0f;

    private string highScoreKey;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 必要ならシーンを跨いでも残す
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        highScoreKey = "HighScore_" + SceneManager.GetActiveScene().name;
        LoadHighScore();
        UpdateScoreText();
        HideAllFeedbackTexts();
    }

    private void Update()
    {
        UpdateDisplayTimer(hitText, ref hitTimer, hitDisplayTime);
        UpdateDisplayTimer(outText, ref outTimer, outDisplayTime);
        UpdateDisplayTimer(twoBaseHitText, ref twoBaseHitTimer, twoBaseHitDisplayTime);
        UpdateDisplayTimer(threeBaseHitText, ref threeBaseHitTimer, threeBaseHitDisplayTime);
        UpdateDisplayTimer(homeRunText, ref homeRunTimer, homeRunDisplayTime);
        UpdateDisplayTimer(faulText, ref faulTimer, faulDisplayTime);
        UpdateDisplayTimer(straikText, ref straikTimer, straikDisplayTime);
        UpdateDisplayTimer(doubleOutText, ref doubleOutTimer, doubleOutTime);
    }

    private void UpdateDisplayTimer(Text text, ref float timer, float displayTime)
    {
        if (text != null && text.enabled)
        {
            timer += Time.deltaTime;
            if (timer > displayTime)
            {
                text.enabled = false;
                timer = 0f;
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;

        switch (points)
        {
            case 0:
                ShowStraikText();
                break;
            case 10:
                ShowFaulText();
                break;
            case 100:
                ShowHitText();
                break;
            case 200:
                ShowTwoBaseHitText();
                break;
            case 300:
                ShowThreeBaseHitText();
                break;
            case 500:
                ShowHomeRunText();
                break;
            case -50:
                ShowOutText();
                break;
            case -100:
                ShowDoubleOutText();
                break;
            default:
                Debug.LogWarning("Unknown score added: " + points);
                break;
        }

        UpdateHighScore();
        UpdateScoreText();
    }

    private void ShowHitText()
    {
        if (hitText != null)
        {
            hitText.enabled = true;
            hitText.text = "ヒット!";
            hitTimer = 0f;
        }
    }

    private void ShowTwoBaseHitText()
    {
        if (twoBaseHitText != null)
        {
            twoBaseHitText.enabled = true;
            twoBaseHitText.text = "ツーベース!!";
            twoBaseHitTimer = 0f;
        }
    }

    private void ShowThreeBaseHitText()
    {
        if (threeBaseHitText != null)
        {
            threeBaseHitText.enabled = true;
            threeBaseHitText.text = "スリーベース!!!";
            threeBaseHitTimer = 0f;
        }
    }

    private void ShowHomeRunText()
    {
        if (homeRunText != null)
        {
            homeRunText.enabled = true;
            homeRunText.text = "ホームラン!!!!";
            homeRunTimer = 0f;
        }
    }

    public void ShowOutText()
    {
        if (outText != null)
        {
            outText.enabled = true;
            outText.text = "アウト";
            outTimer = 0f;
        }
    }

    public void ShowFaulText()
    {
        if (faulText != null)
        {
            faulText.enabled = true;
            faulText.text = "ファウル";
            faulTimer = 0f;
        }
    }

    public void ShowStraikText()
    {
        if (straikText != null)
        {
            straikText.enabled = true;
            straikText.text = "ストライク";
            straikTimer = 0f;
        }
    }

    public void ShowDoubleOutText()
    {
        if (doubleOutText != null)
        {
            doubleOutText.enabled = true;
            doubleOutText.text = "ダブルプレー";
            doubleOutTimer = 0f;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "スコア: " + score.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = "ハイスコア: " + highScore.ToString();
        }
    }

    private void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, highScore);
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
        HideAllFeedbackTexts();
    }

    public void HideAllFeedbackTexts()
    {
        if (hitText != null) hitText.enabled = false;
        if (outText != null) outText.enabled = false;
        if (twoBaseHitText != null) twoBaseHitText.enabled = false;
        if (threeBaseHitText != null) threeBaseHitText.enabled = false;
        if (homeRunText != null) homeRunText.enabled = false;
        if (faulText != null) faulText.enabled = false;
        if (straikText != null) straikText.enabled = false;
        if (doubleOutText != null) doubleOutText.enabled = false;

        // タイマーもリセット
        hitTimer = outTimer = twoBaseHitTimer = threeBaseHitTimer = homeRunTimer = faulTimer = straikTimer = doubleOutTimer = 0f;
    }
}
