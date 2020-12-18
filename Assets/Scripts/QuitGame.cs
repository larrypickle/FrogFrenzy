using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void GameQuit()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
}
