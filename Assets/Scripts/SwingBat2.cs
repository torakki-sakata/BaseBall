using UnityEngine;

public class SwingBat2 : MonoBehaviour
{
    private float rotationSpeed = 15f;
    private float currentRotation = 0f;
    private float maxRotation = 360f;

    public float baseForce = 5000000f;
    private float collisionCooldown = 0.2f;
    private float lastCollisionTime = -10f;

    // バットの上下移動に関する設定
    public float verticalSpeed = 10f;
    private float minY;
    private float maxY;
    private float initialY;

    private Rigidbody batRigidbody;

    void Start()
    {
        GameObject batObject = GameObject.FindGameObjectWithTag("Bat");
        if (batObject != null)
        {
            batRigidbody = batObject.GetComponent<Rigidbody>();
            if (batRigidbody == null)
            {
                Debug.LogWarning("BatにRigidbodyがありません。");
            }
        }
        else
        {
            Debug.LogWarning("Batオブジェクトが見つかりません。");
        }

        // 初期Y座標を基準に上下移動範囲を設定
        initialY = transform.position.y;
        minY = initialY - 0.11f;
        maxY = initialY + 0.05f;
    }

    void Update()
    {
        // 左打者なので逆回転（右回り）
        if (Input.GetMouseButton(0) && currentRotation < maxRotation)
        {
            float rotateStep = Mathf.Min(rotationSpeed, maxRotation - currentRotation);
            transform.Rotate(0, rotateStep, 0); // 右回転
            currentRotation += rotateStep;
        }
        else if (!Input.GetMouseButton(0) && currentRotation > 0)
        {
            float rotateStep = Mathf.Min(rotationSpeed, currentRotation);
            transform.Rotate(0, -rotateStep, 0); // 元に戻す
            currentRotation -= rotateStep;
        }

        // 上下キーでバットを上下移動
        Vector3 position = transform.position;
        if (Input.GetKey(KeyCode.UpArrow) && position.y < maxY)
        {
            position.y += verticalSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && position.y > minY)
        {
            position.y -= verticalSpeed;
        }
        transform.position = position;

        // 手動回転（デバッグ用）
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateBat(10);
        }
    }

    private void RotateBat(float angle)
    {
        transform.Rotate(0, angle, 0);
        Debug.Log($"バットを{angle}度回転しました。");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") && Time.time - lastCollisionTime > collisionCooldown)
        {
            lastCollisionTime = Time.time;

            Rigidbody ballRigidbody = collision.rigidbody;
            if (ballRigidbody == null)
            {
                Debug.LogWarning("Rigidbodyがありません。");
                return;
            }

            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 batCenter = transform.position;
            Vector3 hitDirection = (contactPoint - batCenter).normalized;
            Vector3 batAngularVelocity = transform.right * Mathf.Deg2Rad * rotationSpeed;
            Vector3 velocityAtContact = Vector3.Cross(batAngularVelocity, (contactPoint - batCenter));
            Vector3 combinedDirection = (velocityAtContact + hitDirection).normalized;
            float calculatedForce = baseForce * velocityAtContact.magnitude;

            ballRigidbody.AddForce(combinedDirection * calculatedForce, ForceMode.Impulse);
            Debug.Log($"打ち出し方向: {combinedDirection}, 力: {calculatedForce}");
        }
    }
}
