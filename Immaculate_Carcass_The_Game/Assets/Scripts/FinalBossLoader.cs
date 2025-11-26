using UnityEngine;
using UnityEngine.SceneManagement;

public static class FinalBossLoader
{
    public static void StartBossCombat()
    {
        // Using DialogueManager to show pre-battle dialogue
        DialogueManager.Instance.ShowDialogue(
            () =>
            {
                // This happens AFTER the player finishes reading the dialogue

                // Mark encounter ID for the boss
                EnemyEncounterManager.SetEncounterID(99);

                // Save everything before combat
                PersistentGameState.SaveFromGame();

                // Load combat scene
                SceneManager.LoadScene("CombatScene");
            },

            // Dialogue lines before the final boss battle
            "<size=14><b>The ghost's expression changes...</b></size>",
            "You feel a disturbing presence...",
            "<color=#FF6666>The final battle begins.</color>"
        );
    }
}
