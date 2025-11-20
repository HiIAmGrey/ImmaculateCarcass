using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour
{
    public float fadeSpeed = 1.5f;
    public float floatSpeedMin = 40f;
    public float floatSpeedMax = 60f;

    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;

    // movement
    private Vector2 floatDirection;

    // scale pop
    private float popDuration = 0.12f;
    private float popTimer = 0f;
    private Vector3 startScale = Vector3.one;
    private Vector3 popScale = new Vector3(1.4f, 1.4f, 1.4f);

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // start tiny for pop animation
        transform.localScale = popScale;

        // random float direction
        floatDirection = new Vector2(
            Random.Range(-15f, 15f),
            Random.Range(floatSpeedMin, floatSpeedMax)
        );
    }

    public void ShowDamage(int amount)
    {
        text.text = amount.ToString();
        canvasGroup.alpha = 1f;

        Destroy(gameObject, 1.2f);
    }

    void Update()
    {
        // SCALE POP (1.4 → 1.0)
        if (popTimer < popDuration)
        {
            popTimer += Time.deltaTime;
            float t = popTimer / popDuration;
            transform.localScale = Vector3.Lerp(popScale, startScale, t);
        }

        // FLOAT UPWARD (+ slight sideways drift)
        transform.Translate(floatDirection * Time.deltaTime);

        // FADE OUT
        canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
    }
}
