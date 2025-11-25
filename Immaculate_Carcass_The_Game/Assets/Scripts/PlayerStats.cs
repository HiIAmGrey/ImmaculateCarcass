using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Leveling")]
    public int level = 1;
    public int xp = 0;
    public int xpToNextLevel = 20;

    [Header("Stat Growth")]
    public int attackIncreasePerLevel = 1;
    public int healthIncreasePerLevel = 5;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // load level & XP from persistent data
        level = PersistentGameState.playerLevel;
        xp = PersistentGameState.playerXP;
    }

    public void AddXP(int amount)
    {
        xp += amount;
        Debug.Log("Gained XP: " + amount + " (Total: " + xp + ")");

        // save new XP immediately
        PersistentGameState.playerXP = xp;

        if (xp >= xpToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        xp -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);

        // increase stats
        PlayerCombat.Instance.attackDamage += attackIncreasePerLevel;
        PlayerHealth.Instance.maxHealth += healthIncreasePerLevel;
        PlayerHealth.Instance.currentHealth = PlayerHealth.Instance.maxHealth;

        // save new level and HP to persistent data
        PersistentGameState.playerLevel = level;
        PersistentGameState.playerXP = xp;
        PersistentGameState.playerMaxHP = PlayerHealth.Instance.maxHealth;
        PersistentGameState.playerCurrentHP = PlayerHealth.Instance.currentHealth;

        Debug.Log("LEVEL UP! Now level " + level);
    }
}
