using UnityEngine;
using UnityEngine.SceneManagement;

public class GraveDiggable : MonoBehaviour
{
    private bool playerInRange = false;

    public int graveID = 0;

    public AudioClip digSFX; // play when grave is dug

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // if this grave was already cleared, show the dug version
        if (PersistentGameState.graveDug[graveID])
            SetToDugAppearance();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // already cleared
        if (PersistentGameState.graveDug[graveID])
        {
            UIInteractionPrompt.Instance.ShowPrompt("This grave has already been dug.");
            return;
        }

        // shovel check
        if (!PlayerInventory.Instance.hasShovel)
        {
            UIInteractionPrompt.Instance.ShowPrompt("You need a shovel to dig here.");
            return;
        }

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
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PersistentGameState.graveDug[graveID]) return;

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

        // play dig sound
        if (digSFX != null)
            AudioManager.Instance.PlaySFX(digSFX);

        // not marking the grave as dug here anymore due to poor logic i did earlier
        // (player must actually win the fight first)

        // show dug color right away
        SetToDugAppearance();

        // show dialogue before the fight
        DialogueManager.Instance.ShowDialogue(
            () =>
            {
                // this isn't an overworld AI fight
                PersistentGameState.isOverworldEncounter = false;

                // graves start at encounterID 10
                EnemyEncounterManager.SetEncounterID(10 + graveID);

                // save player pos before combat
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null)
                {
                    PersistentGameState.savedPlayerPos = p.transform.position;
                    PersistentGameState.hasSavedPlayerPos = true;
                }

                PersistentGameState.SaveFromGame();

                // load the grave's combat scene
                SceneManager.LoadScene($"CombatScene_Grave{graveID}");
            },

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
