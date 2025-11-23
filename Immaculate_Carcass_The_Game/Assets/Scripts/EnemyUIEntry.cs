using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnemyUIEntry : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public TMP_Text nameText;
    public Slider hpSlider;
    public CanvasGroup canvasGroup;

    // optional border thing if you want to show selection
    public Image highlightBorder;

    private EnemyController enemy;

    // CombatManager calls this when setting up the UI
    public void Initialize(EnemyController c)
    {
        enemy = c;

        // set the name + hp bar
        nameText.text = enemy.gameObject.name;
        hpSlider.maxValue = enemy.enemyHealth;
        hpSlider.value = enemy.enemyHealth;

        // hook into events so UI updates when enemy takes damage / dies
        enemy.onEnemyDamaged += UpdateUI;
        enemy.onEnemyDied += FadeOut;

        // default: turn off highlight until clicked
        Highlight(false);
    }

    // update hp bar when this enemy is damaged
    public void UpdateUI()
    {
        hpSlider.value = enemy.enemyHealth;
    }

    // when player clicks on this UI entry
    public void OnPointerClick(PointerEventData eventData)
    {
        // tell CombatManager this enemy is now the selected one
        CombatManager.Instance.SetSelectedEnemy(enemy);

        // highlight this entry and unhighlight the others
        foreach (Transform child in transform.parent)
        {
            var entry = child.GetComponent<EnemyUIEntry>();
            if (entry != null)
                entry.Highlight(entry == this); // only highlight the one clicked
        }
    }

    // turns the highlight on/off
    public void Highlight(bool on)
    {
        if (highlightBorder != null)
            highlightBorder.enabled = on;
    }

    // run a little fade when the enemy dies so the UI disappears nicely
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }

    private System.Collections.IEnumerator FadeRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;  // fades kinda fast
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        Destroy(gameObject); // yeet it out of the UI
    }
}
