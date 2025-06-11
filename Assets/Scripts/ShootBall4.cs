using UnityEngine;
using System.Collections;

public class ShootBall4 : MonoBehaviour
{
    public GameObject EndText;
    public GameObject RestartButton;
    public GameObject TitleButton;
    public GameObject LoadingPanel;

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

    private bool isReadyToShoot = false;
    private bool canStartThrowing = false;

    public int GetBallCount()
    {
        return ballCount;
    }

    public int GetMaxBalls()
    {
        return maxBalls;
    }

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

        StartCoroutine(WaitForLoadingToDisappear());
    }

    IEnumerator WaitForLoadingToDisappear()
    {
        while (LoadingPanel != null && LoadingPanel.activeSelf)
        {
            yield return null;
        }
        isReadyToShoot = true;
    }

    public void StartThrowing()
    {
        canStartThrowing = true;
    }

    void Update()
    {
        if (!isReadyToShoot || !canStartThrowing)
            return;

        timer += Time.deltaTime;

        if (timer > timeLimit && shootSwitch && ballCount < maxBalls)
        {
            float rand = Random.value;

            if (rand < 0.05f)
            {
                rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
            }
            else if (rand < 0.1f)
            {
                rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse);
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
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ThrowShoot()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.65f);
        rig.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.01f);
        rig.AddForce(initialForce * 0.6f, 0, -curveForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(0.5f);
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

        rend.enabled = true;
        isInvisible = false;
    }

    IEnumerator ThrowStopBall()
    {
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.6f);
        rig.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);
        rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isInvisible && collision.gameObject.CompareTag("Bat"))
        {
            rend.enabled = true;
            isInvisible = false;
        }

        if (allBallsThrown && IsTargetTag(collision.gameObject.tag))
        {
            if (EndText) EndText.SetActive(true);
            if (RestartButton) RestartButton.SetActive(true);
            if (TitleButton) TitleButton.SetActive(true);

            allBallsThrown = false;
        }
    }

    private bool IsTargetTag(string tag)
    {
        return tag == "Hit" ||
               tag == "Faul" ||
               tag == "2B" ||
               tag == "3B" ||
               tag == "HR" ||
               tag == "Straik" ||
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
