using UnityEngine;
using UnityEngine.SceneManagement;

public static class FinalBossLoader
{
    public static void StartBossCombat()
    {
        //  final boss encounter
        EnemyEncounterManager.SetEncounterID(99);

        // Load  combat scene
        SceneManager.LoadScene("CombatScene");
    }
}
