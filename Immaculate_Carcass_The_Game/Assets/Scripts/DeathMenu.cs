using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    // Called by Retry button
    public void Retry()
    {
        // Restore HP
        PersistentGameState.playerCurrentHP = PersistentGameState.playerMaxHP;

        // Reload the overworld
        SceneManager.LoadScene("GameScene");
    }

    // Called by Quit button
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
