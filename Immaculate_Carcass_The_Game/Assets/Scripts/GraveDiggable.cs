using UnityEngine;
using UnityEngine.SceneManagement;

public class GraveDiggable : MonoBehaviour
{
    private bool playerInRange = false;

    public int graveID = 0;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Restore appearance if grave was already dug
        if (PersistentGameState.graveDug[graveID])
        {
            SetToDugAppearance();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Grave already dug
        if (PersistentGameState.graveDug[graveID])
        {
            UIInteractionPrompt.Instance.ShowPrompt("This grave has already been dug.");
            return;
        }

        // Shovel required
        if (!PlayerInventory.Instance.hasShovel)
        {
            UIInteractionPrompt.Instance.ShowPrompt("You need a shovel to dig here.");
            return;
        }

        // Otherwise ready to dig
        playerInRange = true;
        UIInteractionPrompt.Instance.ShowPrompt("Press E to dig");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        UIInteractionPrompt.Instance.HidePrompt();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (PersistentGameState.graveDug[graveID])
                return;

            if (!PlayerInventory.Instance.hasShovel)
            {
                NotificationManager.Instance.ShowNotification("You have no shovel!", 1.5f);
                return;
            }

            DigGrave();
        }
    }

    void DigGrave()
    {
        UIInteractionPrompt.Instance.HidePrompt();

        // Update persistent memory
        PersistentGameState.graveDug[graveID] = true;
        PersistentGameState.graveCount++;

        // Visual change
        SetToDugAppearance();

        // Play dialogue BEFORE combat
        DialogueManager.Instance.ShowDialogue(
            () =>
            {
                // This code runs AFTER the player presses SPACE to finish reading

                // Graves use encounter IDs 10+
                PersistentGameState.isOverworldEncounter = false;
                EnemyEncounterManager.SetEncounterID(10 + graveID);

                // Save the player’s current position BEFORE combat
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PersistentGameState.savedPlayerPos = player.transform.position;
                    PersistentGameState.hasSavedPlayerPos = true;
                }

                // Save everything else
                PersistentGameState.SaveFromGame();

                // Enter combat
                SceneManager.LoadScene("CombatScene");
            },

            // Dialogue lines
            "You disturb the silent soil...",
            "Something stirs beneath the grave..."
        );
    }

    private void SetToDugAppearance()
    {
        if (rend != null)
            rend.material.color = Color.gray;
    }
}
