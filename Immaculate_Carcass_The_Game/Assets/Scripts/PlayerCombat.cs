using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    [Header("Basic Attack")]
    public int attackDamage = 5;      // base damage (level-up increases this)
    public int damageVariance = 2;    // damage will be +/- this
    public float critChance = 0.10f;  // 10% crit chance
    public float critMultiplier = 1.5f;
    public bool isGuarding = false;

    [Header("Spell Settings")]
    public GameObject arcaneBoltPrefab;
    public Transform spellSpawnPoint;
    public int arcaneBoltBaseDamage = 6;
    [Header("Spell Variance")]
    public int spellVariance = 2;
    public float spellCritChance = 0.08f;
    public float spellCritMultiplier = 1.6f;

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
        // Load the leveled attack stat
        attackDamage = PersistentGameState.playerAttackDamage;
    }

    public void Attack()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        anim.SetTrigger("MeleeAttack");

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy != null)
        {
            // Damage variance
            int dmg = attackDamage + Random.Range(-damageVariance, damageVariance + 1);
            if (dmg < 1) dmg = 1;

            // Critical hit
            if (Random.value < critChance)
            {
                dmg = Mathf.RoundToInt(dmg * critMultiplier);
                Debug.Log("PLAYER CRITICAL HIT!");
            }

            enemy.TakeDamage(dmg);
        }

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

            // Base damage including level scaling
            int dmg = arcaneBoltBaseDamage + PlayerStats.Instance.level;

            // Apply spell variance
            dmg += Random.Range(-spellVariance, spellVariance + 1);
            if (dmg < 1) dmg = 1;

            // Spell critical hit
            if (Random.value < spellCritChance)
            {
                dmg = Mathf.RoundToInt(dmg * spellCritMultiplier);
                Debug.Log("SPELL CRITICAL HIT!");
            }

            GameObject boltObj = Instantiate(arcaneBoltPrefab, spellSpawnPoint.position, Quaternion.identity);
            boltObj.GetComponent<ArcaneBoltProjectile>().Initialize(enemy.transform, dmg);

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
