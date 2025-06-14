using UnityEngine;
using System.Collections;

public class PitchMini : MonoBehaviour
{
    public GameObject EndText;
    public GameObject RestartButton;
    public GameObject TitleButton;
    public AudioClip[] musicTracks;
    public AudioClip endTrack;
    public AudioClip throwSound;

    public GameObject homingBombPrefab;
    public Vector3 spawnAreaMin = new Vector3(-15f, 0.5f, -5f);
    public Vector3 spawnAreaMax = new Vector3(15f, 0.5f, 15f);

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
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
        audioSource = GetComponent<AudioSource>();
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        shootSwitch = true;
        EndText?.SetActive(false);
        RestartButton?.SetActive(false);
        TitleButton?.SetActive(false);

        if (audioSource != null && musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[currentTrackIndex];
            audioSource.loop = true;
            audioSource.Play();
        }

        SpawnHomingBombs(3);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeLimit && shootSwitch && ballCount < maxBalls)
        {
            float rand = Random.value;

            if (rand < 0.20f)
            {
                rig.AddForce(initialForce, 0, 0, ForceMode.Impulse);
            }
            else if (rand < 0.4f)
            {
                rig.AddForce(initialForce2, 0, 0, ForceMode.Impulse);
            }
            else if (rand < 0.70f)
            {
                StartCoroutine(ThrowSlider());
            }
            else
            {
                StartCoroutine(ThrowShoot());
            }

            timer = 0.0f;
            shootSwitch = false;

            if (audioSource != null && throwSound != null)
            {
                audioSource.PlayOneShot(throwSound);
            }

            ballCount++;

            SpawnHomingBombs(3);

            NotifyHomingBombs_BallHit();

            if (ballCount >= maxBalls)
            {
                allBallsThrown = true;
            }
        }

        if (!allBallsThrown && audioSource != null && musicTracks.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
                audioSource.clip = musicTracks[currentTrackIndex];
                audioSource.Play();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentTrackIndex = (currentTrackIndex - 1 + musicTracks.Length) % musicTracks.Length;
                audioSource.clip = musicTracks[currentTrackIndex];
                audioSource.Play();
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

    void OnCollisionEnter(Collision collision)
    {
        if (allBallsThrown && IsTargetTag(collision.gameObject.tag))
        {
            if (audioSource != null && endTrack != null)
            {
                audioSource.clip = endTrack;
                audioSource.loop = true;
                audioSource.Play();
            }

            EndText?.SetActive(true);
            RestartButton?.SetActive(true);
            TitleButton?.SetActive(true);
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
        ScoreManager.Instance?.HideAllFeedbackTexts();
        NotifyHomingBombs_Reset();
    }

    public void ResetGame()
    {
        ballCount = 0;
        shootSwitch = true;
        timer = 0f;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        EndText?.SetActive(false);
        RestartButton?.SetActive(false);
        TitleButton?.SetActive(false);
        ScoreManager.Instance?.ResetScore();
        ScoreManager.Instance?.HideAllFeedbackTexts();

        allBallsThrown = false;

        if (audioSource != null && musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[currentTrackIndex];
            audioSource.loop = true;
            audioSource.Play();
        }
        ResetShoot();
    }

    public void RegisterBallFinished()
    {
        shootSwitch = true;
    }

    void SpawnHomingBombs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                spawnAreaMin.y,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Instantiate(homingBombPrefab, randomPos, Quaternion.identity);
        }
    }

    void NotifyHomingBombs_BallHit()
    {
        HomingBomb[] bombs = FindObjectsOfType<HomingBomb>();
        foreach (var bomb in bombs)
        {
            bomb.ball = this.transform;
            bomb.OnBallHit();
        }
    }

    void NotifyHomingBombs_Reset()
    {
        HomingBomb[] bombs = FindObjectsOfType<HomingBomb>();
        foreach (var bomb in bombs)
        {
            bomb.OnBallReset();
        }
    }
}
