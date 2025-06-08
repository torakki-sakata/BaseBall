using UnityEngine;
using System.Collections;

public class PitchBall : MonoBehaviour
{
    float timer = 0.0f;
    bool shootSwitch = true;

    float initialForce = 50f;
    float initialForce2 = 35f;
    float curveForce = 4f;

    int ballCount = 0;
    int maxBalls = 10;
    bool allBallsThrown = false;

    private Rigidbody rig;
    private Renderer rend;
    private bool isInvisible = false;

    private GameManager gameManager;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        shootSwitch = true;

        gameManager = FindObjectOfType<GameManager>(); // ← これで取得
    }

    void Update()
    {
        if (!shootSwitch || ballCount >= maxBalls) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 速球
        {
            rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 遅球
        {
            rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse);
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // スライダー
        {
            StartCoroutine(ThrowSlider());
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // シュート
        {
            StartCoroutine(ThrowShoot());
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.V)) // 消える魔球
        {
            StartCoroutine(ThrowInvisibleBall());
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.F)) // 火の玉ストレート
        {
            StartCoroutine(ThrowFireball());
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.I)) // 幻惑ボール
        {
            StartCoroutine(ThrowIllusionBall());
            RegisterShot();
        }
        else if (Input.GetKeyDown(KeyCode.S)) // ストップボール
        {
            StartCoroutine(ThrowStopBall());
            RegisterShot();
        }
    }

    void RegisterShot()
    {
        shootSwitch = false;
    }


    IEnumerator ThrowSlider()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.65f);
        rig.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.01f);
        rig.AddForce(initialForce * 0.6f, 0, curveForce, ForceMode.Impulse);
    }

    IEnumerator ThrowShoot()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.65f);
        rig.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.01f);
        rig.AddForce(initialForce * 0.6f, 0, -curveForce, ForceMode.Impulse);
    }

    IEnumerator ThrowInvisibleBall()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.45f);
        rend.enabled = false;
        isInvisible = true;
        yield return new WaitForSeconds(0.55f);
        rend.enabled = true;
        isInvisible = false;
    }

    IEnumerator ThrowFireball()
    {
        rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.6f);
        rig.AddForce(initialForce * 0.5f, 0, 0, ForceMode.Impulse);
    }

    IEnumerator ThrowIllusionBall()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        float elapsed = 0f;
        bool toRight = true;

        while (elapsed < 4.1f)
        {
            yield return new WaitForSeconds(0.1f);
            rig.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.01f);
            float angle = toRight ? curveForce : -curveForce;
            rig.AddForce(initialForce * 0.15f, 0, angle, ForceMode.Impulse);
            toRight = !toRight;
            elapsed += 0.11f;
        }
    }

    IEnumerator ThrowStopBall()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.6f);
        rig.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (isInvisible && tag == "Bat")
        {
            rend.enabled = true;
            isInvisible = false;
        }

        if (tag == "Hit" || tag == "Faul" || tag == "Foul" || tag == "2B" ||
            tag == "3B" || tag == "HR" || tag == "Straik" || tag == "Strike" ||
            tag == "DoubleOut" || tag == "Out")
        {
            ballCount++;

            // GameManager に通知
            if (gameManager != null)
            {
                gameManager.BallThrown();
            }

            if (ballCount >= maxBalls)
            {
                allBallsThrown = true;
            }
        }

        if (allBallsThrown && tag != "Bat")
        {
            allBallsThrown = false;
        }
    }

    public void ResetShoot()
    {
        if (ballCount < maxBalls)
        {
            shootSwitch = true;
            timer = 0.0f;
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.HideAllFeedbackTexts();
        }
    }

    public void ResetGame()
    {
        ballCount = 0;
        shootSwitch = true;
        timer = 0f;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.HideAllFeedbackTexts();
        }

        allBallsThrown = false;
        rend.enabled = true;
        isInvisible = false;

        ResetShoot();
    }

    public void RegisterBallFinished()
    {
        shootSwitch = true;
    }
}
