using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnBallHit : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            audioSource.Play();
        }
    }
}
