using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public List<EnemyController> enemies = new List<EnemyController>();
    public int currentEnemyIndex = 0;

    void Awake()
    {
        Instance = this;

        // Find all enemies in the combat scene
        EnemyController[] foundEnemies = FindObjectsOfType<EnemyController>();
        enemies.AddRange(foundEnemies);

        Debug.Log("CombatManager found " + enemies.Count + " enemies.");
    }

    public EnemyController GetCurrentEnemy()
    {
        if (enemies.Count == 0) return null;
        return enemies[currentEnemyIndex];
    }

    public void EnemyDied(EnemyController enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            Debug.Log("All enemies defeated. Combat ends!");
            // TODO: exit combat or return to overworld here
            return;
        }

        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;
    }
}
