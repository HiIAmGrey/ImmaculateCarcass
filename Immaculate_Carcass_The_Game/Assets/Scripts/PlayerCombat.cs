using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    public int attackDamage = 5;
    public bool isGuarding = false;

    void Awake()
    {
        Instance = this;
    }

    public void Attack()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        EnemyController.Instance.TakeDamage(attackDamage);
        TurnManager.Instance.EndPlayerTurn();
    }

    public void Guard()
    {
        if (TurnManager.Instance.state != TurnState.PlayerTurn)
            return;

        isGuarding = true;

        // OPTIONAL: show UI later
        Debug.Log("Player is guarding!");

        TurnManager.Instance.EndPlayerTurn();
    }
}
