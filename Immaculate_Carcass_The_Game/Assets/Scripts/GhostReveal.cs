using UnityEngine;

public class GhostReveal : MonoBehaviour
{
    [Header("Reveal Trigger")]
    public int[] gravesRequired = { 0, 1, 2 }; 
    // these graves must be dug before this ghost becomes visible

    void Start()
    {
        // If all required graves are dug, myghost should appear.
        // Otherwise, hide it until conditions are met.
        if (!ShouldReveal())
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        // When coming back from combat or reloading scene,
        // check again in case the player just completed the last grave.
        if (ShouldReveal())
            gameObject.SetActive(true);
    }

    private bool ShouldReveal()
    {
        // Check every required grave — if ANY are not dug yet, my ghost stays hidden.
        foreach (int graveID in gravesRequired)
        {
            if (!PersistentGameState.graveDug[graveID])
                return false;
        }

        return true; // all required graves dug
    }
}
