using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Score : MonoBehaviour
{
    public static Score Instance;

    private int score = 0;
    private int highScore = 0;
    public Text scoreText;
    public Text highScoreText;
    public Text hitText;
    private float hitDisplayTime = 1.5f;
    private float hitTimer = 0f;
    public Text outText;
    private float outDisplayTime = 1.5f;
    private float outTimer = 0f;
    public Text twoBaseHitText;
    private float twoBaseHitDisplayTime = 1.5f;
    private float twoBaseHitTimer = 0f;
    public Text threeBaseHitText;
    private float threeBaseHitDisplayTime = 1.5f;
    private float threeBaseHitTimer = 0f;
    public Text homeRunText;
    private float homeRunDisplayTime = 1.5f;
    private float homeRunTimer = 0f;
    public Text faulText;
    private float faulDisplayTime = 1.5f;
    private float faulTimer = 0f;
    public Text straikText;
    private float straikDisplayTime = 1.5f;
    private float straikTimer = 0f;
    public Text doubleOutText;
    private float doubleOutTime = 1.5f;
    private float doubleOutTimer = 0f;
    private string highScoreKey;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        highScoreKey = "HighScore_" + SceneManager.GetActiveScene().name;
        LoadHighScore();
        UpdateScoreText();
        HideAllFeedbackTexts();
    }

    void Update()
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
                ShowdoubleOutText();
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
        }
    }

    private void ShowTwoBaseHitText()
    {
        if (twoBaseHitText != null)
        {
            twoBaseHitText.enabled = true;
            twoBaseHitText.text = "ツーベース!!";
        }
    }

    private void ShowThreeBaseHitText()
    {
        if (threeBaseHitText != null)
        {
            threeBaseHitText.enabled = true;
            threeBaseHitText.text = "スリーベース!!!";
        }
    }

    private void ShowHomeRunText()
    {
        if (homeRunText != null)
        {
            homeRunText.enabled = true;
            homeRunText.text = "ホームラン!!!!";
        }
    }

    public void ShowOutText()
    {
        if (outText != null)
        {
            outText.enabled = true;
            outText.text = "アウト";
        }
    }

    public void ShowFaulText()
    {
        if (faulText != null)
        {
            faulText.enabled = true;
            faulText.text = "ファウル";
        }
    }

    public void ShowStraikText()
    {
        if (straikText != null)
        {
            straikText.enabled = true;
            straikText.text = "ストライク";
        }
    }

    public void ShowdoubleOutText()
    {
        if (doubleOutText != null)
        {
            doubleOutText.enabled = true;
            doubleOutText.text = "ダブルプレー";
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "スコア: " + $"{score}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = "ハイスコア: " + $"{highScore}";
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
        if (scoreText != null)
            scoreText.text = "スコア: 0";

        if (highScoreText != null)
            highScoreText.text = "ハイスコア: " + highScore.ToString();
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
    }
}
