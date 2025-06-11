using UnityEngine;

public class SwingBat : MonoBehaviour
{
    private float rotationSpeed = 14f;
    private float defaultRotationSpeed = 14f;
    private float fullSwingSpeed = 20f;
    private float currentRotation = 0f;
    private float maxRotation = 360f;

    public float baseForce = 5000000f;
    private float collisionCooldown = 0.2f;
    private float lastCollisionTime = -10f;

    public float verticalSpeed = 5000f;
    private float minY;
    private float maxY;
    private float initialY;
    private Rigidbody batRigidbody;
    private bool isFullSwing = false;
    public GameObject SwingText;
    public GameObject FullSwingText;

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

        initialY = transform.position.y;
        minY = initialY - 0.11f;
        maxY = initialY + 0.05f;
        if (SwingText != null) SwingText.SetActive(true);
        if (FullSwingText != null) FullSwingText.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && currentRotation < maxRotation)
        {
            float rotateStep = Mathf.Min(rotationSpeed, maxRotation - currentRotation);
            transform.Rotate(0, -rotateStep, 0);
            currentRotation += rotateStep;
        }
        else if (!Input.GetMouseButton(0) && currentRotation > 0)
        {
            float rotateStep = Mathf.Min(rotationSpeed, currentRotation);
            transform.Rotate(0, rotateStep, 0);
            currentRotation -= rotateStep;
        }

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

        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateBat(10);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
        {
            Vector3 euler = transform.eulerAngles;
            euler.y = 45f;
            transform.eulerAngles = euler;
            currentRotation = 0f;

            Debug.Log("バットの角度をY=45にリセットしました。");

            if (SwingText != null) SwingText.SetActive(true);
            if (FullSwingText != null) FullSwingText.SetActive(false);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
        {
            isFullSwing = !isFullSwing;
            rotationSpeed = isFullSwing ? fullSwingSpeed : defaultRotationSpeed;

            if (SwingText != null) SwingText.SetActive(!isFullSwing);
            if (FullSwingText != null) FullSwingText.SetActive(isFullSwing);

            Debug.Log($"フルスイングモード: {(isFullSwing ? "ON" : "OFF")}");
        }
    }

    private void RotateBat(float angle)
    {
        transform.Rotate(0, -angle, 0);
        Debug.Log($"バットを{-angle}度回転しました。");
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
            Vector3 batAngularVelocity = -transform.right * Mathf.Deg2Rad * rotationSpeed;
            Vector3 velocityAtContact = Vector3.Cross(batAngularVelocity, (contactPoint - batCenter));
            Vector3 combinedDirection = (velocityAtContact + hitDirection).normalized;
            float calculatedForce = baseForce * velocityAtContact.magnitude;
            ballRigidbody.AddForce(combinedDirection * calculatedForce, ForceMode.Impulse);

            Debug.Log($"打ち出し方向: {combinedDirection}, 力: {calculatedForce}");
        }
    }
}
