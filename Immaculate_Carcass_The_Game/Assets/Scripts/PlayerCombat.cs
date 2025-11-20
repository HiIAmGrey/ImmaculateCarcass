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
            Debug.Log("Enemy count = " + CombatManager.Instance.enemies.Count);
            Debug.Log("Current enemy = " + CombatManager.Instance.GetCurrentEnemy());

            if (TurnManager.Instance.state != TurnState.PlayerTurn)
                return;

            var enemy = CombatManager.Instance.GetCurrentEnemy();

            if (enemy != null)
                enemy.TakeDamage(attackDamage);

            TurnManager.Instance.EndPlayerTurn();
        }


    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        isGuarding = true;

        //  show in UI later
        Debug.Log("Player is guarding!");

        TurnManager.Instance.EndPlayerTurn();
    }
}
