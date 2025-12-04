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

    // tracks whether this combat ended with a final boss kill
    private bool finalBossDefeated = false;

    void Awake()
    {
        Instance = this;

        // find all enemies in this combat scene
        EnemyController[] found = FindObjectsOfType<EnemyController>();
        enemies.AddRange(found);

        // make a UI entry for each enemy
        foreach (var e in enemies)
        {
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
        if (enemies.Count == 0)
            return null;

        return enemies[currentEnemyIndex];
    }

    public void SetSelectedEnemy(EnemyController enemy)
    {
        selectedEnemy = enemy;

        foreach (var e in enemies)
            e.SetTargetArrow(e == enemy);
    }

    // ============================================================
    // UPDATED SIGNATURE: EnemyController now calls EnemyDied(this, isFinalBoss)
    // ============================================================
    public void EnemyDied(EnemyController enemy, bool isFinalBoss)
    {
        enemies.Remove(enemy);

        // store this so ExitCombat() can respond properly
        if (isFinalBoss)
            finalBossDefeated = true;

        // if all enemies died, end combat
        if (enemies.Count == 0)
        {
            HandleCombatWin();
            ExitCombat();
            return;
        }

        // clamp index
        if (currentEnemyIndex >= enemies.Count)
            currentEnemyIndex = enemies.Count - 1;

        // switch selection if needed
        if (selectedEnemy == enemy && enemies.Count > 0)
            selectedEnemy = enemies[0];
    }

    // handles grave wins + overworld wins
    private void HandleCombatWin()
    {
        // grave fights use activeGraveID
        if (PersistentGameState.activeGraveID != -1)
        {
            int gid = PersistentGameState.activeGraveID;

            PersistentGameState.graveDug[gid] = true;
            PersistentGameState.graveCount++;

            PersistentGameState.activeGraveID = -1;
        }

        // overworld AI uses encounterID but must be inside array bounds
        if (PersistentGameState.isOverworldEncounter)
        {
            int id = PersistentGameState.encounterID;

            if (id >= 0 && id < PersistentGameState.overworldAIDead.Length)
                PersistentGameState.overworldAIDead[id] = true;
        }

        Debug.Log(
            $"WIN → isOverworldEncounter={PersistentGameState.isOverworldEncounter}, " +
            $"encounterID={PersistentGameState.encounterID}, " +
            $"finalBoss={finalBossDefeated}"
        );
    }

    public void ExitCombat()
    {
        // 🚨 NEW: If final boss was killed, go to victory scene
        if (finalBossDefeated)
        {
            Debug.Log("FINAL BOSS DEFEATED — Loading VictoryScene!");
            SceneManager.LoadScene("VictoryScene");
            return;
        }

        // reset overworld flag when leaving combat
        PersistentGameState.isOverworldEncounter = false;

        // return to overworld normally
        SceneManager.LoadScene("GameScene");
    }
}
