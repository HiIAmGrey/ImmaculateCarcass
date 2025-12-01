using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    [Header("Basic Attack")]
    public int attackDamage = 5;
    public bool isGuarding = false;

    [Header("Spell Settings")]
    public GameObject arcaneBoltPrefab;   // prefab for Arcane Bolt
    public Transform spellSpawnPoint;     // empty placed at the staff tip
    public int arcaneBoltBaseDamage = 6;  // base spell damage before scaling

    private Animator anim;                // cached so animation triggers are clean

    void Awake()
    {
        Instance = this;

        // animator lives on MageRoot
        anim = GetComponent<Animator>();

        Debug.Log("PlayerCombat Awake — CombatManager: " + CombatManager.Instance);
    }

    void Start()
    {
        // load saved attack damage
        attackDamage = PersistentGameState.playerAttackDamage;
    }

    //---------------------------------------
    // Basic Melee Attack
    //---------------------------------------
    public void Attack()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        // play melee animation
        anim.SetTrigger("MeleeAttack");

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        Debug.Log("Selected enemy for melee = " + enemy);

        if (enemy != null)
            enemy.TakeDamage(attackDamage);

        TurnManager.Instance.EndPlayerTurn();
    }

    //---------------------------------------
    // Guard (reduces incoming damage this turn)
    //---------------------------------------
    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        isGuarding = true;
        Debug.Log("Player is guarding.");

        TurnManager.Instance.EndPlayerTurn();
    }

    //---------------------------------------
    // ARCANE BOLT (single-target ranged spell)
    //---------------------------------------
    public void CastArcaneBolt()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy == null)
        {
            Debug.Log("Tried to cast Arcane Bolt but no enemy selected!");
            return;
        }

        if (spellSpawnPoint == null)
        {
            Debug.LogError("SpellSpawnPoint is NOT assigned on PlayerCombat!");
            return;
        }

        // simple stat scaling: base + level
        int finalDamage = arcaneBoltBaseDamage + PlayerStats.Instance.level;

        // spawn the projectile at the staff tip
        GameObject boltObj = Instantiate(
            arcaneBoltPrefab,
            spellSpawnPoint.position,
            Quaternion.identity
        );

        ArcaneBoltProjectile bolt = boltObj.GetComponent<ArcaneBoltProjectile>();

        // assign target + damage
        bolt.Initialize(enemy.transform, finalDamage);

        Debug.Log($"Cast Arcane Bolt on {enemy.name} for {finalDamage} damage.");

        TurnManager.Instance.EndPlayerTurn();
    }
}
