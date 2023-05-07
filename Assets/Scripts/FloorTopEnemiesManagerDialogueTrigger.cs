using UnityEngine;

public class FloorTopEnemiesManagerDialogueTrigger : MonoBehaviour
{
    public GameObject dialogueTrigger;
    public GameObject dialogueBox;
    public GameObject elevatorDoorTrigger;
    public GameObject timer;
    public GameObject enemies;

    private int eventIdx = 0;

    void Update()
    {
        // Wait for all enemies to die
        if (eventIdx == 0)
        {
            if (enemies.transform.childCount == 0)
            {
                dialogueTrigger.SetActive(true);
                GoToNextEvent();
            }
        }
        // Wait for Player to talk to the guy
        else if (eventIdx == 1)
        {
            if (!dialogueTrigger.activeSelf)
            {
                GoToNextEvent();
            }
        }
        // Wait for guy to finish talking
        else if (eventIdx == 2)
        {
            if (!dialogueBox.activeSelf)
            {
                elevatorDoorTrigger.SetActive(true);
                timer.SetActive(true);
                GoToNextEvent();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void GoToNextEvent()
    {
        eventIdx += 1;
        Debug.Log($"Activating event {eventIdx}");
    }
}
