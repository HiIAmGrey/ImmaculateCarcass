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

    public void EndPlayerTurn()
    {
        state = TurnState.EnemyTurn;
        EnemyController.Instance.TakeTurn();
    }

    public void EndEnemyTurn()
    {
        state = TurnState.PlayerTurn;
    }
}
