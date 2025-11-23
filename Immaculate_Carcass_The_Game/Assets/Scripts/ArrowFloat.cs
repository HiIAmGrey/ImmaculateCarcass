using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public float floatSpeed = 2f;        
    public float floatHeight = 0.15f;    

    float startY;

    void Start()
    {
        startY = transform.localPosition.y;
    }

    void Update()
    {
        // breathing float
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        // face the camera 
        transform.forward = Camera.main.transform.forward;
    }
}
