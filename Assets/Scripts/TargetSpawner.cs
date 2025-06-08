using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject[] targetPrefabs; // ターゲットPrefab（スコア別に複数）
    public float spawnInterval = 3f;   // 出現間隔
    public Vector3 spawnAreaMin;       // 出現範囲（左下奥）
    public Vector3 spawnAreaMax;       // 出現範囲（右上手前）

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTarget();
            timer = 0f;
        }
    }

    void SpawnTarget()
    {
        if (targetPrefabs.Length == 0) return;

        // ランダムなPrefab選択
        GameObject prefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];

        // ランダムな位置
        Vector3 position = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(prefab, position, Quaternion.identity);
    }
}
