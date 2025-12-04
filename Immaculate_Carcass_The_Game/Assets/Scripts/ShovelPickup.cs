using UnityEngine;

public class ShovelPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public AudioClip shovelPickupSFX;   // sound that plays when grabbing the shovel

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        UIInteractionPrompt.Instance.ShowPrompt("Press E to pick up shovel");
    }

        void Start()
            {
                if (PersistentGameState.hasShovel)
                {
                    // Player already picked this up
                    Destroy(gameObject);
                }
            }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        UIInteractionPrompt.Instance.HidePrompt();
    }

    void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupShovel();
        }
    }

    void PickupShovel()
    {
        //  give the shovel to the player
        PlayerInventory.Instance.hasShovel = true;

        // MAKE IT PERSISTENT ✔
        PersistentGameState.hasShovel = true;

        // play pickup sound 
        if (shovelPickupSFX != null)
            AudioManager.Instance.PlaySFX(shovelPickupSFX);

        // hide interaction prompt
        UIInteractionPrompt.Instance.HidePrompt();

        // remove the shovel object from the world
        Destroy(gameObject);
    }

}
