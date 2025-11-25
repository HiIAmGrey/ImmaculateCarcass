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

        // nothing to show if there's no enemies somehow
        if (enemies.Count == 0 && enemyUIPanel != null)
            enemyUIPanel.gameObject.SetActive(false);
    }

    // returns the enemy the player clicked
    public EnemyController GetSelectedEnemy()
    {
        return selectedEnemy;
    }

    // old function still here just in case some old code uses it
    public EnemyController GetCurrentEnemy()
    {
        if (enemies.Count == 0)
            return null;

        return enemies[currentEnemyIndex];
    }

    // UI calls this when the player clicks an enemy button
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
        // remove that enemy from the list
        enemies.Remove(enemy);

        // if that was the last enemy alive...
        if (enemies.Count == 0)
        {
            Debug.Log("All enemies defeated. Combat ends!");

            // hide the UI panel so it's not just a blank box
            if (enemyUIPanel != null)
                enemyUIPanel.gameObject.SetActive(false);

            ExitCombat(); // go back to main game scene
            return;
        }

        // keep index in bounds
        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;

        // if the one that died was the selected target, just select a new one
        if (selectedEnemy == enemy)
            selectedEnemy = enemies[0];
    }

    public void ExitCombat()
{
    // Here is where persistent stats will be restored later 
    // PersistentGameState.Load();

    SceneManager.LoadScene("GameScene");
}

}
