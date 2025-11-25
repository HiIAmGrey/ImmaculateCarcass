using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    void Start()
    {
        // if we already saved a position from before combat
        // spawn the player at that exact spot
        if (PersistentGameState.hasSavedPlayerPos)
        {
            transform.position = PersistentGameState.savedPlayerPos;
        }
        else
        {
            // otherwise spawn at the default starting point
            GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawn != null)
                transform.position = spawn.transform.position;
        }
    }
}
