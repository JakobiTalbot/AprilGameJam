using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button startButton;

    private void Awake()
    {
        startButton.Select();
    }

    public void StartButton()
    {
        // load game scene
        SceneManager.LoadScene(2);
    }

    public void QuitButton()
    {
        // quit application
        Application.Quit();
    }
}