using UnityEngine;
using System.Collections;

public class ShootBall3 : MonoBehaviour
{
    public GameObject EndText;
    public GameObject RestartButton;
    public GameObject TitleButton;
    float timer = 0.0f;
    float timeLimit = 1.0f;
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

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        shootSwitch = true;

        if (EndText) EndText.SetActive(false);
        if (RestartButton) RestartButton.SetActive(false);
        if (TitleButton) TitleButton.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeLimit && shootSwitch && ballCount < maxBalls)
        {
            float rand = Random.value;

            if (rand < 0.05f)
            {
                rig.AddForce(initialForce, 0, 0, ForceMode.Impulse); // 速球
            }
            else if (rand < 0.1f)
            {
                rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse); // 遅球
            }
            else if (rand < 0.15f)
            {
                StartCoroutine(ThrowSlider());
            }
            else if (rand < 0.2f)
            {
                StartCoroutine(ThrowShoot());
            }
            else if (rand < 0.4f)
            {
                StartCoroutine(ThrowInvisibleBall());
            }
            else if (rand < 0.6f)
            {
                StartCoroutine(ThrowFireball());
            }
            else if (rand < 0.7f)
            {
                StartCoroutine(ThrowIllusionBall());
            }
            else
            {
                StartCoroutine(ThrowStopBall());
            }

            timer = 0.0f;
            shootSwitch = false;
            ballCount++;

            if (ballCount >= maxBalls)
            {
                allBallsThrown = true;
            }
        }
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
        yield return new WaitForSeconds(0.45f); // 0.35秒後に不可視化
        rend.enabled = false;                   // レンダラーだけを非表示にする
        isInvisible = true;                     // 任意で他の処理にも使えるように
        yield return new WaitForSeconds(0.55f); // 0.65秒後に再表示（合計1.0秒）
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
        isInvisible = true;

        float elapsed = 0f;
        float blinkInterval = 0.2f;

        while (isInvisible && elapsed < 4.0f)
        {
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        rend.enabled = true;  // 念のため可視状態で終了
        isInvisible = false;
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
        if (isInvisible && collision.gameObject.CompareTag("Bat"))
        {
            rend.enabled = true;
            isInvisible = false;
        }
        
        // 最後のボールが指定されたタグのいずれかに当たったらUIを表示
        if (allBallsThrown && IsTargetTag(collision.gameObject.tag))
        {
            if (EndText) EndText.SetActive(true);
            if (RestartButton) RestartButton.SetActive(true);
            if (TitleButton) TitleButton.SetActive(true);

            allBallsThrown = false; // UIは1回だけ表示
        }
    }

    // 指定されたタグのいずれかかどうかをチェックするヘルパー関数
    private bool IsTargetTag(string tag)
    {
        return tag == "Hit" ||
            tag == "Faul" ||    // ※必要に応じて "Foul" に修正
            tag == "2B" ||
            tag == "3B" ||
            tag == "HR" ||
            tag == "Straik" ||  // ※必要に応じて "Strike" に修正
            tag == "DoubleOut" ||
            tag == "Out";
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

        if (EndText) EndText.SetActive(false);
        if (RestartButton) RestartButton.SetActive(false);
        if (TitleButton) TitleButton.SetActive(false);

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
