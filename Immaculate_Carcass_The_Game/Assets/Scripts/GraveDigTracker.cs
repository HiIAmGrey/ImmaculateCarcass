public static class GraveDigTracker
{
    public static int dugCount
    {
        get { return PersistentGameState.graveCount; }
        set { PersistentGameState.graveCount = value; }
    }

    public static void Reset()
    {
        PersistentGameState.graveCount = 0;
    }
}
