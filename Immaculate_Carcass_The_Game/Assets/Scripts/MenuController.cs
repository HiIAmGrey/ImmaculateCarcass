using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("World");
    }
    
    public void QuitGame()
    {   
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
