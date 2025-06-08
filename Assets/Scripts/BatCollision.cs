using UnityEngine;

public class BatCollision : MonoBehaviour
{
    public float baseForce = 3000f;
    private float collisionCooldown = 0.4f;
    private float lastCollisionTime = -10f;

    public float heightFactor = 20f;
    public float sideFactor = 2f;
    public float sweatSpotTolerance = 0.4f;

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

            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;
            Vector3 batCenter = transform.position;

            float yOffset = contactPoint.y - batCenter.y;
            float xOffset = contactPoint.x - batCenter.x;

            // ローカル空間の方向ベクトル（打球方向 = バットのローカル -X軸）
            Vector3 localHitDirection = new Vector3(-1f, yOffset * heightFactor, xOffset * sideFactor).normalized;

            // ローカル→ワールド変換（バットの向きを考慮）
            Vector3 worldHitDirection = transform.TransformDirection(localHitDirection);

            float offsetMagnitude = new Vector2(xOffset, yOffset).magnitude;
            float sweetSpotMultiplier = 1f - Mathf.Clamp01(offsetMagnitude / sweatSpotTolerance);

            float finalForce = baseForce * sweetSpotMultiplier;

            ballRb.AddForce(worldHitDirection * finalForce, ForceMode.Impulse);

            Debug.Log($"打球方向: {worldHitDirection}, 力: {finalForce}, X差: {xOffset}, Y差: {yOffset}, スイート率: {sweetSpotMultiplier}");
        }
    }
}
