using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public TMP_Text continueHint; // "Press SPACE to continue"

    private Queue<string> lines = new Queue<string>();
    private bool dialogueActive = false;

    void Awake()
    {
        Instance = this;
        HideDialogue();
    }

    void Update()
    {
        // player presses SPACE to advance or close dialogue
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }

    // start a dialogue with one or more lines
    public void ShowDialogue(params string[] dialogueLines)
    {
        lines.Clear();

        foreach (string line in dialogueLines)
            lines.Enqueue(line);

        dialogueBox.SetActive(true);
        continueHint.gameObject.SetActive(true);

        dialogueActive = true;

        // show first line immediately
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        // if we run out of lines, close the box
        if (lines.Count == 0)
        {
            HideDialogue();
            return;
        }

        // grab next line
        string nextLine = lines.Dequeue();
        dialogueText.text = nextLine;

        // always show the continue hint so the player knows they can press SPACE
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
