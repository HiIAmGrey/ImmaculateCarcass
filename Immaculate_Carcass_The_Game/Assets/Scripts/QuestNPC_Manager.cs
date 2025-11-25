using UnityEngine;

public class QuestNPC_Manager : MonoBehaviour
{
    public static QuestNPC_Manager Instance;

    public QuestNPC npc; 

    void Awake()
    {
        Instance = this;
    }

    public int GetState()
    {
        return (int)npc.state;
    }

    public void SetState(int s)
    {
        npc.state = (QuestNPC.QuestState)s;
    }
}
