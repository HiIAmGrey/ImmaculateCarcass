using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health Settings")]
    public int maxHealth = 20;
    public int currentHealth = 20;

    private float smoothFill = 1f;

    [Header("UI Elements")]
    public Image hpFill;
    public TMP_Text hpText;
    public Transform damageSpawnPoint;
    public GameObject playerDamagePrefab;

    [Header("Arcane Shield")]
    public int shieldAmount = 0;
    public int shieldTurnsRemaining = 0;
    public int arcaneShieldCooldown = 0;

    [Header("Damage SFX (multiple clips)")]
    public AudioClip[] playerDamageSounds;  
    // randomly picks one when the player gets hurt

    [Header("Death SFX")]
    public AudioClip playerDeathSFX;  
    // plays when the player dies

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // load HP from the persistent system
        maxHealth = PersistentGameState.playerMaxHP;
        currentHealth = PersistentGameState.playerCurrentHP;

        if (maxHealth <= 0)
            maxHealth = 20;

        smoothFill = (float)currentHealth / maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();
    }

    // player takes damage
    public void TakeDamage(int amount)
    {
        Debug.Log("PlayerHealth.TakeDamage CALLED => " + amount);

        // guard damage reduction
        if (PlayerCombat.Instance.isGuarding)
        {
            amount = Mathf.RoundToInt(amount * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
            Debug.Log("Guard reduced damage to: " + amount);
        }

        // shield handling
        if (shieldAmount > 0)
        {
            int absorbed = Mathf.Min(shieldAmount, amount);
            shieldAmount -= absorbed;
            amount -= absorbed;

            Debug.Log($"Shield absorbed {absorbed}. Remaining shield: {shieldAmount}");

            if (shieldAmount <= 0)
            {
                shieldAmount = 0;
                shieldTurnsRemaining = 0;
                PlayerCombat.Instance.DestroyShieldFX();
                Debug.Log("Shield broke!");
            }

            // shield ate all the damage
            if (amount <= 0)
            {
                SpawnDamageText(absorbed);
                return;
            }
        }

        // apply damage
        currentHealth -= amount;

        // save new HP
        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;

        SpawnDamageText(amount);

        // play a random damage sound
        PlayRandomDamageSound();

        // check for death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    // play random hurt sound
    private void PlayRandomDamageSound()
    {
        if (playerDamageSounds == null || playerDamageSounds.Length == 0)
            return;

        int index = Random.Range(0, playerDamageSounds.Length);
        AudioClip clip = playerDamageSounds[index];

        if (clip != null)
            AudioManager.Instance.PlaySFX(clip);
    }

    // heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;

        UpdateHealthUI();
    }

    // update HP bar + number
    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            float targetFill = (float)currentHealth / maxHealth;
            smoothFill = Mathf.Lerp(smoothFill, targetFill, Time.deltaTime * 12f);
            hpFill.fillAmount = smoothFill;

            // hp color gradient
            Color healthy = new Color(0.5f, 0f, 0f);
            Color dying = new Color(0.15f, 0f, 0f);
            hpFill.color = Color.Lerp(dying, healthy, smoothFill);
        }

        if (hpText != null)
            hpText.text = $"{currentHealth}/{maxHealth}";
    }

    // floating damage text
    private void SpawnDamageText(int amount)
    {
        if (playerDamagePrefab != null && damageSpawnPoint != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(damageSpawnPoint.position);

            GameObject dmgObj = Instantiate(playerDamagePrefab, CombatManager.Instance.combatCanvas);
            RectTransform canvasRect = CombatManager.Instance.combatCanvas;
            RectTransform dmgRect = dmgObj.GetComponent<RectTransform>();

            Vector2 uiPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out uiPos);
            dmgRect.anchoredPosition = uiPos;

            dmgObj.GetComponent<FloatingDamage>().ShowDamage(amount);
        }
    }

    // player death
    void Die()
    {
        Debug.Log("Player died.");

        // save game before death scene
        PersistentGameState.SaveFromGame();

        // restore HP so the player doesn't reload at 0 HP
        PersistentGameState.playerCurrentHP = PersistentGameState.playerMaxHP;

        // play death sfx if assigned
        if (playerDeathSFX != null)
            AudioManager.Instance.PlaySFX(playerDeathSFX);

        // short delay so the sound is actually audible
        StartCoroutine(LoadDeathSceneAfterDelay(1f));
    }

    // delay before switching scenes
    private System.Collections.IEnumerator LoadDeathSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        UnityEngine.SceneManagement.SceneManager.LoadScene("DeathScene");
    }
}
