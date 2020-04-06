using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : CustomMenuButton
{
    public override void ButtonAction()
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
}
