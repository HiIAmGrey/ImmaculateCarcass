using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int enemyHealth = 20;
    public int enemyDamage = 3;
    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;

    public Canvas combatCanvas; 

    public void TakeDamage(int dmg)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);

        GameObject dmgNum = Instantiate(damageNumberPrefab, combatCanvas.transform);

        RectTransform canvasRect = combatCanvas.GetComponent<RectTransform>();
        RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out uiPos
        );

        dmgRect.anchoredPosition = uiPos;

        dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

        enemyHealth -= dmg;
        if (enemyHealth <= 0)
            Die();
    }

    public void TakeTurn()
    {
        int dmg = enemyDamage;

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
        CombatManager.Instance.EnemyDied(this);
        Destroy(gameObject);
    }
}
