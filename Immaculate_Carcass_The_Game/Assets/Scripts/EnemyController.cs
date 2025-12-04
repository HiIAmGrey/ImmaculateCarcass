using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public int enemyHealth = 20;

    [Header("Damage")]
    public int enemyDamage = 3;     // base damage
    public int damageVariance = 1;  // +/- this value
    public float critChance = 0.05f;
    public float critMultiplier = 1.4f;

    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;

    public System.Action onEnemyDamaged;
    public System.Action onEnemyDied;

    public GameObject targetArrow;

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        // Floating number UI
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
        if (enemyHealth < 0) enemyHealth = 0;

        // Hit animation (if "Hit" trigger exists)
        if (anim != null)
        {
            if (anim.runtimeAnimatorController != null &&
                HasParameter(anim, "Hit", AnimatorControllerParameterType.Trigger))
            {
                anim.SetTrigger("Hit");
            }
        }

        onEnemyDamaged?.Invoke();

        if (enemyHealth <= 0)
            Die();
    }

    public void TakeTurn()
    {
        // Play attack animation if exists
        if (anim != null)
            anim.SetTrigger("Attack");

        CombatManager.Instance.StartCoroutine(DoDelayedAttack());
    }

    private IEnumerator DoDelayedAttack()
    {
        yield return new WaitForSeconds(0.6f);

        // Base damage + variance
        int dmg = enemyDamage + Random.Range(-damageVariance, damageVariance + 1);
        if (dmg < 1) dmg = 1;

        // Crit
        if (Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critMultiplier);
            Debug.Log("ENEMY CRITICAL HIT!");
        }

        // Guarding reduces damage
        if (PlayerCombat.Instance.isGuarding)
        {
            dmg = Mathf.RoundToInt(dmg * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
        }

        PlayerHealth.Instance.TakeDamage(dmg);

        TurnManager.Instance.EndEnemyTurn();
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        PlayerStats.Instance.AddXP(5);

        onEnemyDied?.Invoke();
        CombatManager.Instance.EnemyDied(this);

        Destroy(gameObject);
    }

    public void SetTargetArrow(bool on)
    {
        if (targetArrow != null)
            targetArrow.SetActive(on);
    }

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
