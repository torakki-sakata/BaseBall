using UnityEngine;

public class RandomLightRotation : MonoBehaviour
{
    private Vector3[] rotations = new Vector3[]
    {
        new Vector3(-49.94f, -206.5f, -2.646f),
        new Vector3(18.961f, -103.2f, -47.18f),
        new Vector3(50f, 30f, 0f)
    };

    void Start()
    {
        ApplyRandomRotation();
    }

    public void ApplyRandomRotation()
    {
        int index = Random.Range(0, rotations.Length);
        transform.eulerAngles = rotations[index];
    }
}
