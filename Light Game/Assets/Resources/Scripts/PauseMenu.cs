using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused { get; private set; }    // Whether the game is paused

    [SerializeField]
    GameObject pauseMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Pause/unpause with escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused)
                Pause();
            else
                Resume();
        }
    }

    // Pause the game
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }


    #region Menu Buttons

    // Unpause the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    // Quit to Menu
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        GameIsPaused = false;
    }

    #endregion
}
