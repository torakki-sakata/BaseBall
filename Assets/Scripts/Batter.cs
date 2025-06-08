using Fusion;
using UnityEngine;

public class Batter : NetworkBehaviour
{
    [Networked] private bool IsLeftHanded { get; set; }
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private const float swingDuration = 0.5f;

    private const float swingAngle = 180f; // ← 修正：180度スイング

    private Quaternion initialRotation;
    private float manualRotation = 0f; // Aキーによる回転量

    private float verticalSpeed = 0.01f;
    private float minY;
    private float maxY;
    private float initialY;

    private Ball ball;
    public void SetBall(Ball newBall)
    {
        ball = newBall;
    }

    public override void Spawned()
    {
        Ball ball = FindObjectOfType<Ball>();
        initialRotation = transform.rotation;
        initialY = transform.position.y;
        minY = initialY - 0.1f; // ← 修正：-0.1 に変更
        maxY = initialY + 0.1f; // ← 修正：+0.1 に変更
        ball = FindObjectOfType<Ball>(); // ← これで取得
        if (!Object.HasInputAuthority) return;
        Debug.Log("Batter Spawned");
        if (ball != null)
        {
            SetBall(ball);
            Debug.Log("Ball を Spawned 内でセットしました");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out GameInput input))
        {
            // スイング開始
            if (input.isSwinging && !isSwinging)
            {
                isSwinging = true;
                swingTimer = 0f;

                if (ball != null)
                {
                    Debug.Log($"Ball が存在しています。StateAuthority: {ball.HasStateAuthority}");

                    if (ball.HasStateAuthority)
                    {
                        ball.SetHitter(Runner.LocalPlayer);
                        Debug.Log($"Hitter を設定しました: {Runner.LocalPlayer}");
                    }
                    else
                    {
                        Debug.LogWarning("Ball は存在するが StateAuthority がありません！");
                    }
                }
                else
                {
                    Debug.LogWarning("Ball が null です。");
                }
            }

            // 打席切り替え（位置・回転）
            if (input.changeStance)
            {
                IsLeftHanded = !IsLeftHanded;

                Vector3 newPosition = IsLeftHanded
                    ? new Vector3(32.93f, transform.position.y, 0.36f)
                    : new Vector3(32.93f, transform.position.y, -0.5f);
                transform.position = newPosition;

                float yRotation = IsLeftHanded ? 360f : 0f;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
                initialRotation = transform.rotation;
                manualRotation = 0f; // 回転状態をリセット
            }

            // Aキーでバットを10度回転（打席に応じて方向切替）
            if (input.rotateLeft)
            {
                float direction = IsLeftHanded ? 1f : -1f;
                manualRotation += 10f * direction;
            }

            // 上下移動
            Vector3 pos = transform.position;
            if (input.moveUp && pos.y < maxY)
            {
                pos.y += verticalSpeed;
            }
            else if (input.moveDown && pos.y > minY)
            {
                pos.y -= verticalSpeed;
            }
            transform.position = pos;
        }

        // スイング処理
        if (isSwinging)
        {
            swingTimer += Runner.DeltaTime;

            float direction = IsLeftHanded ? 1f : -1f;
            float angleOffset = Mathf.Sin((swingTimer / swingDuration) * Mathf.PI) * swingAngle * direction;

            transform.rotation = initialRotation * Quaternion.Euler(0f, angleOffset + manualRotation, 0f);

            if (swingTimer >= swingDuration)
            {
                isSwinging = false;
                swingTimer = 0f;
                transform.rotation = initialRotation * Quaternion.Euler(0f, manualRotation, 0f);
            }
        }
        else
        {
            transform.rotation = initialRotation * Quaternion.Euler(0f, manualRotation, 0f);
        }
    }
}
