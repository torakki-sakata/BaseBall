using UnityEngine;
using System.Collections.Generic;

public class ShootBall : MonoBehaviour
{
    public GameObject EndText;
    public GameObject RestartButton;
    public GameObject TitleButton;
    public AudioClip[] musicTracksSet1;
    public AudioClip[] musicTracksSet2;
    public AudioClip endTrack;
    public AudioClip throwSound;
    private List<AudioClip[]> allMusicSets;
    private int currentSetIndex = 0;
    private int currentTrackIndex = 0;
    private AudioSource audioSource;
    float timer = 0.0f;
    float timeLimit = 1.0f;
    bool shootSwitch = true;
    float initialForce = 50f;
    float initialForce2 = 35f;
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
        EndText?.SetActive(false);
        RestartButton?.SetActive(false);
        TitleButton?.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        allMusicSets = new List<AudioClip[]>() { musicTracksSet1, musicTracksSet2 };
        PlayCurrentTrack();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeLimit && shootSwitch && ballCount < maxBalls)
        {
            float chosenForce = Random.value < 0.5f ? initialForce : initialForce2;
            rig.AddForce(chosenForce, 0, 0, ForceMode.Impulse);
            timer = 0.0f;
            shootSwitch = false;
            if (audioSource != null && throwSound != null)
            {
                audioSource.PlayOneShot(throwSound);
            }

            ballCount++;
            if (ballCount >= maxBalls)
                allBallsThrown = true;
        }

        if (!allBallsThrown && audioSource != null && allMusicSets.Count > 0)
        {
            AudioClip[] currentTracks = allMusicSets[currentSetIndex];

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentTrackIndex = (currentTrackIndex + 1) % currentTracks.Length;
                PlayCurrentTrack();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentTrackIndex = (currentTrackIndex - 1 + currentTracks.Length) % currentTracks.Length;
                PlayCurrentTrack();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                currentSetIndex = (currentSetIndex + 1) % allMusicSets.Count;
                currentTrackIndex = 0;
                PlayCurrentTrack();
            }
        }
    }

    void PlayCurrentTrack()
    {
        if (audioSource != null && allMusicSets.Count > 0)
        {
            AudioClip[] currentTracks = allMusicSets[currentSetIndex];
            if (currentTracks.Length > 0)
            {
                audioSource.clip = currentTracks[currentTrackIndex];
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (allBallsThrown && IsTargetTag(collision.gameObject.tag))
        {
            NormaChecker checker = FindObjectOfType<NormaChecker>();
            if (checker != null && ScoreManager.Instance != null)
            {
                checker.currentScore = ScoreManager.Instance.score;
                checker.CheckNormaClear();
            }
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
        return tag == "Hit" || tag == "Faul" || tag == "2B" || tag == "3B" || tag == "HR" ||
               tag == "Straik" || tag == "DoubleOut" || tag == "Out";
    }

    public void ResetShoot()
    {
        if (ballCount < maxBalls)
        {
            shootSwitch = true;
            timer = 0.0f;
        }
        ScoreManager.Instance?.HideAllFeedbackTexts();
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
        PlayCurrentTrack();
        ResetShoot();
    }

    public void RegisterBallFinished()
    {
        shootSwitch = true;
    }
}
