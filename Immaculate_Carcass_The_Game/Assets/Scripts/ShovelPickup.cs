using UnityEngine;

public class ShovelPickup : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        UIInteractionPrompt.Instance.ShowPrompt("Press E to pick up the shovel");
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
            // playergets the shovel
            PlayerInventory.Instance.hasShovel = true;

            // Hide the prompt
            UIInteractionPrompt.Instance.HidePrompt();

            // Show notification
            NotificationManager.Instance.ShowNotification("Shovel acquired!", 1.5f);

            // hide the shovel
            gameObject.SetActive(false);
        }
    }
}
