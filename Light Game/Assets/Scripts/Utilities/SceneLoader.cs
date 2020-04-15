using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    GameObject loadingCanvas;
    [SerializeField]
    Slider slider;   // Loading bar slider

    // Load a scene 
    public void LoadScene(string name)
    {
        StartCoroutine(LoadAsynchronously(name));
    }

    // Bein loading and track progress
    IEnumerator LoadAsynchronously (string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        loadingCanvas.SetActive(true);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
