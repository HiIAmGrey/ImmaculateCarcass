using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int enemyHealth = 20;

    [Header("Damage Settings")]
    public int enemyDamage = 3;          // base damage
    public int damageVariance = 1;       // +/- this much on hit
    public float critChance = 0.05f;     // chance to crit
    public float critMultiplier = 1.4f;  // crit damage multiplier

    [Header("Hit Sound Settings")]
    // These are the custom hit sounds you assign per enemy prefab.
    // Slime gets wet/splat sounds, ghost gets airy/echo sounds, golem gets rock impacts, etc.
    public AudioClip[] hitSounds;

    [Header("UI / Visuals")]
    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;
    public GameObject targetArrow;

    public System.Action onEnemyDamaged;
    public System.Action onEnemyDied;

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // ============================================
    //   TAKING DAMAGE
    // ============================================
    public void TakeDamage(int dmg)
    {
        // Floating damage number setup
        Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);
        GameObject dmgNum = Instantiate(damageNumberPrefab, CombatManager.Instance.combatCanvas);

        RectTransform canvasRect = CombatManager.Instance.combatCanvas;
        RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out uiPos);
        dmgRect.anchoredPosition = uiPos;

        dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

        // Apply real damage
        enemyHealth -= dmg;
        if (enemyHealth < 0)
            enemyHealth = 0;

        // Play hit animation if this enemy has one
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            if (HasParameter(anim, "Hit", AnimatorControllerParameterType.Trigger))
                anim.SetTrigger("Hit");
        }

        // Play hit sound (each enemy prefab has its own custom list)
        PlayHitSound();

        // Notify UI or combat manager
        onEnemyDamaged?.Invoke();

        if (enemyHealth <= 0)
            Die();
    }

    // ============================================
    //   PLAY HIT SOUND
    // ============================================
    private void PlayHitSound()
    {
        // No sound assigned? Just skip.
        if (hitSounds == null || hitSounds.Length == 0)
            return;

        // Pick a random clip from the list
        AudioClip chosen = hitSounds[Random.Range(0, hitSounds.Length)];

        // Play the clip through the global SFX channel
        AudioManager.Instance.PlaySFX(chosen);
    }

    // ============================================
    //   ENEMY TURN
    // ============================================
    public void TakeTurn()
    {
        // Play attack animation
        if (anim != null)
            anim.SetTrigger("Attack");

        CombatManager.Instance.StartCoroutine(DoDelayedAttack());
    }

    private IEnumerator DoDelayedAttack()
    {
        // Delay to sync with animation impact
        yield return new WaitForSeconds(0.6f);

        // Damage variance
        int dmg = enemyDamage + Random.Range(-damageVariance, damageVariance + 1);
        if (dmg < 1) dmg = 1;

        // Critical hit
        if (Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critMultiplier);
            Debug.Log("Enemy CRITICAL hit!");
        }

        // Guard reduces incoming damage
        if (PlayerCombat.Instance.isGuarding)
        {
            dmg = Mathf.RoundToInt(dmg * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
        }

        // Give damage to player
        PlayerHealth.Instance.TakeDamage(dmg);

        // End enemy turn
        TurnManager.Instance.EndEnemyTurn();
    }

    // ============================================
    //   DEATH
    // ============================================
    void Die()
    {
        Debug.Log("Enemy died!");
        PlayerStats.Instance.AddXP(5);

        onEnemyDied?.Invoke();
        CombatManager.Instance.EnemyDied(this);

        Destroy(gameObject); // clean up object
    }

    // ============================================
    //   TARGET ARROW TOGGLE
    // ============================================
    public void SetTargetArrow(bool on)
    {
        if (targetArrow != null)
            targetArrow.SetActive(on);
    }

    // ============================================
    //   UTILITY: Check Animator Params
    // ============================================
    private bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == type && param.name == paramName)
                return true;
        }
        return false;
    }
}
