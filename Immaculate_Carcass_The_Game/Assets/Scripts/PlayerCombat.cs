using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    [Header("Basic Attack")]
    public int attackDamage = 5;
    public bool isGuarding = false;

    [Header("Spell Settings")]
    public GameObject arcaneBoltPrefab;
    public Transform spellSpawnPoint;
    public int arcaneBoltBaseDamage = 6;

    [Header("Shield Spell")]
    public GameObject arcaneShieldPrefab;
    public float shieldPercentage = 0.08f;

    private GameObject activeShield;
    private Animator anim;

    void Awake()
    {
        Instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        attackDamage = PersistentGameState.playerAttackDamage;
    }

    public void Attack()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        anim.SetTrigger("MeleeAttack");

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy != null)
            enemy.TakeDamage(attackDamage);

        TurnManager.Instance.EndPlayerTurn();
    }

    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        isGuarding = true;
        TurnManager.Instance.EndPlayerTurn();
    }

    public void CastArcaneBolt()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy == null) return;

        anim.SetTrigger("SpellAttack");

        int finalDamage = arcaneBoltBaseDamage + PlayerStats.Instance.level;

        GameObject boltObj = Instantiate(arcaneBoltPrefab, spellSpawnPoint.position, Quaternion.identity);
        boltObj.GetComponent<ArcaneBoltProjectile>().Initialize(enemy.transform, finalDamage);

        TurnManager.Instance.EndPlayerTurn();
    }

    public void CastArcaneShield()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn) return;

        var hp = PlayerHealth.Instance;

        if (hp.arcaneShieldCooldown > 0)
        {
            Debug.Log("Arcane Shield on cooldown!");
            return;
        }

        anim.SetTrigger("SpellAttack");

        int shieldValue = Mathf.RoundToInt(hp.maxHealth * shieldPercentage);
        hp.shieldAmount = shieldValue;
        hp.shieldTurnsRemaining = 2;
        hp.arcaneShieldCooldown = 3;

        if (activeShield != null)
            Destroy(activeShield);

        activeShield = Instantiate(arcaneShieldPrefab, transform.position, Quaternion.identity, transform);

        Debug.Log($"Shield applied: {shieldValue} for 2 turns.");

        TurnManager.Instance.EndPlayerTurn();
    }

    public void DestroyShieldFX()
    {
        if (activeShield != null)
            Destroy(activeShield);

        activeShield = null;
    }
}
