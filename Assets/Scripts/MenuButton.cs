using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    // Change to main scene
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    // Exit application
    public void ExitGame()
    {
        Application.Quit();
    }
}
