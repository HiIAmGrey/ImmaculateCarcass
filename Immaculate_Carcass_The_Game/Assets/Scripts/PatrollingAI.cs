using UnityEngine;
using UnityEngine.SceneManagement;
public class PatrollingAI : MonoBehaviour
{
    public float speed = 3f;
    public float patrolRadius = 5f;
    public float waitTime = 2f;
    public float chaseRange = 5f;     // distance at which AI begins chasing
    public float stopChaseRange = 8f; // distance at which AI gives up chasing

    public float engageCombatDistance = 2f; // aggro range for AI
    private Vector3 patrolTarget;
    private float waitTimer;

    private Transform player;

    private enum AIState { Patrol, Waiting, Chase, Return }
    private AIState state = AIState.Patrol;
    private Vector3 lastPatrolPoint;
    void Start()
    {
    GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p == null)
            {
                Debug.LogError("⚠️ ERROR: No GameObject tagged 'Player' found in scene!");
            }
            else
            {
                player = p.transform;
            }
        PickNewPatrolTarget();
    }

    void Update()
    {    
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case AIState.Patrol:
                PatrolBehavior(distToPlayer);
                break;

            case AIState.Waiting:
                WaitingBehavior(distToPlayer);
                break;

            case AIState.Chase:
                ChaseBehavior(distToPlayer);
                break;
            case AIState.Return:
                ReturnBehavior(distToPlayer);
                break;
        }
    }

    // ─────────────────────────────────────────────
    // Patrol State
    // ─────────────────────────────────────────────
    void PatrolBehavior(float distToPlayer)
    {
        // Chase if player is close enough
        if (distToPlayer <= chaseRange)
        {
            state = AIState.Chase;
            return;
        }

        // Move toward patrol target
        transform.position = Vector3.MoveTowards(
            transform.position,
            patrolTarget,
            speed * Time.deltaTime
        );

        // Reached patrol point → wait
        if (Vector3.Distance(transform.position, patrolTarget) < 0.3f)
        {
            waitTimer = waitTime;
            state = AIState.Waiting;
        }
    }

    // ─────────────────────────────────────────────
    // Waiting State
    // ─────────────────────────────────────────────
    void WaitingBehavior(float distToPlayer)
    {
        if (distToPlayer <= chaseRange)
        {
            state = AIState.Chase;
            return;
        }

        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            PickNewPatrolTarget();
            state = AIState.Patrol;
        }
    }

    // ─────────────────────────────────────────────
    // Chase State
    // ─────────────────────────────────────────────
    void ChaseBehavior(float distToPlayer)
    {
        // Move toward player
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * 1.2f * Time.deltaTime // chase slightly faster
            
        );
        Debug.Log("Chasing player!");

        // If close enough, trigger combat scene
        if (distToPlayer <= engageCombatDistance)
        {
            Debug.Log("Enemy reached player — loading combat scene!");
            SceneManager.LoadScene("CombatScene");   
            return;
        }
    
        // If player gets far enough away, stop chasing
       if (distToPlayer > stopChaseRange)
      {
        Debug.Log("Player escaped — returning to patrol point.");
        state = AIState.Return;
      }
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────
    void PickNewPatrolTarget()
    {
        Vector2 circle = Random.insideUnitCircle * patrolRadius;

        patrolTarget = new Vector3(
            transform.position.x + circle.x,
            transform.position.y,
            transform.position.z + circle.y
        );
        lastPatrolPoint = patrolTarget;
    }
    void ReturnBehavior(float distToPlayer)
    {
        // If player re-enters chase range during return, chase again
        if (distToPlayer <= chaseRange)
        {
            state = AIState.Chase;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            lastPatrolPoint,
            speed * Time.deltaTime
        );

        // Once the enemy reaches the saved patrol point, resume patrol
        if (Vector3.Distance(transform.position, lastPatrolPoint) < 0.3f)
        {
            state = AIState.Patrol;
        }
    }
}