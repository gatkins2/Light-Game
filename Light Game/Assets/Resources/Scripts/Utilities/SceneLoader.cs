using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load a scene 
    public void LoadScene(string name)
    {
        StartCoroutine(LoadAsynchronously(name));
    }

    // Bein loading and track progress
    IEnumerator LoadAsynchronously (string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);
            yield return null;
        }
    }
}
