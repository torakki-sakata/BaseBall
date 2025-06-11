using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelect : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("Main");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("Middle");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("Hard");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene("VSMode");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Online");
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Start");
        }
    }
}
