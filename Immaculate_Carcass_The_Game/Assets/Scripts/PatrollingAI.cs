using UnityEngine;
using UnityEngine.SceneManagement;

public class PatrollingAI : MonoBehaviour
{
    public int aiID = 0; // overworld enemy ID for persistence

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
        // don't respawn dead overworld mobs
        if (PersistentGameState.overworldAIDead[aiID])
        {
            gameObject.SetActive(false);
            return;
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        PickNewPatrolTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case AIState.Patrol: PatrolBehavior(distToPlayer); break;
            case AIState.Waiting: WaitingBehavior(distToPlayer); break;
            case AIState.Chase: ChaseBehavior(distToPlayer); break;
            case AIState.Return: ReturnBehavior(distToPlayer); break;
        }
    }

    // ---------------------------------------------------------------
    // Patrol state
    // ---------------------------------------------------------------
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

    // ---------------------------------------------------------------
    // Waiting
    // ---------------------------------------------------------------
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

    // ---------------------------------------------------------------
    // Chase + combat start
    // ---------------------------------------------------------------
    void ChaseBehavior(float distToPlayer)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * 1.2f * Time.deltaTime
        );

        if (distToPlayer <= engageCombatDistance)
        {
            StartOverworldCombat();
            return;
        }

        if (distToPlayer > stopChaseRange)
        {
            state = AIState.Return;
        }
    }

    // ---------------------------------------------------------------
    // Return to patrol
    // ---------------------------------------------------------------
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

    // ---------------------------------------------------------------
    // Start Combat (THIS IS THE FIXED FUNCTION)
    // ---------------------------------------------------------------
    void StartOverworldCombat()
    {
        Debug.Log($"Overworld encounter triggered by AI {aiID}");

        // mark encounter as overworld fight
        PersistentGameState.isOverworldEncounter = true;

        // ✅ THIS is the correct ID the CombatManager will read
        PersistentGameState.encounterID = aiID;

        // save player position
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            PersistentGameState.savedPlayerPos = p.transform.position;
            PersistentGameState.hasSavedPlayerPos = true;
        }

        PersistentGameState.SaveFromGame();

        // remove overworld AI immediately so it cannot duplicate
        Destroy(gameObject);

        // load the overworld combat scene
        SceneManager.LoadScene("CombatScene_BigUgly");
    }

    // ---------------------------------------------------------------
    // Helper to choose a new random patrol point
    // ---------------------------------------------------------------
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
