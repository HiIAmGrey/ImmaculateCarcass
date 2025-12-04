using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public void Retry()
    {
        // restore HP
        PersistentGameState.playerCurrentHP = PersistentGameState.playerMaxHP;

        // clear encounter flags so enemy respawns
        PersistentGameState.isOverworldEncounter = false;
        PersistentGameState.encounterID = -1;

        // do NOT touch overworldAIDead[] here
        // Player Lost so Enemy not flagged

        SceneManager.LoadScene("GameScene");
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
