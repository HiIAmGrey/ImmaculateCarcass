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

    [Header("Basic Attack SFX")]
    public AudioClip attackSFX;

    [Header("Spell Settings")]
    public GameObject arcaneBoltPrefab;
    public Transform spellSpawnPoint;
    public int arcaneBoltBaseDamage = 6;

    [Header("Spell SFX")]
    public AudioClip arcaneBoltSFX;

    [Header("Shield Spell")]
    public GameObject arcaneShieldPrefab;
    public float shieldPercentage = 0.08f;

    [Header("Shield SFX")]
    public AudioClip shieldSFX;

    [Header("Guard SFX")]
    public AudioClip guardSFX;

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

        // play attack sound
        if (attackSFX != null)
            AudioManager.Instance.PlaySFX(attackSFX);

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy != null)
        {
            int dmg = attackDamage + Random.Range(-damageVariance, damageVariance + 1);
            if (dmg < 1) dmg = 1;

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

        // play guard sound
        if (guardSFX != null)
            AudioManager.Instance.PlaySFX(guardSFX);

        TurnManager.Instance.EndPlayerTurn();
    }

    public void CastArcaneBolt()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        anim.SetTrigger("SpellAttack");

        // play arcane bolt sound
        if (arcaneBoltSFX != null)
            AudioManager.Instance.PlaySFX(arcaneBoltSFX);

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        if (enemy == null) return;

        int finalDamage = arcaneBoltBaseDamage + PlayerStats.Instance.level;

        GameObject boltObj = Instantiate(arcaneBoltPrefab, spellSpawnPoint.position, Quaternion.identity);
        boltObj.GetComponent<ArcaneBoltProjectile>().Initialize(enemy.transform, finalDamage);

        TurnManager.Instance.EndPlayerTurn();
    }

    public void CastArcaneShield()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        anim.SetTrigger("SpellAttack");

        // play shield sound
        if (shieldSFX != null)
            AudioManager.Instance.PlaySFX(shieldSFX);

        var hp = PlayerHealth.Instance;

        if (hp.arcaneShieldCooldown > 0)
        {
            Debug.Log("Arcane Shield on cooldown!");
            return;
        }

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
