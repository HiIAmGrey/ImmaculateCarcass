using UnityEngine;

public enum TurnState { PlayerTurn, EnemyTurn }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public TurnState state = TurnState.PlayerTurn;

    void Awake()
    {
        Instance = this;
    }

    //---------------------------------------
    // PLAYER → ENEMY
    //---------------------------------------
    public void EndPlayerTurn()
    {
        state = TurnState.EnemyTurn;

        var enemy = CombatManager.Instance.GetCurrentEnemy();
        if (enemy != null)
            enemy.TakeTurn();
    }

    //---------------------------------------
    // ENEMY → PLAYER
    //---------------------------------------
    public void EndEnemyTurn()
    {
        state = TurnState.PlayerTurn;

        // Guard automatically expires after enemy turn
        PlayerCombat.Instance.isGuarding = false;

        // ===== SHIELD TURN REDUCE =====
        if (PlayerHealth.Instance.shieldTurnsRemaining > 0)
        {
            PlayerHealth.Instance.shieldTurnsRemaining--;

            // Expired shield
            if (PlayerHealth.Instance.shieldTurnsRemaining == 0)
            {
                PlayerHealth.Instance.shieldAmount = 0;
                PlayerCombat.Instance.DestroyShieldFX();
                Debug.Log("Shield expired.");
            }
        }

        // ===== COOLDOWN REDUCE =====
        if (PlayerHealth.Instance.arcaneShieldCooldown > 0)
            PlayerHealth.Instance.arcaneShieldCooldown--;
    }
}
