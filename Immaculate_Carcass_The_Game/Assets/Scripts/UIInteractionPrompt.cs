using UnityEngine;
using TMPro;

public class UIInteractionPrompt : MonoBehaviour
{
    public static UIInteractionPrompt Instance;

    public GameObject promptPanel;
    public TMP_Text promptText;

    void Awake()
    {
        // simple singleton so other scripts can call ShowPrompt()
        Instance = this;
    }

    public void ShowPrompt(string msg)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = msg;
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}
