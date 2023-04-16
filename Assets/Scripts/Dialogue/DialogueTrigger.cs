using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager DialogueManager;

    [Header("DialogueManager Context")]
    public string characterName;
    [TextArea]
    public string[] dialogue;

    private void OnTriggerEnter(Collider other)
    {
        DialogueManager.StartDialogue(characterName, dialogue);
        gameObject.SetActive(false);
    }
}
