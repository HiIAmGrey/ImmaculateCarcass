using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;   // now supports TMP

    [Header("Audio")]
    public AudioClip dialogueBlipSFX;
    public float blipCooldown = 0.05f;

    private float nextBlipTime = 0f;

    private string[] lines;
    private int index = 0;
    private System.Action onFinished;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDialogue(System.Action finishedCallback, params string[] dialogueLines)
    {
        onFinished = finishedCallback;
        lines = dialogueLines;
        index = 0;

        dialogueBox.SetActive(true);
        DisplayLine();
    }

    public void ShowDialogue(params string[] dialogueLines)
    {
        ShowDialogue(null, dialogueLines);
    }

    void Update()
    {
        if (!dialogueBox.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
            NextLine();
    }

    void DisplayLine()
    {
        dialogueText.text = lines[index];

        // play blip effect if assigned
        if (dialogueBlipSFX && Time.time >= nextBlipTime)
        {
            AudioManager.Instance.PlaySFX(dialogueBlipSFX);
            nextBlipTime = Time.time + blipCooldown;
        }
    }

    void NextLine()
    {
        index++;
        if (index >= lines.Length)
        {
            dialogueBox.SetActive(false);
            onFinished?.Invoke();
            return;
        }

        DisplayLine();
    }
}
