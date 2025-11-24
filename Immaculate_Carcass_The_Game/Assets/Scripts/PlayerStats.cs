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

    public void AddXP(int amount)
    {
        xp += amount;
        Debug.Log("Gained XP: " + amount + " (Total: " + xp + ")");

        if (xp >= xpToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        xp -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f); // progressive scaling

        // increase stats
        PlayerCombat.Instance.attackDamage += attackIncreasePerLevel;
        PlayerHealth.Instance.maxHealth += healthIncreasePerLevel;
        PlayerHealth.Instance.currentHealth = PlayerHealth.Instance.maxHealth;

        Debug.Log("LEVEL UP! Now level " + level);
    }
}
