using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NormaChecker : MonoBehaviour
{
    public Text normaText;
    public int currentScore;

    public void CheckNormaClear()
    {
        if (normaText == null) return;

        string sceneName = SceneManager.GetActiveScene().name;
        int requiredScore = GetRequiredScore(sceneName);

        normaText.gameObject.SetActive(true);
        normaText.enabled = true;

        if (currentScore >= requiredScore)
        {
            normaText.text = "ノルマクリア達成！";
            normaText.color = Color.green;
        }
        else
        {
            normaText.text = "ノルマクリア失敗...";
            normaText.color = Color.red;
        }
    }

    private int GetRequiredScore(string sceneName)
    {
        switch (sceneName)
        {
            case "Main": return 1000;
            case "Middle": return 2000;
            case "Hard": return 3000;
            default: return int.MaxValue;
        }
    }
}
