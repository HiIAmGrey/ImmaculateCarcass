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

    public void Attack()
    {
        // don't let the player attack if it's not their turn
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        Debug.Log("Enemy count = " + CombatManager.Instance.enemies.Count);

        // use the enemy the player actually clicked on
        var enemy = CombatManager.Instance.GetSelectedEnemy();

        Debug.Log("Selected enemy = " + enemy);

        if (enemy != null)
        {
            // just deal the damage lol
            enemy.TakeDamage(attackDamage);
        }
        else
        {
            Debug.Log("No enemy selected (somehow)");
        }

        // end the player's turn after attacking
        TurnManager.Instance.EndPlayerTurn();
    }

    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        // block half damage next hit
        isGuarding = true;

        Debug.Log("Player is guarding!");

        // still ends the player's turn
        TurnManager.Instance.EndPlayerTurn();
    }
}
