using UnityEngine;

public static class PersistentGameState
{
    // For now, we store only the grave info.
    public static bool[] graveDug = new bool[3];

    public static int graveCount = 0;

    //  Reset when starting a new game or game victory
    public static void ResetAll()
    {
        graveDug = new bool[3];
        graveCount = 0;
    }
}
