using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIEntry : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public Slider hpSlider;
    public CanvasGroup canvasGroup;

    private EnemyController enemy;

    // Called by CombatManager
    public void Initialize(EnemyController c)
    {
        enemy = c;
        nameText.text = enemy.gameObject.name;
        hpSlider.maxValue = enemy.enemyHealth;
        hpSlider.value = enemy.enemyHealth;

        enemy.onEnemyDamaged += UpdateUI;
        enemy.onEnemyDied += FadeOut;
    }

    public void UpdateUI()
    {
        hpSlider.value = enemy.enemyHealth;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }

    private System.Collections.IEnumerator FadeRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        Destroy(gameObject);
    }
}
