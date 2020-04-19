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

    const float minLoadTime = 4f;

    // Load a scene 
    public void LoadScene(string name)
    {
        StartCoroutine(LoadAsynchronously(name));
    }

    // Bein loading and track progress
    IEnumerator LoadAsynchronously (string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        operation.allowSceneActivation = false;
        loadingCanvas.SetActive(true);
        float artificialProgress = 0f;
        float startTime = Time.time;
        
        // Display a smooth loading bar
        while (!operation.isDone && Time.time - startTime < minLoadTime)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            artificialProgress += Time.deltaTime / minLoadTime;
            slider.value = Mathf.Clamp01(Mathf.Min(progress, artificialProgress));
            yield return null;
        }

        // Finish loading scene
        operation.allowSceneActivation = true;
    }
}
