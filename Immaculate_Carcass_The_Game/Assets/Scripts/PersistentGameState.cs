using UnityEngine;

public static class PersistentGameState
{
    // keeping track of player stats we want to save
    public static int playerLevel = 1;
    public static int playerXP = 0;
    public static int playerMaxHP = 100;
    public static int playerCurrentHP = 100;
    public static int playerAttackDamage = 5;

    // items the player picks up (only shovel right now)
    public static bool hasShovel = false;

    // used for spawning the player back where they were
    public static Vector3 savedPlayerPos = Vector3.zero;
    public static bool hasSavedPlayerPos = false;

    // grave digging info (3 graves in your game)
    public static bool[] graveDug = new bool[3];
    public static int graveCount = 0;

    // quest progress from QuestNPC
    // 0 = NotStarted, 1 = InProgress, 2 = Completed, 3 = FinalBattle
    public static int questState = 0;

    // overworld enemies that should stay dead
    public static bool[] overworldAIDead = new bool[10];

    // which encounter triggered combat
    public static int encounterID = 0;

    // save everything from the live scripts before changing scenes
    public static void SaveFromGame()
    {
        if (PlayerStats.Instance != null)
        {
          playerXP = PlayerStats.Instance.xp;
            PlayerStats.Instance.xp = playerXP;
        }

        if (PlayerHealth.Instance != null)
        {
            playerMaxHP = PlayerHealth.Instance.maxHealth;
            playerCurrentHP = PlayerHealth.Instance.currentHealth;
        }

        if (PlayerInventory.Instance != null)
        {
            hasShovel = PlayerInventory.Instance.hasShovel;
        }

        if (QuestNPC_Manager.Instance != null)
        {
            questState = QuestNPC_Manager.Instance.GetState();
        }

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            savedPlayerPos = p.transform.position;
            hasSavedPlayerPos = true;
        }
    }

    // load everything back into the scripts
    public static void LoadIntoGame()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.level = playerLevel;
            PlayerStats.Instance.xp = playerXP;
        }

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.maxHealth = playerMaxHP;
            PlayerHealth.Instance.currentHealth = playerCurrentHP;
        }

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.hasShovel = hasShovel;
        }

        if (QuestNPC_Manager.Instance != null)
        {
            QuestNPC_Manager.Instance.SetState(questState);
        }
    }

    // wipes everything for a brand-new run (used by VictoryMenu + MainMenu)
    public static void ResetAll()
    {
        playerLevel = 1;
        playerXP = 0;
        playerMaxHP = 100;
        playerCurrentHP = 100;
        playerAttackDamage = 5;

        hasShovel = false;

        hasSavedPlayerPos = false;
        savedPlayerPos = Vector3.zero;

        graveDug = new bool[3];
        graveCount = 0;

        questState = 0;

        overworldAIDead = new bool[10];

        encounterID = 0;
    }
}
