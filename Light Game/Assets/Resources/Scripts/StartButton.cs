using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : CustomMenuButton
{
    public override void ButtonAction()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}
