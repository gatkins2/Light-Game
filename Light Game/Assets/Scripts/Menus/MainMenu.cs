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
        // Go to loading screen room
        RoomController rc = Camera.main.GetComponent<RoomController>();
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
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
