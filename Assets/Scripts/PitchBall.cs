using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PitchBall : MonoBehaviour
{
    float timer = 0.0f;
    bool shootSwitch = true;
    bool autoPitchTriggered = false;

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

    public GameObject pitcherObject;
    private Renderer pitcherRenderer;
    private Color originalColor;

    public Text timerText;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        shootSwitch = true;

        gameManager = FindObjectOfType<GameManager>();

        if (pitcherObject != null)
        {
            pitcherRenderer = pitcherObject.GetComponent<Renderer>();
            if (pitcherRenderer != null)
            {
                originalColor = pitcherRenderer.material.color;
            }
        }
    }

    void Update()
    {
        if (!shootSwitch || ballCount >= maxBalls) return;

        timer += Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = $"投球まで: {Mathf.Max(0, 5 - timer):0.0} 秒";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            TriggerManualPitch(() => rig.AddForce(initialForce, 0, 0, ForceMode.Impulse), Color.green);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            TriggerManualPitch(() => rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse), Color.green);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            TriggerManualPitch(() => StartCoroutine(ThrowSlider()), Color.green);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            TriggerManualPitch(() => StartCoroutine(ThrowShoot()), Color.green);
        else if (Input.GetKeyDown(KeyCode.V))
            TriggerManualPitch(() => StartCoroutine(ThrowInvisibleBall()), Color.red);
        else if (Input.GetKeyDown(KeyCode.F))
            TriggerManualPitch(() => StartCoroutine(ThrowFireball()), Color.red);
        else if (Input.GetKeyDown(KeyCode.I))
            TriggerManualPitch(() => StartCoroutine(ThrowIllusionBall()), Color.red);
        else if (Input.GetKeyDown(KeyCode.S))
            TriggerManualPitch(() => StartCoroutine(ThrowStopBall()), Color.red);

        if (timer >= 5f && !autoPitchTriggered)
        {
            autoPitchTriggered = true;

            int random = Random.Range(1, 5); // 1〜4（速球・遅球・スライダー・シュート）
            switch (random)
            {
                case 1:
                    TriggerManualPitch(() => rig.AddForce(initialForce, 0, 0, ForceMode.Impulse), Color.yellow);
                    break;
                case 2:
                    TriggerManualPitch(() => rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse), Color.yellow);
                    break;
                case 3:
                    TriggerManualPitch(() => StartCoroutine(ThrowSlider()), Color.yellow);
                    break;
                case 4:
                    TriggerManualPitch(() => StartCoroutine(ThrowShoot()), Color.yellow);
                    break;
            }
        }
    }

    void TriggerManualPitch(System.Action pitchAction, Color color)
    {
        if (!shootSwitch) return;

        ChangePitcherColor(color);
        StartCoroutine(DelayedThrow(pitchAction));
    }

    IEnumerator DelayedThrow(System.Action throwAction)
    {
        shootSwitch = false;
        yield return new WaitForSeconds(1.0f);
        throwAction.Invoke();
        timer = 0f;
        autoPitchTriggered = false;
    }

    void ChangePitcherColor(Color color)
    {
        if (pitcherRenderer != null)
        {
            pitcherRenderer.material.color = color;
        }
    }

    void ResetPitcherColor()
    {
        if (pitcherRenderer != null)
        {
            pitcherRenderer.material.color = originalColor;
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
            ResetPitcherColor();

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
            autoPitchTriggered = false;
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
        autoPitchTriggered = false;

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

        ResetPitcherColor();
        ResetShoot();
    }

    public void RegisterBallFinished()
    {
        shootSwitch = true;
    }
}
