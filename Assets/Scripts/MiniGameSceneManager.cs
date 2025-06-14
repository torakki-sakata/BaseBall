using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGameSceneManager : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("OniHard");
    }
}
