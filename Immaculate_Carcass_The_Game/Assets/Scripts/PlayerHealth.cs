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

    // ============================
    // LIFECYCLE
    // ============================
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load HP from persistent system
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

    // ============================
    // DAMAGE
    // ============================
    public void TakeDamage(int amount)
    {
        Debug.Log("PlayerHealth.TakeDamage CALLED => " + amount);

        // ----------------------------------------
        // GUARD DAMAGE REDUCTION
        // ----------------------------------------
        if (PlayerCombat.Instance.isGuarding)
        {
            amount = Mathf.RoundToInt(amount * 0.5f);
            PlayerCombat.Instance.isGuarding = false;
            Debug.Log("Guard reduced damage to: " + amount);
        }

        // ----------------------------------------
        // SHIELD ABSORPTION
        // ----------------------------------------
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

            if (amount <= 0)
            {
                SpawnDamageText(absorbed);
                return;
            }
        }

        // ----------------------------------------
        // APPLY HP DAMAGE
        // ----------------------------------------
        currentHealth -= amount;

        // persistent save
        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;

        SpawnDamageText(amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    // ============================
    // HEALING
    // ============================
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;

        UpdateHealthUI();
    }

    // ============================
    // UI UPDATE
    // ============================
    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            float targetFill = (float)currentHealth / maxHealth;
            smoothFill = Mathf.Lerp(smoothFill, targetFill, Time.deltaTime * 12f);
            hpFill.fillAmount = smoothFill;

            // HP color gradient
            Color healthy = new Color(0.5f, 0f, 0f);
            Color dying   = new Color(0.15f, 0f, 0f);
            hpFill.color = Color.Lerp(dying, healthy, smoothFill);
        }

        if (hpText != null)
            hpText.text = $"{currentHealth}/{maxHealth}";
    }

    // ============================
    // DAMAGE FLOATING TEXT
    // ============================
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

    // ============================
    // DEATH
    // ============================
    void Die()
    {
        Debug.Log("Player died.");
        // Add death animation, scene transitions, etc.
    }
}
