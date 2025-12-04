using UnityEngine;
using UnityEngine.SceneManagement;

public class PatrollingAI : MonoBehaviour
{
    public int aiID = 0; // used for persistent death tracking

    public float speed = 3f;
    public float patrolRadius = 5f;
    public float waitTime = 2f;
    public float chaseRange = 5f;
    public float stopChaseRange = 8f;

    public float engageCombatDistance = 2f;

    private Vector3 patrolTarget;
    private float waitTimer;
    private Transform player;

    private enum AIState { Patrol, Waiting, Chase, Return }
    private AIState state = AIState.Patrol;

    private Vector3 lastPatrolPoint;

    void Start()
    {
        // Prevent respawning dead enemies
        if (PersistentGameState.overworldAIDead[aiID])
        {
            gameObject.SetActive(false);
            return;
        }

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
        if (player == null)
            return;

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
        if (distToPlayer <= chaseRange)
        {
            state = AIState.Chase;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            patrolTarget,
            speed * Time.deltaTime
        );

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
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * 1.2f * Time.deltaTime
        );

        Debug.Log("Chasing player!");

       if (distToPlayer <= engageCombatDistance)
                {
                    Debug.Log("Enemy reached player — loading combat scene!");

                    // Save player position before combat
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    if (playerObj != null)
                    {
                        PersistentGameState.savedPlayerPos = playerObj.transform.position;
                        PersistentGameState.hasSavedPlayerPos = true;
                    }

                    // Mark this overworld enemy as permanently dead
                    PersistentGameState.overworldAIDead[aiID] = true;
                    PersistentGameState.SaveFromGame();

                    // Pass encounter ID to combat
                    PersistentGameState.isOverworldEncounter = true;
                    EnemyEncounterManager.SetEncounterID(aiID);

                    // Destroy this overworld object so it never comes back
                    Destroy(gameObject, 0.1f);

                    // Load the correct combat scene
                    SceneManager.LoadScene("CombatScene_BigUgly");
                    return;
                }


        if (distToPlayer > stopChaseRange)
        {
            Debug.Log("Player escaped — returning to patrol point.");
            state = AIState.Return;
        }
    }

    // ─────────────────────────────────────────────
    // Return State
    // ─────────────────────────────────────────────
    void ReturnBehavior(float distToPlayer)
    {
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

        if (Vector3.Distance(transform.position, lastPatrolPoint) < 0.3f)
        {
            state = AIState.Patrol;
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
}
