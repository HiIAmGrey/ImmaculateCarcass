using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;
    private Vector3 targetPosition;
    private bool moving;

    private Animator anim;   // we'll use later

    void Start()
    {
        targetPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Left-click to set destination
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
                moving = true;
                if (anim) anim.SetBool("isMoving", true);
            }
        }

        if (moving)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // ignore vertical tilt

            // face the direction of travel
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15f);

            transform.position += direction * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            {
                moving = false;
                if (anim) anim.SetBool("isMoving", false);
            }
        }
    }
}
