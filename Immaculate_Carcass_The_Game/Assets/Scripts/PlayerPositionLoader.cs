using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    void Start()
    {
        // if we saved a position earlier spawn there
        if (PersistentGameState.hasSavedPlayerPos)
        {
            transform.position = PersistentGameState.savedPlayerPos;
        }
        else
        {
            // otherwise put player at spawn point for a new run
            GameObject spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawn != null)
                transform.position = spawn.transform.position;
        }
    }
}
