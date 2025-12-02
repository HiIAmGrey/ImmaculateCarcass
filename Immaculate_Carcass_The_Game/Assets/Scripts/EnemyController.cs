using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public int enemyHealth = 20;
    public int enemyDamage = 3;

    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;

    public System.Action onEnemyDamaged;
    public System.Action onEnemyDied;

    public GameObject targetArrow;

    private Animator anim;  // Enemy's animator (optional)

    void Awake()
    {
        // Grab animator if the enemy has one
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        // Spawn floating damage numbers
        Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);

        GameObject dmgNum = Instantiate(damageNumberPrefab, CombatManager.Instance.combatCanvas);
        RectTransform canvasRect = CombatManager.Instance.combatCanvas;
        RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out uiPos);
        dmgRect.anchoredPosition = uiPos;

        dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

        // Apply actual damage
        enemyHealth -= dmg;
        if (enemyHealth < 0) enemyHealth = 0;

         // Eyeball Hit Anim
       if (anim != null)
{
    // Only play Hit trigger if it exists
    if (anim.runtimeAnimatorController != null &&
        HasParameter(anim, "Hit", AnimatorControllerParameterType.Trigger))
    {
        anim.SetTrigger("Hit");
    }
}

        // Update UI
        onEnemyDamaged?.Invoke();

        if (enemyHealth <= 0)
            Die();
    }

    public void TakeTurn()
    {
        // Play the enemy’s attack animation (if it has one)
        if (anim != null)
            anim.SetTrigger("Attack");

        // Delay the hit so it lines up with the animation
        CombatManager.Instance.StartCoroutine(DoDelayedAttack());
    }

    private IEnumerator DoDelayedAttack()
    {
        // Small delay before damage lands
        yield return new WaitForSeconds(0.6f);

        int dmg = enemyDamage;

        // Guard reduces damage (one-time block)
        if (PlayerCombat.Instance.isGuarding)
        {
            dmg = Mathf.RoundToInt(dmg * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
        }

        // Deal damage to player
        PlayerHealth.Instance.TakeDamage(dmg);

        // Enemy turn ends afterward
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
