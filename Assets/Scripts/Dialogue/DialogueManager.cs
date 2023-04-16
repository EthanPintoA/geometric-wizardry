using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("GameObjects")]
    public Image dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private readonly Queue<string> sentences = new();

    public void StartDialogue(string name, string[] sentences)
    {
        if (!dialogueBox.IsActive())
        {
            dialogueBox.gameObject.SetActive(true);
        }

        nameText.text = name;

        this.sentences.Clear();

        foreach (string sentence in sentences)
        {
            this.sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            if (dialogueBox.IsActive())
            {
                dialogueBox.gameObject.SetActive(false);
            }
        }
        else
        {
            dialogueText.text = sentences.Dequeue();
        }
    }

    public void DisplayNextSentenceOnClick(InputAction.CallbackContext ctx)
    {
        if (ctx.started) { DisplayNextSentence(); }
    }
}
