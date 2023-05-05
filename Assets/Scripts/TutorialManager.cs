using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public GameObject dialogueTrigger1;
    public GameObject dialogueTrigger2;
    public GameObject doorTrigger;

    private uint eventIdx = 0;

    private void Update()
    {
        // Check if starting dialogue trigger has been triggered
        if (eventIdx == 0)
        {
            if (!dialogueTrigger1.activeSelf)
            {
                GotoNextEvent();
            }

        }
        // Check if starting dialogue has ended
        else if (eventIdx == 1)
        {
            if (!dialogueBox.activeSelf)
            {
                dialogueTrigger2.SetActive(true);
                GotoNextEvent();
            }
        }
        // Check if spell tutorial dialogue trigger has been triggered
        else if (eventIdx == 2)
        {
            if (!dialogueTrigger2.activeSelf)
            {
                GotoNextEvent();
            }
        }
        // Check if spell tutorial dialogue has ended
        else if (eventIdx == 3)
        {
            if (!dialogueBox.activeSelf)
            {
                doorTrigger.SetActive(true);
                GotoNextEvent();
            }
        }
    }

    private void GotoNextEvent()
    {
        eventIdx += 1;
        Debug.Log($"Activating event {eventIdx}");
    }
}