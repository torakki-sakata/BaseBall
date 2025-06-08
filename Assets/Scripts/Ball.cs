using UnityEngine;
using Fusion;
using System.Collections;

[RequireComponent(typeof(NetworkTransform), typeof(Rigidbody))]
public class Ball : NetworkBehaviour
{
    [Networked] public PlayerRef Hitter { get; private set; }
    private Vector3 initialPosition = new Vector3(20.95001f, 0.1999969f, -0.07f);

    private Rigidbody rb;
    public Rigidbody Rb => rb;
    private Renderer rend;

    public float speed = 50f;
    public float curveForce = 4f;
    public float forceStraight = 80f;
    public float forceSlow = 60f;
    public int maxBalls = 10;
    private int ballCount = 0;
    private bool isThrown = false;
    private bool isInvisible = false;

    public int BallCount => ballCount;
    public int MaxBalls => maxBalls;

    public void SetHitter(PlayerRef hitter)
    {
        if (HasStateAuthority)
        {
            Hitter = hitter;
            Debug.Log($"Hitter がセットされました: {hitter}");
        }
    }

    public void AddBallCount()
    {
        ballCount = Mathf.Min(ballCount + 1, maxBalls);
    }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (GetInput(out GameInput input))
        {
            if (input.throwBall && !isThrown)
            {
                StartCoroutine(ThrowStraight());
            }
            else if (input.throwSlow && !isThrown)
            {
                StartCoroutine(ThrowSlow());
            }
            else if (input.throwSlider && !isThrown)
            {
                StartCoroutine(ThrowSlider());
            }
            else if (input.throwShoot && !isThrown)
            {
                StartCoroutine(ThrowShoot());
            }
            else if (input.throwInvisible && !isThrown)
            {
                StartCoroutine(ThrowInvisibleBall());
            }
            else if (input.throwStop && !isThrown)
            {
                StartCoroutine(ThrowStopBall());
            }

            if (input.resetBall)
            {
                ResetBall();
            }
        }
    }

    IEnumerator ThrowStraight()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        yield return null;
        ballCount++;
    }

    IEnumerator ThrowSlow()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.right * forceSlow, ForceMode.Impulse);
        yield return null;
        ballCount++;
    }

    IEnumerator ThrowSlider()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        yield return new WaitForSeconds(0.65f);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.01f);
        rb.AddForce(new Vector3(forceStraight * 0.6f, 0, curveForce), ForceMode.Impulse);
        ballCount++;
    }

    IEnumerator ThrowShoot()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        yield return new WaitForSeconds(0.65f);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.01f);
        rb.AddForce(new Vector3(forceStraight * 0.6f, 0, -curveForce), ForceMode.Impulse);
        ballCount++;
    }

    IEnumerator ThrowInvisibleBall()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        yield return new WaitForSeconds(0.45f);
        rend.enabled = false;
        isInvisible = true;
        yield return new WaitForSeconds(0.55f);
        rend.enabled = true;
        isInvisible = false;
        ballCount++;
    }

    IEnumerator ThrowStopBall()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        yield return new WaitForSeconds(0.6f);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);
        rb.AddForce(Vector3.right * forceStraight, ForceMode.Impulse);
        ballCount++;
    }

    public void ResetBall()
    {
        if (!Object.HasStateAuthority) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        isThrown = false;
        rend.enabled = true;
        isInvisible = false;
    }

    private void ResetBallPosition()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
    }
}
