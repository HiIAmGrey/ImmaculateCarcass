using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public RectTransform combatCanvas;

    public List<EnemyController> enemies = new List<EnemyController>();
    public int currentEnemyIndex = 0;

    public GameObject enemyUIPrefab;   
    public Transform enemyUIPanel;

    // whichever enemy the player clicked on
    public EnemyController selectedEnemy; 

    void Awake()
    {
        Instance = this;

        // grab every enemy in the scene
        EnemyController[] foundEnemies = FindObjectsOfType<EnemyController>();
        enemies.AddRange(foundEnemies);

        Debug.Log("CombatManager found " + enemies.Count + " enemies.");

        // make a UI entry for each enemy
        foreach (var enemy in enemies)
        {
            GameObject ui = Instantiate(enemyUIPrefab, enemyUIPanel);
            ui.GetComponent<EnemyUIEntry>().Initialize(enemy);

            // auto-select the first enemy so there's always a valid target
            if (selectedEnemy == null)
                selectedEnemy = enemy;
        }

        if (enemies.Count == 0 && enemyUIPanel != null)
            enemyUIPanel.gameObject.SetActive(false);
    }

    public EnemyController GetSelectedEnemy()
    {
        return selectedEnemy;
    }

    public EnemyController GetCurrentEnemy()
    {
        if (enemies.Count == 0)
            return null;

        return enemies[currentEnemyIndex];
    }

    public void SetSelectedEnemy(EnemyController enemy)
    {
        selectedEnemy = enemy;

        foreach (var e in enemies)
        {
            bool isSelected = (e == enemy);
            e.SetTargetArrow(isSelected);
        }

        Debug.Log("Selected enemy: " + enemy.gameObject.name);
    }

    public void EnemyDied(EnemyController enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            Debug.Log("All enemies defeated. Combat ends!");

            if (enemyUIPanel != null)
                enemyUIPanel.gameObject.SetActive(false);

            ExitCombat();
            return;
        }

        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;

        if (selectedEnemy == enemy)
            selectedEnemy = enemies[0];
    }

    public void ExitCombat()
    {
        // MARK OVERWORLD AI AS DEAD  BUT ONLY IF THE ENCOUNTER WAS FROM A PATROLLING AI
        if (PersistentGameState.encounterID >= 100)
        {
            PersistentGameState.overworldAIDead[PersistentGameState.encounterID] = true;
        }

        SceneManager.LoadScene("GameScene");
    }

}
