using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("Notification UI")]
    public TMP_Text notificationText;
    public float defaultDuration = 1.5f;

    private float timer = 0f;
    private bool showing = false;

    void Awake()
    {
        Instance = this;
        ClearNotification();
    }

    void Update()
    {
        if (!showing) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ClearNotification();
        }
    }

    public void ShowNotification(string message, float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;

        notificationText.gameObject.SetActive(true);
        notificationText.text = message;

        timer = duration;
        showing = true;
    }

    public void ClearNotification()
    {
        notificationText.text = "";
        notificationText.gameObject.SetActive(false);
        showing = false;
    }
}
