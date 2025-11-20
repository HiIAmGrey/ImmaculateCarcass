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
    public Image hpFill;           // Drag HpFill image here
    public TMP_Text hpText;        // Drag TMP text object (for HP numbers)
    public Transform damageSpawnPoint;
    public GameObject playerDamagePrefab;

    void Start()
    {
         Instance = this;
        currentHealth = maxHealth;
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
            if (currentHealth < 0) currentHealth = 0;

            // spawn the floating damage UI
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

            // update the player HP UI
            UpdateHealthUI();

            // if player dies implement death logic here
        }


    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            float targetFill = (float)currentHealth / maxHealth;
            smoothFill = Mathf.Lerp(smoothFill, targetFill, Time.deltaTime * 15f);
            hpFill.fillAmount = smoothFill;

            // HP Color Change 
            Color healthy = new Color(0.5f, 0f, 0f);   // deep crimson
            Color dying = new Color(0.15f, 0f, 0f);    // almost black-red
            Color currentColor = Color.Lerp(dying, healthy, smoothFill);

            // Low-HP pulsing 
            if (smoothFill < 0.3f)
            {
                float pulse = Mathf.Sin(Time.time * 6f) * 0.25f + 0.75f; // subtle pulse between 0.5–1
                currentColor *= pulse; // slightly brighten/dim
            }

            hpFill.color = currentColor;
        }

        // Update HP Text
        if (hpText != null)
            hpText.text = $"{currentHealth}/{maxHealth}";
    }
}
