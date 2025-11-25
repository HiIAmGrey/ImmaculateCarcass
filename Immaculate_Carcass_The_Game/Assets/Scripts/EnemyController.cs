using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int enemyHealth = 20;
    public int enemyDamage = 3;
    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;
    public System.Action onEnemyDamaged;
    public System.Action onEnemyDied;
    public GameObject targetArrow;

    public Canvas combatCanvas; 

public void TakeDamage(int dmg)
{
    // damage text code stays the same
    Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);

    GameObject dmgNum = Instantiate(damageNumberPrefab, CombatManager.Instance.combatCanvas);
    RectTransform canvasRect = CombatManager.Instance.combatCanvas;
    RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

    Vector2 uiPos;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out uiPos);
    dmgRect.anchoredPosition = uiPos;

    dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

    // apply damage
    enemyHealth -= dmg;
    if (enemyHealth < 0) enemyHealth = 0;

    // tell UI the HP changed
    onEnemyDamaged?.Invoke();

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
    Debug.Log("Enemy died!");

    // 1. Mark overworld enemy as dead
    if (PersistentGameState.encounterID >= 0)
    {
        if (PersistentGameState.isOverworldEncounter)
            {
                PersistentGameState.overworldAIDead[PersistentGameState.encounterID] = true;
                Debug.Log("Marked overworld enemy " + PersistentGameState.encounterID + " as DEAD.");
            }
            else
            {
                Debug.Log("Combat was from a grave — NOT marking overworld enemies.");
            }
    }

    // 2. Give XP
    PlayerStats.Instance.AddXP(5);

    // 3. Notify UI
    onEnemyDied?.Invoke();
    CombatManager.Instance.EnemyDied(this);

    // 4. Destroy object in combat scene
    Destroy(gameObject);
}

public void SetTargetArrow(bool on)
{
    if (targetArrow != null)
        targetArrow.SetActive(on);
}

}