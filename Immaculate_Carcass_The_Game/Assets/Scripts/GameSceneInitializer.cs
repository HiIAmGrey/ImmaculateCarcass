using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Awake()
    {
        // load all saved data BEFORE the scene finishes booting
        PersistentGameState.LoadIntoGame();

        Debug.Log("GameSceneInitializer: Loaded game state into scene.");
    }

    void Start()
    {
        // place player back where they were (if applicable)
        if (PersistentGameState.hasSavedPlayerPos)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = PersistentGameState.savedPlayerPos;
            }
        }
    }
}
