using UnityEngine;

public class ChangeStadium : MonoBehaviour
{
    public GameObject Koshien;
    public GameObject Zinguu;
    public GameObject Escon;

    private int stadiumIndex = 0; // 0: Koshien, 1: Zinguu, 2: Escon

    void Start()
    {
        UpdateStadiumVisibility();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleStadium();
        }
    }

    public void ToggleStadium()
    {
        stadiumIndex = (stadiumIndex + 1) % 3; // 0 → 1 → 2 → 0 → ...
        UpdateStadiumVisibility();
    }

    private void UpdateStadiumVisibility()
    {
        if (Koshien != null) Koshien.SetActive(stadiumIndex == 0);
        if (Zinguu != null) Zinguu.SetActive(stadiumIndex == 1);
        if (Escon != null) Escon.SetActive(stadiumIndex == 2);
    }
}
