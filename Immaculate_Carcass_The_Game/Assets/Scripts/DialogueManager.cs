using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public TMP_Text continueHint;

    private Queue<string> lines = new Queue<string>();
    private bool dialogueActive = false;

    private System.Action onDialogueFinished;

    void Awake()
    {
        Instance = this;
        HideDialogue();
    }

    void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }

    //  VERSION 1 — NO CALLBACK 
    public void ShowDialogue(params string[] dialogueLines)
    {
        ShowDialogue(null, dialogueLines);
    }

    //  VERSION 2 — CALLBACK VERSION
    public void ShowDialogue(System.Action finishedCallback, params string[] dialogueLines)
    {
        onDialogueFinished = finishedCallback;

        lines.Clear();
        foreach (string line in dialogueLines)
            lines.Enqueue(line);

        dialogueActive = true;
        dialogueBox.SetActive(true);
        continueHint.gameObject.SetActive(true);

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            HideDialogue();

            // Run callback AFTER the dialogue ends
            onDialogueFinished?.Invoke();
            onDialogueFinished = null;
            return;
        }

        string nextLine = lines.Dequeue();
        dialogueText.text = nextLine;
        continueHint.gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        dialogueActive = false;
        dialogueBox.SetActive(false);
        dialogueText.text = "";

        if (continueHint != null)
            continueHint.gameObject.SetActive(false);
    }
}
