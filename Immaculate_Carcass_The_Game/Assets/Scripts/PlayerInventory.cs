using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Inventory Flags")]
    public bool hasShovel = false;

    void Awake()
    {
        // global ref for persist
        Instance = this;

        // load shovel state from persist system
        hasShovel = PersistentGameState.hasShovel;
    }

    // call this from ShovelPickup
    public void PickUpShovel()
    {
        hasShovel = true;

        // save shovel to persist system
        PersistentGameState.hasShovel = true;

        Debug.Log("Shovel acquired! (persistent)");
    }
}
