using UnityEngine;

public class BatCollision : MonoBehaviour
{
    public float baseForce = 100f;
    private float defaultBaseForce;
    private float boostedBaseForce = 150f;
    private float collisionCooldown = 0.4f;
    private float lastCollisionTime = -10f;
    public float heightFactor = 20f;
    public float sideFactor = 2f;
    public float sweatSpotTolerance = 0.4f;
    private bool isBoosted = false;
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        defaultBaseForce = baseForce;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F))
        {
            isBoosted = !isBoosted;
            baseForce = isBoosted ? boostedBaseForce : defaultBaseForce;
            Debug.Log($"打撃力モード: {(isBoosted ? "強化" : "通常")}, baseForce = {baseForce}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") && Time.time - lastCollisionTime > collisionCooldown)
        {
            lastCollisionTime = Time.time;

            Rigidbody ballRb = collision.rigidbody;
            if (ballRb == null)
            {
                Debug.LogWarning("ボールにRigidbodyがありません。");
                return;
            }
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            
            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;
            Vector3 batCenter = transform.position;
            float yOffset = contactPoint.y - batCenter.y;
            float xOffset = contactPoint.x - batCenter.x;
            Vector3 localHitDirection = new Vector3(-1f, yOffset * heightFactor, xOffset * sideFactor).normalized;
            Vector3 worldHitDirection = transform.TransformDirection(localHitDirection);
            float offsetMagnitude = new Vector2(xOffset, yOffset).magnitude;
            float sweetSpotMultiplier = 1f - Mathf.Clamp01(offsetMagnitude / sweatSpotTolerance);
            float finalForce = baseForce * sweetSpotMultiplier;
            ballRb.AddForce(worldHitDirection * finalForce, ForceMode.Impulse);
            Debug.Log($"打球方向: {worldHitDirection}, 力: {finalForce}, X差: {xOffset}, Y差: {yOffset}, スイート率: {sweetSpotMultiplier}");
        }
    }
}
