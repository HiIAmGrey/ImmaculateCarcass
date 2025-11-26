using UnityEngine;

public class QuestNPC_Manager : MonoBehaviour
{
    public static QuestNPC_Manager Instance;

    // reminder that 0 = NotStarted, 1 = InProgress, 2 = Completed, 3 = FinalBattle
    public int currentState = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load previously saved quest state
        currentState = PersistentGameState.questState;
    }

    public int GetState()
    {
        return currentState;
    }

    public void SetState(int newState)
    {
        currentState = newState;
        PersistentGameState.questState = newState;
    }
}
