using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool hasShovel = false;

    void Awake()
    {
        Instance = this;
    }

    
}
