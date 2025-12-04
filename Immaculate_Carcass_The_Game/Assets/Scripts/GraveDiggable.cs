using UnityEngine;
using UnityEngine.SceneManagement;

public class GraveDiggable : MonoBehaviour
{
    private bool playerInRange = false;

    public int graveID = 0;

    // dig sound (played when the grave actually gets dug up)
    public AudioClip digSFX;

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

        // otherwise we're good to dig
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

        // play the dig sound 
        if (digSFX != null)
            AudioManager.Instance.PlaySFX(digSFX);

        // update persistent game state
        PersistentGameState.graveDug[graveID] = true;
        PersistentGameState.graveCount++;

        // visual change
        SetToDugAppearance();

        // digging dialogue BEFORE combat
        DialogueManager.Instance.ShowDialogue(
            () =>
            {
                // After dialogue finishes:

                // Mark this is NOT an overworld AI encounter
                PersistentGameState.isOverworldEncounter = false;

                // Use encounter IDs for graves
                EnemyEncounterManager.SetEncounterID(10 + graveID);

                // Save the player position before combat
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PersistentGameState.savedPlayerPos = player.transform.position;
                    PersistentGameState.hasSavedPlayerPos = true;
                }

                // Save all game data
                PersistentGameState.SaveFromGame();

                // Load unique combat scene for THIS grave
                SceneManager.LoadScene($"CombatScene_Grave{graveID}");
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
