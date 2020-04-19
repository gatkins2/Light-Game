using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
   /// <summary>
    /// Start button action
    /// </summary>
    public void StartButton()
    {
        // Disable menu buttons
        gameObject.SetActive(false);

        // Go to loading screen room
        GameObject loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        Vector3 position = loadingScreen.transform.position;
        position.z = -10;
        Camera.main.transform.position = position;

        // Start loading scene
        Camera.main.GetComponent<SceneLoader>().LoadScene("Gameplay");
    }

    /// <summary>
    /// Quit button action
    /// </summary>
    public void QuitButton()
    {
        // Quit the game
        Application.Quit();
    }
}
