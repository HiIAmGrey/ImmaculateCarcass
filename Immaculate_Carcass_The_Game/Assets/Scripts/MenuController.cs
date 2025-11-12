using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public SceneFader fader;

    public void PlayGame()
    {
         Debug.Log("PlayGame() triggered!");
        if (fader != null)
            fader.FadeToScene("GameScene");
        else
            SceneManager.LoadScene("GameScene"); // fallback if fader not set
    }

    public void QuitGame()
    {   
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
