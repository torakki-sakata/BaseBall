using UnityEngine;

public class TargetObject : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) // Ball タグのボールが当たったら
        {
            Destroy(gameObject); // 的を消す
        }
    }
}
