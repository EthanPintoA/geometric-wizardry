using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager DialogueManager;

    [Header("DialogueManager Context")]
    public string characterName;
    public string[] dialogue;

    private void OnTriggerEnter(Collider other)
    {
        DialogueManager.StartDialogue(characterName, dialogue);
        Destroy(gameObject);
    }
}
