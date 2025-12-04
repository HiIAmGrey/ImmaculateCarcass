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
        // floating damage text
        Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);
        GameObject dmgNum = Instantiate(damageNumberPrefab, CombatManager.Instance.combatCanvas);

        RectTransform canvasRect = CombatManager.Instance.combatCanvas;
        RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out uiPos);
        dmgRect.anchoredPosition = uiPos;

        dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

        // apply real damage
        enemyHealth -= dmg;
        if (enemyHealth < 0)
            enemyHealth = 0;

        // hit animation
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            if (HasParameter(anim, "Hit", AnimatorControllerParameterType.Trigger))
                anim.SetTrigger("Hit");
        }

        // hit sound
        PlayHitSound();

        onEnemyDamaged?.Invoke();

        if (enemyHealth <= 0)
            Die();
    }

    // ============================================
    //   PLAY HIT SOUND
    // ============================================
    private void PlayHitSound()
    {
        if (hitSounds == null || hitSounds.Length == 0)
            return;

        AudioClip chosen = hitSounds[Random.Range(0, hitSounds.Length)];
        AudioManager.Instance.PlaySFX(chosen);
    }

    // ============================================
    //   ENEMY TURN
    // ============================================
    public void TakeTurn()
    {
        if (anim != null)
            anim.SetTrigger("Attack");

        CombatManager.Instance.StartCoroutine(DoDelayedAttack());
    }

    private IEnumerator DoDelayedAttack()
    {
        yield return new WaitForSeconds(0.6f);

        int dmg = enemyDamage + Random.Range(-damageVariance, damageVariance + 1);
        if (dmg < 1) dmg = 1;

        if (Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critMultiplier);
            Debug.Log("Enemy CRITICAL hit!");
        }

        if (PlayerCombat.Instance.isGuarding)
        {
            dmg = Mathf.RoundToInt(dmg * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
        }

        PlayerHealth.Instance.TakeDamage(dmg);
        TurnManager.Instance.EndEnemyTurn();
    }

    // ============================================
    //   DEATH
    // ============================================
    void Die()
    {
        Debug.Log("Enemy died!");
        PlayerStats.Instance.AddXP(5);

        // detect final boss (does NOT break any existing fights)
        BossTag boss = GetComponent<BossTag>();
        bool isFinalBoss = (boss != null && boss.isFinalBoss);

        onEnemyDied?.Invoke();

        // pass final boss flag to CombatManager
        CombatManager.Instance.EnemyDied(this, isFinalBoss);

        Destroy(gameObject);
    }

    // ============================================
    //   TARGET ARROW
    // ============================================
    public void SetTargetArrow(bool on)
    {
        if (targetArrow != null)
            targetArrow.SetActive(on);
    }

    // ============================================
    //   UTILITY
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
