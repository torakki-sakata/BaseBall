using UnityEngine;
using System.Collections;

public class ShootBall2 : MonoBehaviour
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

    void Start()
    {
        rig = GetComponent<Rigidbody>();
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

            if (rand < 0.20f)
            {
                // ストレート
                rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
            }
            else if (rand < 0.4f)
            {
                // 遅いストレート
                rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse);
            }
            else if (rand < 0.70f)
            {
                // スライダー
                StartCoroutine(ThrowSlider());
            }
            else
            {
                // シュート
                StartCoroutine(ThrowShoot());
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
        // 初速
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);

        yield return new WaitForSeconds(0.65f); // 少し進んで

        rig.velocity = Vector3.zero; // 0.01秒停止
        yield return new WaitForSeconds(0.01f);

        // 右カーブ
        rig.AddForce(initialForce * 0.6f, 0, curveForce, ForceMode.Impulse);
    }

    IEnumerator ThrowShoot()
    {
        // 初速
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);

        yield return new WaitForSeconds(0.65f); // 少し進んで

        rig.velocity = Vector3.zero; // 0.01秒停止
        yield return new WaitForSeconds(0.01f);

        // 左カーブ
        rig.AddForce(initialForce * 0.6f, 0, -curveForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
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
            tag == "Out" ||
            tag == "None";
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

        ResetShoot();
    }

    public void RegisterBallFinished()
    {
        shootSwitch = true;
    }
}
