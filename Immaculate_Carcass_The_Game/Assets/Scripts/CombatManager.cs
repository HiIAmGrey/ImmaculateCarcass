using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    // this is just the combat canvas so I can reference it later if needed
    public RectTransform combatCanvas;

    public List<EnemyController> enemies = new List<EnemyController>();
    public int currentEnemyIndex = 0;

    public GameObject enemyUIPrefab;   // the prefab for the enemy UI entry
    public Transform enemyUIPanel;     // the whole UI panel on the right side

    void Awake()
    {
        Instance = this;

        // finding all the enemies in the scene so I can set up their UI
        EnemyController[] foundEnemies = FindObjectsOfType<EnemyController>();
        enemies.AddRange(foundEnemies);

        Debug.Log("CombatManager found " + enemies.Count + " enemies.");

        // spawn a UI entry for each enemy
        foreach (var enemy in enemies)
        {
            GameObject ui = Instantiate(enemyUIPrefab, enemyUIPanel);
            ui.GetComponent<EnemyUIEntry>().Initialize(enemy);
        }

        // if somehow there are no enemies at the start, just hide the panel
        if (enemies.Count == 0 && enemyUIPanel != null)
            enemyUIPanel.gameObject.SetActive(false);
    }

    public EnemyController GetCurrentEnemy()
    {
        if (enemies.Count == 0)
            return null; // there's literally no enemy left lol

        return enemies[currentEnemyIndex];
    }

    public void EnemyDied(EnemyController enemy)
    {
        // remove that enemy from the list
        enemies.Remove(enemy);

        // if that was the last enemy alive...
        if (enemies.Count == 0)
        {
            Debug.Log("All enemies defeated. Combat ends!");

            // hide the whole UI panel so there's not an empty box sitting there
            if (enemyUIPanel != null)
                enemyUIPanel.gameObject.SetActive(false);

            return;
        }

        // make sure the index doesn't go out of bounds
        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;
    }
}
