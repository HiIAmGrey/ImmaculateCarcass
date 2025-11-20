using UnityEngine;

public class CanvasSingleton : MonoBehaviour
{
    public static CanvasSingleton Instance;

    void Awake()
    {
        Instance = this;
    }
}
