using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    public int enemyHealth = 20;
    public int enemyDamage = 3;
    public GameObject damageNumberPrefab;
    public Transform damageSpawnPoint;

    void Awake()
    {
        Instance = this;
    }

    // Called when player attacks
public void TakeDamage(int dmg)
{
    // 1. Convert world → screen pixel position
    Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);

    // 2. Spawn the UI text
    GameObject dmgNum = Instantiate(damageNumberPrefab, CanvasSingleton.Instance.transform);

    RectTransform canvasRect = CanvasSingleton.Instance.GetComponent<RectTransform>();
    RectTransform dmgRect = dmgNum.GetComponent<RectTransform>();

    // 3. Convert screen → UI local position
    Vector2 uiPos;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRect,
        screenPos,
        null,   // IMPORTANT: null for Screen Space — Overlay
        out uiPos
    );

    dmgRect.anchoredPosition = uiPos;

    // 4. Show damage
    dmgNum.GetComponent<FloatingDamage>().ShowDamage(dmg);

    // 5. Apply damage
    enemyHealth -= dmg;
    if (enemyHealth <= 0)
        Die();
}







    // Enemy’s turn logic
    public void TakeTurn()
    {
        int dmg = enemyDamage;

        // Player guard effect
        if (PlayerCombat.Instance.isGuarding)
        {
            dmg = Mathf.RoundToInt(dmg * 0.5f); // 50% reduced
            PlayerCombat.Instance.isGuarding = false; // remove guard
        }

        PlayerHealth.Instance.TakeDamage(dmg);
        Debug.Log("Enemy attacked player for: " + dmg);

        TurnManager.Instance.EndEnemyTurn();
    }

    void Die()
    {
        Debug.Log("Enemy is dead!");
        // You can add: Destroy(gameObject) or scene transition later
    }
}
