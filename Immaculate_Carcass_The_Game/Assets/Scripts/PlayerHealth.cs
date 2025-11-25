using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    private float smoothFill;
    public static PlayerHealth Instance;

    [Header("UI Elements")]
    public Image hpFill;           
    public TMP_Text hpText;        
    public Transform damageSpawnPoint;
    public GameObject playerDamagePrefab;

    void Start()
    {
        // set global reference
        Instance = this;

        // load from persistent state (or defaults on new game)
        maxHealth = PersistentGameState.playerMaxHP;
        currentHealth = PersistentGameState.playerCurrentHP;

        // correct fill state
        if (maxHealth > 0)
            smoothFill = (float)currentHealth / maxHealth;
        else
            smoothFill = 1f;

        UpdateHealthUI();
    }

    void Update()
    {
        UpdateHealthUI();

        // Test controls
        if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(10);

        if (Input.GetKeyDown(KeyCode.H))
            Heal(10);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        // save updated HP to persistent system
        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;

        // spawn floating damage UI
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

        // update HUD
        UpdateHealthUI();

        // handle death if needed (not implemented yet)
    }


    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // save persistent health
        PersistentGameState.playerCurrentHP = currentHealth;
        PersistentGameState.playerMaxHP = maxHealth;
    }

    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            float targetFill = (float)currentHealth / maxHealth;
            smoothFill = Mathf.Lerp(smoothFill, targetFill, Time.deltaTime * 15f);
            hpFill.fillAmount = smoothFill;

            // HP Color Change 
            Color healthy = new Color(0.5f, 0f, 0f);   
            Color dying = new Color(0.15f, 0f, 0f);   
            Color currentColor = Color.Lerp(dying, healthy, smoothFill);

            // Low-HP pulsing 
            if (smoothFill < 0.3f)
            {
                float pulse = Mathf.Sin(Time.time * 6f) * 0.25f + 0.75f;
                currentColor *= pulse;
            }

            hpFill.color = currentColor;
        }

        // Update HP Text
        if (hpText != null)
            hpText.text = $"{currentHealth}/{maxHealth}";
    }
}
