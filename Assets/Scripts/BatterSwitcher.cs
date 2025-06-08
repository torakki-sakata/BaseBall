using UnityEngine;

public class BatterSwitcher : MonoBehaviour
{
    public GameObject rightBatter; // 右打者
    public GameObject leftBatter;  // 左打者
    private bool isRightBatterActive = true;

    void Start()
    {
        SetBatterVisibility(true); // 初期表示は右打者
    }

    void Update()
    {
        // Cキーを押したときに切り替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleBatter();
        }
    }

    public void ToggleBatter()
    {
        isRightBatterActive = !isRightBatterActive;
        SetBatterVisibility(isRightBatterActive);
    }

    private void SetBatterVisibility(bool showRightBatter)
    {
        if (rightBatter != null) rightBatter.SetActive(showRightBatter);
        if (leftBatter != null) leftBatter.SetActive(!showRightBatter);
    }
}
