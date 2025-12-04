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

    public EnemyController selectedEnemy;

    void Awake()
    {
        Instance = this;

        // get all enemies in the scene
        EnemyController[] foundEnemies = FindObjectsOfType<EnemyController>();
        enemies.AddRange(foundEnemies);

        foreach (var e in enemies)
        {
            // make a UI entry for each enemy
            var ui = Instantiate(enemyUIPrefab, enemyUIPanel);
            ui.GetComponent<EnemyUIEntry>().Initialize(e);

            if (selectedEnemy == null)
                selectedEnemy = e;
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
        if (enemies.Count == 0) return null;
        return enemies[currentEnemyIndex];
    }

    public void SetSelectedEnemy(EnemyController enemy)
    {
        selectedEnemy = enemy;

        foreach (var e in enemies)
            e.SetTargetArrow(e == enemy);
    }

    public void EnemyDied(EnemyController enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
        {
            // all enemies dead -> combat over

            // if this was a grave fight (references encounter ids)
            if (PersistentGameState.encounterID >= 10)
            {
                int graveID = PersistentGameState.encounterID - 10;

                // now the grave is officially cleared
                PersistentGameState.graveDug[graveID] = true;
                PersistentGameState.graveCount++;
            }

            if (enemyUIPanel != null)
                enemyUIPanel.gameObject.SetActive(false);

            ExitCombat();
            return;
        }

        // make sure we don't go out of range
        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;

        if (selectedEnemy == enemy)
            selectedEnemy = enemies[0];
    }

    public void ExitCombat()
    {
        // overworld AI should stay dead if the fight came from one
        if (PersistentGameState.encounterID >= 0 && PersistentGameState.encounterID < 10)
        {
            PersistentGameState.overworldAIDead[PersistentGameState.encounterID] = true;
        }

        SceneManager.LoadScene("GameScene");
    }
}
