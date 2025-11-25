using UnityEngine;
using TMPro;

public class UIInteractionPrompt : MonoBehaviour
{
    public static UIInteractionPrompt Instance;

    [Header("References")]
    public GameObject promptPanel;   
    public TMP_Text promptText;       

    void Awake()
    {
        Instance = this;
        HidePrompt();
    }

    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        promptPanel.SetActive(false);
    }
}
