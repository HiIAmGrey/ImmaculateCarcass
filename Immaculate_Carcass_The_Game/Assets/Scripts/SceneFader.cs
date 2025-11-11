using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
         SceneManager.LoadScene(sceneName);
    }
    void OnEnable()
{
    // Subscribe to scene load event
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    // Unsubscribe to avoid duplicates
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    StartCoroutine(FadeIn());
}

IEnumerator FadeIn()
{
    float t = fadeDuration;
    Color c = fadeImage.color;
    c.a = 1f;
    fadeImage.color = c;

    while (t > 0f)
    {
        t -= Time.deltaTime;
        c.a = Mathf.Clamp01(t / fadeDuration);
        fadeImage.color = c;
        yield return null;
    }
}

}
