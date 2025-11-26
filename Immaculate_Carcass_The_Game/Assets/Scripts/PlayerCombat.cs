using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    public int attackDamage = 5;
    public bool isGuarding = false;

    void Awake()
    {
        Instance = this;
        Debug.Log("PlayerCombat Awake — CombatManager.Instance = " + CombatManager.Instance);
    }

    void Start()
    {
        // Load attack damage from saved data
        attackDamage = PersistentGameState.playerAttackDamage;
    }

    public void Attack()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        Debug.Log("Enemy count = " + CombatManager.Instance.enemies.Count);

        var enemy = CombatManager.Instance.GetSelectedEnemy();
        Debug.Log("Selected enemy = " + enemy);

        if (enemy != null)
            enemy.TakeDamage(attackDamage);

        TurnManager.Instance.EndPlayerTurn();
    }

    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        isGuarding = true;
        Debug.Log("Player is guarding!");

        TurnManager.Instance.EndPlayerTurn();
    }
}
