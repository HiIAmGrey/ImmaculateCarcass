using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Combat Trigger Settings")]
    public float stopBeforeEnemy = 2f;          // how far to stop from enemy visually
    public float combatTriggerDistance = 3f;    // how far away combat should trigger

    private Vector3 targetPosition;
    private bool moving;
    private Animator anim;
    private Transform targetEnemy;

    void Start()
    {
        targetPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detect left click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Clicked an enemy
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetEnemy = hit.collider.transform;

                    // Approach the enemy, but stop slightly before reaching it
                    Vector3 dirToEnemy = (transform.position - targetEnemy.position).normalized;
                    targetPosition = targetEnemy.position + dirToEnemy * stopBeforeEnemy;

                    moving = true;
                    if (anim) anim.SetBool("isMoving", true);
                    Debug.Log("Approaching enemy...");
                    return;
                }

                // Clicked ground
                targetPosition = hit.point;
                targetEnemy = null;
                moving = true;
                if (anim) anim.SetBool("isMoving", true);
            }
        }

        if (moving)
        {
            MovePlayer();
        }

        // Check distance to enemy
        if (targetEnemy != null)
        {
            Collider enemyCollider = targetEnemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                float dist = Vector3.Distance(transform.position, enemyCollider.ClosestPoint(transform.position));
                Debug.Log($"Distance to enemy: {dist:F2}");

                // Trigger combat when close enough 
                if (dist <= combatTriggerDistance)
                {
                    Debug.Log("Reached enemy! Entering combat...");
                    SceneManager.LoadScene("CombatScene");
                }
            }
        }
    }

    void MovePlayer()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

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
